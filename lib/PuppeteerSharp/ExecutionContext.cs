using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Helpers;

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

        private readonly string EvaluationScriptSuffix = $"//# sourceURL={EvaluationScriptUrl}";
        private static Regex _sourceUrlRegex = new Regex(@"^[\040\t]*\/\/[@#] sourceURL=\s*(\S*?)\s*$", RegexOptions.Multiline);
        private readonly CDPSession _client;
        private readonly int _contextId;

        internal ExecutionContext(
            CDPSession client,
            ContextPayload contextPayload,
            Frame frame)
        {
            _client = client;
            _contextId = contextPayload.Id;
            Frame = frame;
        }

        /// <summary>
        /// Frame associated with this execution context.
        /// </summary>
        /// <remarks>
        /// NOTE Not every execution context is associated with a frame. For example, workers and extensions have execution contexts that are not associated with frames.
        /// </remarks>
        public Frame Frame { get; }

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
        public Task<JToken> EvaluateExpressionAsync(string script)
            => EvaluateAsync<JToken>(EvaluateExpressionHandleAsync(script));

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
            => EvaluateAsync<T>(EvaluateExpressionHandleAsync(script));

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
        /// <seealso cref="EvaluateFunctionHandleAsync(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => EvaluateAsync<JToken>(EvaluateFunctionHandleAsync(script, args));

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
        /// <seealso cref="EvaluateFunctionHandleAsync(string, object[])"/>
        /// <returns>Task which resolves to script return value</returns>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
            => EvaluateAsync<T>(EvaluateFunctionHandleAsync(script, args));

        /// <summary>
        /// The method iterates JavaScript heap and finds all the objects with the given prototype.
        /// </summary>
        /// <returns>A task which resolves to a handle to an array of objects with this prototype.</returns>
        /// <param name="prototypeHandle">A handle to the object prototype.</param>
        public async Task<JSHandle> QueryObjectsAsync(JSHandle prototypeHandle)
        {
            if (prototypeHandle.Disposed)
            {
                throw new PuppeteerException("Prototype JSHandle is disposed!");
            }

            if (!((JObject)prototypeHandle.RemoteObject).TryGetValue(MessageKeys.ObjectId, out var objectId))
            {
                throw new PuppeteerException("Prototype JSHandle must not be referencing primitive value");
            }

            var response = await _client.SendAsync("Runtime.queryObjects", new Dictionary<string, object>
            {
                {"prototypeObjectId", objectId.ToString()}
            }).ConfigureAwait(false);

            return CreateJSHandle(response[MessageKeys.Objects]);
        }

        internal async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return null;
            }

            try
            {
                return await EvaluateHandleAsync("Runtime.evaluate", new Dictionary<string, object>
                {
                    ["expression"] = _sourceUrlRegex.IsMatch(script) ? script : $"{script}\n{EvaluationScriptSuffix}",
                    ["contextId"] = _contextId,
                    ["returnByValue"] = false,
                    ["awaitPromise"] = true,
                    ["userGesture"] = true
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new EvaluationFailedException(ex.Message, ex);
            }
        }

        internal async Task<JSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
        {
            if (string.IsNullOrEmpty(script))
            {
                return null;
            }

            try
            {
                return await EvaluateHandleAsync("Runtime.callFunctionOn", new Dictionary<string, object>
                {
                    ["functionDeclaration"] = $"{script}\n{EvaluationScriptSuffix}\n",
                    [MessageKeys.ExecutionContextId] = _contextId,
                    ["arguments"] = args.Select(FormatArgument),
                    ["returnByValue"] = false,
                    ["awaitPromise"] = true,
                    ["userGesture"] = true
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new EvaluationFailedException(ex.Message, ex);
            }
        }

        internal JSHandle CreateJSHandle(dynamic remoteObject)
            => (remoteObject.subtype == "node" && Frame != null)
                ? new ElementHandle(this, _client, remoteObject, Frame.FrameManager.Page, Frame.FrameManager)
                : new JSHandle(this, _client, remoteObject);

        private async Task<T> EvaluateAsync<T>(Task<JSHandle> handleEvaluator)
        {
            var handle = await handleEvaluator.ConfigureAwait(false);
            var result = default(T);

            try
            {
                result = await handle.JsonValueAsync<T>()
                    .ContinueWith(jsonTask => jsonTask.Exception != null ? default : jsonTask.Result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Object reference chain is too long") ||
                    ex.Message.Contains("Object couldn't be returned by value"))
                {
                    return default;
                }
                throw new EvaluationFailedException(ex.Message, ex);
            }
            await handle.DisposeAsync().ConfigureAwait(false);
            return result is JToken token && token.Type == JTokenType.Null ? default : result;
        }

        private async Task<JSHandle> EvaluateHandleAsync(string method, dynamic args)
        {
            var response = await _client.SendAsync(method, args).ConfigureAwait(false);

            if (response[MessageKeys.ExceptionDetails] is JToken exceptionDetails)
            {
                throw new EvaluationFailedException("Evaluation failed: " +
                    GetExceptionMessage(exceptionDetails.ToObject<EvaluateExceptionResponseDetails>(true)));
            }

            return CreateJSHandle(response.result);
        }

        private object FormatArgument(object arg)
        {
            switch (arg)
            {
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
            return new { value = arg };
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
    }
}