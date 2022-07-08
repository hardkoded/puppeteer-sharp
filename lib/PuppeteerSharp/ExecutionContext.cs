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
    /// <summary>
    /// The class represents a context for JavaScript execution. Examples of JavaScript contexts are:
    /// Each <see cref="Frame"/> has a separate <see cref="ExecutionContext"/>
    /// All kind of web workers have their own contexts
    /// </summary>
    public class ExecutionContext
    {
        internal const string EvaluationScriptUrl = "__puppeteer_evaluation_script__";

        private readonly string _evaluationScriptSuffix = $"//# sourceURL={EvaluationScriptUrl}";
        private static readonly Regex _sourceUrlRegex = new(@"^[\040\t]*\/\/[@#] sourceURL=\s*(\S*?)\s*$", RegexOptions.Multiline);
        private readonly int _contextId;

        internal ExecutionContext(
            CDPSession client,
            ContextPayload contextPayload,
            DOMWorld world)
        {
            Client = client;
            _contextId = contextPayload.Id;
            World = world;
        }

        internal CDPSession Client { get; }

        internal DOMWorld World { get; }

        /// <summary>
        /// Frame associated with this execution context.
        /// </summary>
        /// <remarks>
        /// NOTE Not every execution context is associated with a frame. For example, workers and extensions have execution contexts that are not associated with frames.
        /// </remarks>
        public Frame Frame => World?.Frame;

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="EvaluateExpressionHandleAsync(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateExpressionAsync(string script) => EvaluateExpressionAsync<JToken>(script);

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// </remarks>
        /// <seealso cref="EvaluateFunctionAsync{T}(string, object[])"/>
        /// <seealso cref="EvaluateExpressionHandleAsync(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<T> EvaluateExpressionAsync<T>(string script)
            => RemoteObjectTaskToObject<T>(EvaluateExpressionInternalAsync(true, script));

        internal async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
            => CreateJSHandle(await EvaluateExpressionInternalAsync(false, script).ConfigureAwait(false));

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args) => EvaluateFunctionAsync<JToken>(script, args);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <seealso cref="EvaluateExpressionAsync{T}(string)"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => RemoteObjectTaskToObject<T>(EvaluateFunctionInternalAsync(true, script, args));

        internal async Task<JSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
            => CreateJSHandle(await EvaluateFunctionInternalAsync(false, script, args).ConfigureAwait(false));

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// </summary>
        /// <returns>A task which resolves to a handle to an array of objects with this prototype.</returns>
        /// <param name="prototypeHandle">A handle to the object prototype.</param>
        public async Task<JSHandle> QueryObjectsAsync(JSHandle prototypeHandle)
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
                PrototypeObjectId = prototypeHandle.RemoteObject.ObjectId
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
                ["contextId"] = _contextId,
                ["returnByValue"] = returnByValue,
                ["awaitPromise"] = true,
                ["userGesture"] = true
            });

        private Task<RemoteObject> EvaluateFunctionInternalAsync(bool returnByValue, string script, params object[] args)
            => ExecuteEvaluationAsync("Runtime.callFunctionOn", new RuntimeCallFunctionOnRequest
            {
                FunctionDeclaration = $"{script}\n{_evaluationScriptSuffix}\n",
                ExecutionContextId = _contextId,
                Arguments = args.Select(FormatArgument),
                ReturnByValue = returnByValue,
                AwaitPromise = true,
                UserGesture = true
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

        internal JSHandle CreateJSHandle(RemoteObject remoteObject)
            => remoteObject.Subtype == RemoteObjectSubtype.Node && Frame != null
                ? new ElementHandle(this, Client, remoteObject, Frame, Frame.FrameManager.Page, Frame.FrameManager)
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
                Value = arg
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

        internal async Task<ElementHandle> AdoptBackendNodeAsync(object backendNodeId)
        {
            var obj = await Client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = backendNodeId,
                ExecutionContextId = _contextId
            }).ConfigureAwait(false);

            return CreateJSHandle(obj.Object) as ElementHandle;
        }

        internal async Task<ElementHandle> AdoptElementHandleAsync(ElementHandle elementHandle)
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
                ObjectId = elementHandle.RemoteObject.ObjectId
            }).ConfigureAwait(false);

            var obj = await Client.SendAsync<DomResolveNodeResponse>("DOM.resolveNode", new DomResolveNodeRequest
            {
                BackendNodeId = nodeInfo.Node.BackendNodeId,
                ExecutionContextId = _contextId
            }).ConfigureAwait(false);

            return CreateJSHandle(obj.Object) as ElementHandle;
        }
    }
}
