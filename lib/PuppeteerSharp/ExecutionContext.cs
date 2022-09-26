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

        private readonly string _evaluationScriptSuffix = $"//# sourceURL={EvaluationScriptUrl}";
        private static readonly Regex _sourceUrlRegex = new(@"^[\040\t]*\/\/[@#] sourceURL=\s*(\S*?)\s*$", RegexOptions.Multiline);

        internal ExecutionContext(
            CDPSession client,
            ContextPayload contextPayload,
            DOMWorld world)
        {
            Client = client;
            ContextId = contextPayload.Id;
            ContextName = contextPayload.Name;
            World = world;
        }

        internal int ContextId { get; }

        internal string ContextName { get; }

        internal CDPSession Client { get; }

        internal DOMWorld World { get; }

        /// <inheritdoc/>
        public IFrame Frame => World?.Frame;

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
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args) => EvaluateFunctionAsync<JToken>(script, args);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => RemoteObjectTaskToObject<T>(EvaluateFunctionInternalAsync(true, script, args));

        /// <inheritdoc/>
        public async Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
        {
            if (prototypeHandle == null)
            {
                throw new ArgumentNullException(nameof(prototypeHandle));
            }

            if (prototypeHandle.Disposed)
            {
                throw new PuppeteerException("Prototype JSHandle is disposed!");
            }

            if (prototypeHandle.RemoteObject.ObjectId == null)
            {
                throw new PuppeteerException("Prototype JSHandle must not be referencing primitive value");
            }

            var response = await Client.SendAsync<RuntimeQueryObjectsResponse>("Runtime.queryObjects", new RuntimeQueryObjectsRequest
            {
                PrototypeObjectId = prototypeHandle.RemoteObject.ObjectId,
            }).ConfigureAwait(false);

            return CreateJSHandle(response.Objects);
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

        private Task<RemoteObject> EvaluateFunctionInternalAsync(bool returnByValue, string script, params object[] args)
            => ExecuteEvaluationAsync("Runtime.callFunctionOn", new RuntimeCallFunctionOnRequest
            {
                FunctionDeclaration = $"{script}\n{_evaluationScriptSuffix}\n",
                ExecutionContextId = ContextId,
                Arguments = args.Select(FormatArgument),
                ReturnByValue = returnByValue,
                AwaitPromise = true,
                UserGesture = true,
            });

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

        internal IJSHandle CreateJSHandle(RemoteObject remoteObject)
            => remoteObject.Subtype == RemoteObjectSubtype.Node && Frame != null
                ? new ElementHandle(this, Client, remoteObject, Frame, ((Frame)Frame).FrameManager.Page, ((Frame)Frame).FrameManager)
                : new JSHandle(this, Client, remoteObject);

        private object FormatArgument(object arg)
        {
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

                case JSHandle objectHandle:
                    return objectHandle.FormatArgument(this);
            }
            return new RuntimeCallFunctionOnRequestArgument
            {
                Value = arg,
            };
        }

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

        internal async Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId)
        {
            var obj = await Client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = backendNodeId,
                ExecutionContextId = ContextId,
            }).ConfigureAwait(false);

            return CreateJSHandle(obj.Object) as IElementHandle;
        }

        internal async Task<IElementHandle> AdoptElementHandleAsync(IElementHandle elementHandle)
        {
            if (elementHandle.ExecutionContext == this)
            {
                throw new PuppeteerException("Cannot adopt handle that already belongs to this execution context");
            }
            if (World == null)
            {
                throw new PuppeteerException("Cannot adopt handle without DOMWorld");
            }

            var nodeInfo = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = elementHandle.RemoteObject.ObjectId,
            }).ConfigureAwait(false);

            var obj = await Client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = nodeInfo.Node.BackendNodeId,
                ExecutionContextId = ContextId,
            }).ConfigureAwait(false);

            return CreateJSHandle(obj.Object) as ElementHandle;
        }
    }
}
