// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Script;
using ScriptMessageEventArgs = WebDriverBiDi.Script.MessageEventArgs;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// Represents an exposed function that can be called from the browser.
/// This is the BiDi implementation equivalent to the upstream ExposedFunction.ts.
/// </summary>
internal class ExposableFunction : IAsyncDisposable
{
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new();

    private readonly BidiFrame _frame;
    private readonly string _name;
    private readonly Delegate _apply;
    private readonly string _channel;
    private readonly List<(BidiFrame Frame, string ScriptId)> _scripts = [];

    private EventObserver<ScriptMessageEventArgs> _messageObserver;

    private ExposableFunction(BidiFrame frame, string name, Delegate apply)
    {
        _frame = frame;
        _name = name;
        _apply = apply;
        _channel = $"__puppeteer__{frame.Id}_page_exposeFunction_{name}";
    }

    /// <summary>
    /// Gets the name of the exposed function.
    /// </summary>
    public string Name => _name;

    private Session Session => _frame.BrowsingContext.Session;

    /// <summary>
    /// Creates a new ExposableFunction and initializes it.
    /// </summary>
    /// <param name="frame">The frame to expose the function on.</param>
    /// <param name="name">The name of the function.</param>
    /// <param name="apply">The delegate to invoke.</param>
    /// <returns>The initialized ExposableFunction.</returns>
    public static async Task<ExposableFunction> FromAsync(BidiFrame frame, string name, Delegate apply)
    {
        var func = new ExposableFunction(frame, name, apply);
        await func.InitializeAsync().ConfigureAwait(false);
        return func;
    }

    /// <summary>
    /// Disposes the exposed function, removing it from the browser.
    /// </summary>
    /// <returns>A task that completes when disposal is complete.</returns>
    public async ValueTask DisposeAsync()
    {
        // Unsubscribe from message events
        if (_messageObserver != null)
        {
            _messageObserver.Unobserve();
            _messageObserver = null;
        }

        // Remove preload scripts and delete the global function
        foreach (var (frame, scriptId) in _scripts)
        {
            try
            {
                var realm = frame.MainRealm as BidiFrameRealm;

                // Delete the global function
                await realm.EvaluateFunctionAsync(
                    @"name => { delete globalThis[name]; }",
                    _name).ConfigureAwait(false);

                // Delete from child frames too
                foreach (var childFrame in frame.ChildFrames.OfType<BidiFrame>())
                {
                    var childRealm = childFrame.MainRealm as BidiFrameRealm;
                    try
                    {
                        await childRealm.EvaluateFunctionAsync(
                            @"name => { delete globalThis[name]; }",
                            _name).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Ignore errors from child frames
                    }
                }

                // Remove the preload script
                await frame.BrowsingContext.Session.Driver.Script.RemovePreloadScriptAsync(
                    new RemovePreloadScriptCommandParameters(scriptId)).ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }

        _scripts.Clear();
    }

    private static string EscapeJsString(string value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value, _jsonOptions);
    }

