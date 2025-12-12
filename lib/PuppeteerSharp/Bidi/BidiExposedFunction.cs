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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// Handles exposing .NET functions to the browser via WebDriver BiDi protocol.
/// </summary>
internal class BidiExposedFunction : IAsyncDisposable
{
    private readonly BidiFrame _frame;
    private readonly string _name;
    private readonly Delegate _function;
    private readonly string _channel;
    private readonly List<(BidiFrame Frame, string ScriptId)> _scripts = new();
    private EventObserver<WebDriverBiDi.Script.MessageEventArgs> _messageObserver;

    private BidiExposedFunction(BidiFrame frame, string name, Delegate function)
    {
        _frame = frame;
        _name = name;
        _function = function;
        _channel = $"__puppeteer__{_frame.Id}_page_exposeFunction_{_name}";
    }

    /// <summary>
    /// Creates and initializes a new BidiExposedFunction.
    /// </summary>
    /// <param name="frame">The frame to expose the function in.</param>
    /// <param name="name">The name of the function to expose.</param>
    /// <param name="function">The delegate to invoke when the function is called.</param>
    /// <returns>A task that completes when the function has been exposed.</returns>
    public static async Task<BidiExposedFunction> FromAsync(BidiFrame frame, string name, Delegate function)
    {
        var exposedFunction = new BidiExposedFunction(frame, name, function);
        await exposedFunction.InitializeAsync().ConfigureAwait(false);
        return exposedFunction;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Unsubscribe from message events
        _messageObserver?.Unobserve();

        // Remove preload scripts and delete the function from globalThis
        foreach (var (frame, scriptId) in _scripts)
        {
            try
            {
                var tasks = new List<Task>();

                // Delete from main frame
                tasks.Add(frame.EvaluateFunctionAsync(
                    "name => { delete globalThis[name]; }",
                    _name));

                // Delete from child frames
                foreach (var childFrame in frame.ChildFrames.Cast<BidiFrame>())
                {
                    tasks.Add(childFrame.EvaluateFunctionAsync(
                        "name => { delete globalThis[name]; }",
                        _name));
                }

                // Remove preload script
                tasks.Add(_frame.BidiPage.BidiBrowser.Driver.Script.RemovePreloadScriptAsync(
                    new RemovePreloadScriptCommandParameters(scriptId)));

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    private static object ConvertArgument(object arg, Type targetType)
    {
        if (arg == null)
        {
            return GetDefaultValue(targetType);
        }

        if (targetType.IsAssignableFrom(arg.GetType()))
        {
            return arg;
        }

        // Handle numeric conversions
        if (IsNumericType(targetType))
        {
            return Convert.ChangeType(arg, targetType, CultureInfo.InvariantCulture);
        }

        // Handle JsonElement
        if (arg is JsonElement jsonElement)
        {
            return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
        }

        return arg;
    }

    private static object GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(int) ||
               type == typeof(long) ||
               type == typeof(short) ||
               type == typeof(byte) ||
               type == typeof(float) ||
               type == typeof(double) ||
               type == typeof(decimal);
    }

    private static object ExtractValue(RemoteValue remoteValue)
    {
        return remoteValue.Type switch
        {
            "undefined" => null,
            "null" => null,
            _ => remoteValue.Value,
        };
    }

    private async Task InitializeAsync()
    {
        var channelProperties = new ChannelProperties(_channel)
        {
            ResultOwnership = ResultOwnership.Root,
        };
        var channelValue = new ChannelValue(channelProperties);

        // Subscribe to message events
        _messageObserver = _frame.BidiPage.BidiBrowser.Driver.Script.OnMessage.AddObserver(HandleMessageAsync);

        // Create the function declaration that will be injected into the page
        var functionDeclaration = CreateFunctionDeclaration();

        // Get all frames (main frame and child frames)
        var frames = new List<BidiFrame> { _frame };
        CollectChildFrames(_frame, frames);

        // Install the function in all frames
        var installTasks = frames.Select(async frame =>
        {
            try
            {
                var frameRealm = frame.MainRealm as BidiFrameRealm;

                // Add preload script for future navigations
                var addPreloadScriptParams = new AddPreloadScriptCommandParameters(functionDeclaration)
                {
                    Arguments = [channelValue],
                    Sandbox = frameRealm?.WindowRealm?.Sandbox,
                };

                // Execute both: add preload script (for future navigations) and call function immediately
                var preloadScriptTask = _frame.BidiPage.BidiBrowser.Driver.Script
                    .AddPreloadScriptAsync(addPreloadScriptParams);

                // Call the function immediately with the channel argument
                var callFunctionTask = frameRealm?.WindowRealm?.CallFunctionAsync(
                    functionDeclaration,
                    awaitPromise: false,
                    new CallFunctionParameters
                    {
                        Arguments = [channelValue],
                    });

                var scriptResult = await preloadScriptTask.ConfigureAwait(false);
                if (callFunctionTask != null)
                {
                    await callFunctionTask.ConfigureAwait(false);
                }

                _scripts.Add((frame, scriptResult.PreloadScriptId));
            }
            catch (Exception ex)
            {
                // Re-throw for now to see what's happening
                System.Diagnostics.Debug.WriteLine($"Error in InitializeAsync: {ex}");
                throw;
            }
        });

        await Task.WhenAll(installTasks).ConfigureAwait(false);
    }

    private void CollectChildFrames(BidiFrame frame, List<BidiFrame> frames)
    {
        foreach (var childFrame in frame.ChildFrames.Cast<BidiFrame>())
        {
            frames.Add(childFrame);
            CollectChildFrames(childFrame, frames);
        }
    }

    private string CreateFunctionDeclaration()
    {
        // This JavaScript function creates the exposed function in globalThis.
        // Functions cannot be serialized over BiDi, so we use a different approach:
        // 1. Store resolve/reject in a Map keyed by unique ID on the browser side
        // 2. Only send the ID and args through the channel
        // 3. Also expose __puppeteer_resolve__ and __puppeteer_reject__ functions that take an ID
        //    and call the corresponding resolve/reject from the Map
        var resolverMapName = $"__puppeteer__resolvers_{_name}";
        return $@"(callback) => {{
            const resolvers = new Map();
            globalThis.{resolverMapName} = resolvers;
            Object.assign(globalThis, {{
                [{JsonSerializer.Serialize(_name)}]: function (...args) {{
                    return new Promise((resolve, reject) => {{
                        const id = Math.random().toString(36).substring(2) + Date.now().toString(36);
                        resolvers.set(id, {{ resolve, reject }});
                        callback({{ id, args }});
                    }});
                }}
            }});
        }}";
    }

