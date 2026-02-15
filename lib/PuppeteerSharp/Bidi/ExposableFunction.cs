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

#if !CDP_ONLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi;
using WebDriverBiDi.Script;
using BrowsingContext = PuppeteerSharp.Bidi.Core.BrowsingContext;
using ScriptMessageEventArgs = WebDriverBiDi.Script.MessageEventArgs;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// Manages an exposed function that can be called from the browser context.
/// Similar to upstream's ExposableFunction class.
/// </summary>
internal class ExposableFunction : IAsyncDisposable
{
    private readonly BidiFrame _frame;
    private readonly string _name;
    private readonly Delegate _puppeteerFunction;
    private readonly string _channel;
    private readonly List<(BidiFrame Frame, string ScriptId)> _scripts = new();
    private EventObserver<ScriptMessageEventArgs> _observer;
    private int _disposed;

    private ExposableFunction(BidiFrame frame, string name, Delegate puppeteerFunction)
    {
        _frame = frame;
        _name = name;
        _puppeteerFunction = puppeteerFunction;
        _channel = $"__puppeteer__{frame.Id}_page_exposeFunction_{name}";
    }

    /// <summary>
    /// Gets the name of the exposed function.
    /// </summary>
    public string Name => _name;

    private BiDiDriver Connection => _frame.BidiPage.BidiBrowser.Driver;

    /// <summary>
    /// Creates and initializes an ExposableFunction.
    /// </summary>
    public static async Task<ExposableFunction> FromAsync(BidiFrame frame, string name, Delegate puppeteerFunction)
    {
        var func = new ExposableFunction(frame, name, puppeteerFunction);
        await func.InitializeAsync().ConfigureAwait(false);
        return func;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        // Ensure we only dispose once using atomic operation
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        // Unsubscribe from messages using atomic exchange
        var observer = Interlocked.Exchange(ref _observer, null);
        observer?.Unobserve();

        // Remove preload scripts and delete the function from globalThis
        foreach (var (frame, scriptId) in _scripts)
        {
            try
            {
                var tasks = new List<Task>
                {
                    frame.MainRealm.EvaluateFunctionAsync(
                        @"name => { delete globalThis[name]; }",
                        _name),
                };

                // Also delete from child frames
                foreach (var childFrame in frame.ChildFrames.OfType<BidiFrame>())
                {
                    tasks.Add(childFrame.MainRealm.EvaluateFunctionAsync(
                        @"name => { delete globalThis[name]; }",
                        _name));
                }

                // Remove the preload script
                tasks.Add(Connection.Script.RemovePreloadScriptAsync(
                    new RemovePreloadScriptCommandParameters(scriptId)));

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }

        _scripts.Clear();
    }

    private async Task InitializeAsync()
    {
        var channelValue = new ChannelValue(new ChannelProperties(_channel)
        {
            Ownership = ResultOwnership.Root,
        });

        // Subscribe to script messages
        _observer = Connection.Script.OnMessage.AddObserver(HandleMessage);

        // The function declaration that will be injected
        // This creates a function on globalThis that communicates back via the channel
        var functionDeclaration = $@"(callback) => {{
            Object.assign(globalThis, {{
                [{JsonSerializer.Serialize(_name)}]: function (...args) {{
                    return new Promise((resolve, reject) => {{
                        callback([resolve, reject, args]);
                    }});
                }},
            }});
        }}";

        // Collect all browsing contexts (main context + child contexts)
        var contexts = new List<BrowsingContext> { _frame.BrowsingContext };
        for (var i = 0; i < contexts.Count; i++)
        {
            contexts.AddRange(contexts[i].Children);
        }

        // Add preload script only to the top-level context (for future navigations)
        var addPreloadParams = new AddPreloadScriptCommandParameters(functionDeclaration)
        {
            Arguments = [channelValue],
            Contexts = [_frame.BrowsingContext.Id],
        };
        var scriptResult = await Connection.Script.AddPreloadScriptAsync(addPreloadParams).ConfigureAwait(false);
        _scripts.Add((_frame, scriptResult.PreloadScriptId));

