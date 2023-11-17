using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class ExecutionContext : IExecutionContext
    {
        internal const string EvaluationScriptUrl = "__puppeteer_evaluation_script__";

        private static readonly Regex _sourceUrlRegex = new(@"^[\040\t]*\/\/[@#] sourceURL=\s*\S*?\s*$", RegexOptions.Multiline);

        private readonly string _evaluationScriptSuffix = $"//# sourceURL={EvaluationScriptUrl}";
        private readonly TaskQueue _puppeteerUtilQueue = new();
        private IJSHandle _puppeteerUtil;

        internal ExecutionContext(
            CDPSession client,
            ContextPayload contextPayload,
            IsolatedWorld world)
        {
            Client = client;
            ContextId = contextPayload.Id;
            ContextName = contextPayload.Name;
            World = world;
        }

        /// <inheritdoc/>
        IFrame IExecutionContext.Frame => World?.Frame;

        internal Frame Frame => World?.Frame;

        internal int ContextId { get; }

        internal string ContextName { get; }

        internal CDPSession Client { get; }

        internal IsolatedWorld World { get; }

        /// <inheritdoc/>
        public Task<JToken> EvaluateExpressionAsync(string script) => EvaluateExpressionAsync<JToken>(script);

        /// <inheritdoc/>
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => RemoteObjectTaskToObject<T>(EvaluateExpressionInternalAsync(true, script));

        /// <inheritdoc/>
        public async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
            => CreateJSHandle(await EvaluateExpressionInternalAsync(false, script).ConfigureAwait(false));

        /// <inheritdoc/>
        public async Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
            => CreateJSHandle(await EvaluateFunctionInternalAsync(false, script, args).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => EvaluateFunctionAsync<JToken>(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => RemoteObjectTaskToObject<T>(EvaluateFunctionInternalAsync(true, script, args));

        internal async Task<IJSHandle> GetPuppeteerUtilAsync()
        {
            await _puppeteerUtilQueue.Enqueue(async () =>
            {
                if (_puppeteerUtil == null)
                {
                    await Client.Connection.ScriptInjector.InjectAsync(
                        async (string script) =>
                        {
                            if (_puppeteerUtil != null)
                            {
                                await _puppeteerUtil.DisposeAsync().ConfigureAwait(false);
                            }

                            await InstallGlobalBindingAsync(new Binding(
                                "__ariaQuerySelector",
                                (Func<IElementHandle, string, Task<IElementHandle>>)Client.Connection.CustomQuerySelectorRegistry.InternalQueryHandlers["aria"].QueryOneAsync))
                                .ConfigureAwait(false);
                            _puppeteerUtil = await EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
                        },
                        _puppeteerUtil == null).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
            return _puppeteerUtil;
        }

        internal IJSHandle CreateJSHandle(RemoteObject remoteObject)
            => remoteObject.Subtype == RemoteObjectSubtype.Node && Frame != null
                ? new ElementHandle(World, remoteObject)
                : new JSHandle(World, remoteObject);

        private static string GetExceptionMessage(EvaluateExceptionResponseDetails exceptionDetails)
        {
            if (exceptionDetails.Exception != null)
            {
                return exceptionDetails.Exception.Description ?? exceptionDetails.Exception.Value;
            }

            var message = exceptionDetails.Text;
            if (exceptionDetails.StackTrace != null)
            {
                foreach (var callframe in exceptionDetails.StackTrace.CallFrames)
                {
                    var location = $"{callframe.Url}:{callframe.LineNumber}:{callframe.ColumnNumber}";
                    var functionName = string.IsNullOrEmpty(callframe.FunctionName) ? "<anonymous>" : callframe.FunctionName;
                    message += $"\n at ${functionName} (${location})";
                }
            }

            return message;
        }

        private async Task InstallGlobalBindingAsync(Binding binding)
        {
            try
            {
                if (World != null)
                {
                    World.Bindings.TryAdd(binding.Name, binding);
                    await World.AddBindingToContextAsync(this, binding.Name).ConfigureAwait(false);
                }
            }
            catch
            {
                // If the binding cannot be added, then either the browser doesn't support
                // bindings (e.g. Firefox) or the context is broken. Either breakage is
                // okay, so we ignore the error.
            }
        }

        private async Task<T> RemoteObjectTaskToObject<T>(Task<RemoteObject> remote)
        {
            var response = await remote.ConfigureAwait(false);
            return response == null ? default : (T)RemoteObjectHelper.ValueFromRemoteObject<T>(response);
        }

        private Task<RemoteObject> EvaluateExpressionInternalAsync(bool returnByValue, string script)
            => ExecuteEvaluationAsync("Runtime.evaluate", new Dictionary<string, object>
            {
                ["expression"] = _sourceUrlRegex.IsMatch(script) ? script : $"{script}\n{_evaluationScriptSuffix}",
                ["contextId"] = ContextId,
                ["returnByValue"] = returnByValue,
                ["awaitPromise"] = true,
                ["userGesture"] = true,
            });

        private async Task<RemoteObject> EvaluateFunctionInternalAsync(bool returnByValue, string script, params object[] args)
            => await ExecuteEvaluationAsync("Runtime.callFunctionOn", new RuntimeCallFunctionOnRequest
            {
                FunctionDeclaration = $"{script}\n{_evaluationScriptSuffix}\n",
                ExecutionContextId = ContextId,
                Arguments = await Task.WhenAll(args.Select(FormatArgumentAsync).ToArray()).ConfigureAwait(false),
                ReturnByValue = returnByValue,
                AwaitPromise = true,
                UserGesture = true,
            }).ConfigureAwait(false);

        private async Task<RemoteObject> ExecuteEvaluationAsync(string method, object args)
        {
            try
            {
                var response = await Client.SendAsync<EvaluateHandleResponse>(method, args).ConfigureAwait(false);

                if (response.ExceptionDetails != null)
                {
                    throw new EvaluationFailedException("Evaluation failed: " +
                        GetExceptionMessage(response.ExceptionDetails));
                }

                return response.Result;
            }
            catch (MessageException ex)
            {
                if (ex.Message.Contains("Object reference chain is too long") ||
                    ex.Message.Contains("Object couldn't be returned by value"))
                {
                    return default;
                }

                throw new EvaluationFailedException(ex.Message, ex);
            }
        }

        private async Task<object> FormatArgumentAsync(object arg)
        {
            if (arg is TaskCompletionSource<object> tcs)
            {
                arg = await tcs.Task.ConfigureAwait(false);
            }

            if (arg is LazyArg lazyArg)
            {
                arg = await lazyArg(this).ConfigureAwait(false);
            }

            switch (arg)
            {
                case BigInteger big:
                    return new { unserializableValue = $"{big}n" };

                case int integer when integer == -0:
                    return new { unserializableValue = "-0" };

                case double d:
                    if (double.IsPositiveInfinity(d))
                    {
                        return new { unserializableValue = "Infinity" };
                    }

                    if (double.IsNegativeInfinity(d))
                    {
                        return new { unserializableValue = "-Infinity" };
                    }

                    if (double.IsNaN(d))
                    {
                        return new { unserializableValue = "NaN" };
                    }

                    break;

                case IJSHandle objectHandle:
                    return objectHandle.FormatArgument(this);
            }

            return new RuntimeCallFunctionOnRequestArgument
            {
                Value = arg,
            };
        }
    }
}