    private async Task InitializeAsync()
    {
        var channel = new ChannelValue(new ChannelProperties(_channel)
        {
            Ownership = ResultOwnership.Root,
        });

        // Subscribe to script.message events
        // First, subscribe to the event in the browser
        await Session.SubscribeAsync(["script.message"], [_frame.BrowsingContext.Id]).ConfigureAwait(false);
        _messageObserver = Session.Driver.Script.OnMessage.AddObserver(HandleMessage);

        // JavaScript function that will be injected as a preload script
        // This creates a global function that when called, sends a message via the channel
        var functionDeclaration = $@"(callback) => {{
            Object.assign(globalThis, {{
                [{EscapeJsString(_name)}]: function (...args) {{
                    return new Promise((resolve, reject) => {{
                        callback([resolve, reject, args]);
                    }});
                }},
            }});
        }}";

        // Collect all frames (main frame + child frames)
        var frames = new List<BidiFrame> { _frame };
        for (var i = 0; i < frames.Count; i++)
        {
            frames.AddRange(frames[i].ChildFrames.OfType<BidiFrame>());
        }

        // Add preload script and execute immediately for each frame
        foreach (var frame in frames)
        {
            var realm = frame.MainRealm as BidiFrameRealm;
            try
            {
                // Add preload script for future navigations
                var commandParameters = new AddPreloadScriptCommandParameters(functionDeclaration)
                {
                    Contexts = [frame.BrowsingContext.Id],
                    Arguments = [channel],
                };

                var addScriptTask = Session.Driver.Script.AddPreloadScriptAsync(commandParameters);

                // Also execute immediately for current context
                // NOTE: This requires WebDriverBiDi >= 0.0.38 for ChannelValue serialization support.
                // With older versions, only pages loaded after ExposeFunctionAsync is called will have the function available.
                var callFunctionTask = realm.WindowRealm.CallFunctionAsync(
                    functionDeclaration,
                    false,
                    new CallFunctionParameters { Arguments = new List<ArgumentValue> { channel } });

                await Task.WhenAll(addScriptTask, callFunctionTask).ConfigureAwait(false);
                var addScriptResult = await addScriptTask.ConfigureAwait(false);
                _scripts.Add((frame, addScriptResult.PreloadScriptId));
            }
            catch
            {
                // If it errors, the frame probably doesn't support call function
                // We fail gracefully like upstream does
            }
        }
    }

    private void HandleMessage(ScriptMessageEventArgs args)
    {
        if (args.ChannelId != _channel)
        {
            return;
        }

        // Find the realm from the source
        var realm = GetRealm(args.Source);
        if (realm == null)
        {
            // Unrelated message
            return;
        }

        // Handle the message asynchronously - fire and forget
        _ = HandleMessageAsync(args, realm);
    }

    private async Task HandleMessageAsync(ScriptMessageEventArgs args, BidiRealm realm)
    {
        // Create a handle from the data (which contains [resolve, reject, args])
        var dataHandle = BidiJSHandle.From(args.Data, realm);

        try
        {
            // Extract the args array from the data
            var argsHandle = await dataHandle.EvaluateFunctionHandleAsync("([, , args]) => args").ConfigureAwait(false);

            // Get the properties (individual arguments)
            var properties = await argsHandle.GetPropertiesAsync().ConfigureAwait(false);

            // Convert args to .NET types
            var methodParams = _apply.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            var dotNetArgs = new object[methodParams.Length];

            var sortedProperties = properties.OrderBy(kvp => int.TryParse(kvp.Key, out var n) ? n : int.MaxValue).ToList();

            for (var i = 0; i < methodParams.Length && i < sortedProperties.Count; i++)
            {
                var handle = sortedProperties[i].Value;

                // For element handles, pass them as-is
                if (handle is BidiElementHandle elementHandle)
                {
                    dotNetArgs[i] = elementHandle;
                }
                else
                {
                    // For other types, get the JSON value
                    dotNetArgs[i] = await handle.JsonValueAsync<object>().ConfigureAwait(false);

                    // Convert to the expected parameter type
                    if (dotNetArgs[i] != null && methodParams[i] != typeof(object))
                    {
                        dotNetArgs[i] = Convert.ChangeType(dotNetArgs[i], methodParams[i], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }

            // Dispose handles
            foreach (var prop in properties.Values)
            {
                await prop.DisposeAsync().ConfigureAwait(false);
            }

            await argsHandle.DisposeAsync().ConfigureAwait(false);

            // Call the .NET function
            var result = await BindingUtils.ExecuteBindingAsync(_apply, dotNetArgs).ConfigureAwait(false);

            // Resolve the promise with the result
            try
            {
                await dataHandle.EvaluateFunctionAsync(
                    "([resolve], result) => { resolve(result); }",
                    result).ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors when resolving
            }
        }
        catch (Exception error)
        {
            // Reject the promise with the error
            try
            {
                await dataHandle.EvaluateFunctionAsync(
                    @"([, reject], name, message, stack) => {
                        const error = new Error(message);
                        error.name = name;
                        if (stack) {
                            error.stack = stack;
                        }
                        reject(error);
                    }",
                    error.GetType().Name,
                    error.Message,
                    error.StackTrace).ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors when rejecting
            }
        }
        finally
        {
            await dataHandle.DisposeAsync().ConfigureAwait(false);
        }
    }

    private BidiRealm GetRealm(Source source)
    {
        var frame = FindFrame(source.Context);
        if (frame == null)
        {
            return null;
        }

        // Find the realm by ID
        if (frame.MainRealm is BidiFrameRealm mainRealm && mainRealm.WindowRealm.Id == source.RealmId)
        {
            return mainRealm;
        }

        if (frame.IsolatedRealm is BidiFrameRealm isolatedRealm && isolatedRealm.WindowRealm.Id == source.RealmId)
        {
            return isolatedRealm;
        }

        // Default to main realm
        return frame.MainRealm as BidiRealm;
    }

    private BidiFrame FindFrame(string contextId)
    {
        var frames = new List<BidiFrame> { _frame };
        for (var i = 0; i < frames.Count; i++)
        {
            if (frames[i].Id == contextId)
            {
                return frames[i];
            }

            frames.AddRange(frames[i].ChildFrames.OfType<BidiFrame>());
        }

        return null;
    }
}