    private async void HandleMessageAsync(WebDriverBiDi.Script.MessageEventArgs args)
    {
        if (args.ChannelId != _channel)
        {
            return;
        }

        // Find the realm for this message
        var realm = FindRealm(args.Source);
        if (realm == null)
        {
            return;
        }

        var resolverMapName = $"__puppeteer__resolvers_{_name}";
        string callId = null;

        try
        {
            // The data object is { id, args } - we need to extract these
            // Since functions can't be serialized, we can't get a handle to the data.
            // Instead, we evaluate to get the id and args directly.
            var dataValue = args.Data.Value;
            var argsList = new List<object>();

            if (dataValue is WebDriverBiDi.Script.RemoteValueDictionary dict)
            {
                // Extract id
                if (dict.TryGetValue("id", out var idValue) && idValue.Value is string id)
                {
                    callId = id;
                }

                // Extract args
                if (dict.TryGetValue("args", out var argsValue) && argsValue.Value is WebDriverBiDi.Script.RemoteValueList argValues)
                {
                    foreach (var argValue in argValues)
                    {
                        argsList.Add(ExtractValue(argValue));
                    }
                }
            }

            if (callId == null)
            {
                return;
            }

            // Call the .NET function with the arguments
            object result;
            try
            {
                result = InvokeFunction(argsList.ToArray());

                // If the result is a Task, await it
                if (result is Task task)
                {
                    await task.ConfigureAwait(false);

                    // Get the result from the Task if it has one
                    var taskType = task.GetType();
                    if (taskType.IsGenericType)
                    {
                        var resultProperty = taskType.GetProperty("Result");
                        result = resultProperty?.GetValue(task);
                    }
                    else
                    {
                        result = null;
                    }
                }

                // Resolve the promise by looking up the resolver in the Map and calling resolve
                await realm.EvaluateFunctionAsync(
                    $@"(id, result) => {{
                        const resolvers = globalThis.{resolverMapName};
                        const resolver = resolvers?.get(id);
                        if (resolver) {{
                            resolvers.delete(id);
                            resolver.resolve(result);
                        }}
                    }}",
                    callId,
                    result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Unwrap TargetInvocationException to get the actual exception
                var actualException = ex is TargetInvocationException && ex.InnerException != null
                    ? ex.InnerException
                    : ex;

                // Reject the promise with the error
                var errorName = actualException.GetType().Name;
                var errorMessage = actualException.Message;

                // Build the stack trace with the .NET class name included
                var errorStack = $"{actualException.GetType().Name}: {actualException.Message}\n{actualException.StackTrace}";

                try
                {
                    await realm.EvaluateFunctionAsync(
                        $@"(id, name, message, stack) => {{
                            const resolvers = globalThis.{resolverMapName};
                            const resolver = resolvers?.get(id);
                            if (resolver) {{
                                resolvers.delete(id);
                                const error = new Error(message);
                                error.name = name;
                                if (stack) {{
                                    error.stack = stack;
                                }}
                                resolver.reject(error);
                            }}
                        }}",
                        callId,
                        errorName,
                        errorMessage,
                        errorStack).ConfigureAwait(false);
                }
                catch
                {
                    // Ignore errors when rejecting
                }
            }
        }
        catch (Exception outerEx)
        {
            // Try to reject the promise if we have a callId
            if (callId != null)
            {
                try
                {
                    await realm.EvaluateFunctionAsync(
                        $@"(id, name, message, stack) => {{
                            const resolvers = globalThis.{resolverMapName};
                            const resolver = resolvers?.get(id);
                            if (resolver) {{
                                resolvers.delete(id);
                                const error = new Error(message);
                                error.name = name;
                                if (stack) {{
                                    error.stack = stack;
                                }}
                                resolver.reject(error);
                            }}
                        }}",
                        callId,
                        outerEx.GetType().Name,
                        outerEx.Message,
                        outerEx.StackTrace).ConfigureAwait(false);
                }
                catch
                {
                    // Ignore errors when rejecting
                }
            }
        }
    }

    private BidiRealm FindRealm(Source source)
    {
        var frame = FindFrame(source.Context);
        if (frame == null)
        {
            return null;
        }

        // Try to find the realm by ID
        var defaultRealm = frame.MainRealm as BidiFrameRealm;
        if (defaultRealm?.WindowRealm?.Id == source.RealmId)
        {
            return defaultRealm;
        }

        var internalRealm = frame.IsolatedRealm as BidiFrameRealm;
        if (internalRealm?.WindowRealm?.Id == source.RealmId)
        {
            return internalRealm;
        }

        return defaultRealm;
    }

    private BidiFrame FindFrame(string contextId)
    {
        var frames = new List<BidiFrame> { _frame };
        foreach (var frame in frames)
        {
            if (frame.Id == contextId)
            {
                return frame;
            }

            foreach (var childFrame in frame.ChildFrames.Cast<BidiFrame>())
            {
                frames.Add(childFrame);
            }
        }

        return null;
    }

    private object InvokeFunction(object[] args)
    {
        var methodInfo = _function.GetMethodInfo();
        var parameters = methodInfo.GetParameters();

        // Convert arguments to match parameter types
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
            else
            {
                convertedArgs[i] = GetDefaultValue(parameters[i].ParameterType);
            }
        }

        return _function.DynamicInvoke(convertedArgs);
    }
}