        // Call the function immediately on all contexts (main + children)
        var tasks = contexts.Select(async context =>
        {
            try
            {
                var callParams = new CallFunctionCommandParameters(functionDeclaration, context.DefaultRealm.Target, true);
                callParams.Arguments.Add(channelValue);
                await Connection.Script.CallFunctionAsync(callParams).ConfigureAwait(false);
            }
            catch
            {
                // If it errors, the frame probably doesn't support call function.
                // We fail gracefully.
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private async void HandleMessage(ScriptMessageEventArgs message)
    {
        // Wrap entire method body in try-catch since async void can't propagate exceptions
        try
        {
            // Check if disposed
            if (_disposed == 1)
            {
                return;
            }

            if (message.ChannelId != _channel)
            {
                return;
            }

            var realm = GetRealm(message.Source);
            if (realm == null)
            {
                // Unrelated message
                return;
            }

            // Create a handle for the data (which is [resolve, reject, args])
            var dataHandle = BidiJSHandle.From(message.Data, realm);

            try
            {
                // Get the args from the data
                var argsHandle = await dataHandle.EvaluateFunctionHandleAsync("([, , args]) => args").ConfigureAwait(false);

                // Get all properties (array indices)
                var properties = await argsHandle.GetPropertiesAsync().ConfigureAwait(false);
                var args = new List<object>();

                foreach (var kvp in properties.OrderBy(p => int.TryParse(p.Key, out var i) ? i : int.MaxValue))
                {
                    var handle = kvp.Value;

                    // Element handles are passed as is, everything else as JSON value
                    if (handle is BidiElementHandle elementHandle)
                    {
                        args.Add(elementHandle);
                    }
                    else
                    {
                        args.Add(await handle.JsonValueAsync<object>().ConfigureAwait(false));
                    }
                }

                // Call the puppeteer function
                object result;
                try
                {
                    result = await InvokeFunctionAsync(args.ToArray()).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Unwrap TargetInvocationException to get the actual exception
                    var actualException = ex is System.Reflection.TargetInvocationException tie && tie.InnerException != null
                        ? tie.InnerException
                        : ex;

                    // Reject with error
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
                            actualException.GetType().Name,
                            actualException.Message,
                            actualException.StackTrace ?? string.Empty).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Ignore errors when rejecting
                    }

                    return;
                }

                // Resolve with result
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
            finally
            {
                await dataHandle.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            // Swallow any exceptions in async void handler to prevent crashes
            // Errors are expected when the page/frame is closed during callback execution
        }
    }

    private async Task<object> InvokeFunctionAsync(object[] args)
    {
        // Convert args to the expected parameter types
        var methodInfo = _puppeteerFunction.Method;
        var parameters = methodInfo.GetParameters();
        var convertedArgs = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            if (i < args.Length)
            {
                convertedArgs[i] = ConvertArgument(args[i], parameters[i].ParameterType);
            }
            else if (parameters[i].HasDefaultValue)
            {
                convertedArgs[i] = parameters[i].DefaultValue;
            }
            else if (!parameters[i].ParameterType.IsValueType || Nullable.GetUnderlyingType(parameters[i].ParameterType) != null)
            {
                // Reference types and nullable value types can accept null
                convertedArgs[i] = null;
            }
            else
            {
                throw new ArgumentException($"Missing required argument for parameter '{parameters[i].Name}' at index {i}");
            }
        }

        var result = _puppeteerFunction.DynamicInvoke(convertedArgs);

        // If the result is a Task, await it
        if (result is Task task)
        {
            await task.ConfigureAwait(false);

            // Get the result if it's a Task<T>
            var taskType = task.GetType();
            if (taskType.IsGenericType)
            {
                var resultProperty = taskType.GetProperty("Result");
                return resultProperty?.GetValue(task);
            }

            return null;
        }

        return result;
    }

    private object ConvertArgument(object arg, Type targetType)
    {
        if (arg == null)
        {
            return null;
        }

        // Handle JsonElement conversion
        if (targetType == typeof(JsonElement) && arg is not JsonElement)
        {
            var json = JsonSerializer.Serialize(arg);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        // Handle numeric conversions with overflow checking
        if (targetType == typeof(int))
        {
            if (arg is long longVal)
            {
                checked
                {
                    return (int)longVal;
                }
            }

            if (arg is double doubleVal)
            {
                checked
                {
                    return (int)doubleVal;
                }
            }
        }

        if (targetType == typeof(double) && arg is long longVal2)
        {
            return (double)longVal2;
        }

        return arg;
    }

    private BidiRealm GetRealm(Source source)
    {
        var frame = FindFrame(source.Context);
        if (frame == null)
        {
            return null;
        }

        // Find the realm that matches the source realm ID
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
            var frame = frames[i];
            if (frame.Id == contextId)
            {
                return frame;
            }

            frames.AddRange(frame.ChildFrames.OfType<BidiFrame>());
        }

        return null;
    }
}

#endif
