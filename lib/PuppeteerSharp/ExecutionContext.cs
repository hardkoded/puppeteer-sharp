using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    public class ExecutionContext
    {
        private readonly Session _client;
        private readonly int _contextId;

        public ExecutionContext(Session client, ContextPayload contextPayload, Func<dynamic, JSHandle> objectHandleFactory)
        {
            _client = client;
            _contextId = contextPayload.Id;
            FrameId = contextPayload.AuxData.FrameId;
            IsDefault = contextPayload.AuxData.IsDefault;
            ObjectHandleFactory = objectHandleFactory;
        }

        public Func<dynamic, JSHandle> ObjectHandleFactory { get; internal set; }
        public string FrameId { get; internal set; }
        public bool IsDefault { get; internal set; }

        public async Task<dynamic> EvaluateExpressionAsync(string script)
        {
            var handle = await EvaluateExpressionHandleAsync(script);
            dynamic result = await handle.JsonValue();
            await handle.Dispose();
            return result;
        }

        public async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var result = await EvaluateExpressionAsync(script);
            return ((JToken)result).ToObject<T>();
        }

        public async Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
        {
            var handle = await EvaluateFunctionHandleAsync(script, args);
            dynamic result = await handle.JsonValue();
            await handle.Dispose();
            return result;
        }

        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var result = await EvaluateFunctionAsync(script, args);
            return ((JToken)result).ToObject<T>();
        }

        internal async Task<JSHandle> EvaluateExpressionHandleAsync(string script)
        {
            if (string.IsNullOrEmpty(script))
            {
                return null;
            }

            dynamic remoteObject;

            try
            {
                remoteObject = await _client.SendAsync("Runtime.evaluate", new Dictionary<string, object>()
                {
                    {"contextId", _contextId},
                    {"expression", script},
                    {"returnByValue", false},
                    {"awaitPromise", true}
                });

                return ObjectHandleFactory(remoteObject.result);
            }
            catch (Exception ex)
            {
                throw new EvaluationFailedException("Evaluation Failed", ex);
            }
        }

        internal async Task<JSHandle> EvaluateFunctionHandleAsync(string script, object[] args)
        {
            if (string.IsNullOrEmpty(script))
            {
                return null;
            }

            dynamic result = await _client.SendAsync("Runtime.callFunctionOn", new Dictionary<string, object>()
                {
                    {"functionDeclaration", script },
                    {"executionContextId", _contextId},
                    {"arguments", FormatArguments(args)},
                    {"returnByValue", false},
                    {"awaitPromise", true}
                });

            if (result.exceptionDetails != null)
            {
                throw new EvaluationFailedException("Evaluation failed: " +
                    Helper.GetExceptionMessage(result.exceptionDetails.ToObject<EvaluateExceptionDetails>()));
            }

            return ObjectHandleFactory(result.result);
        }

        private object FormatArguments(object[] args)
        {
            return args.Select(o => new { value = o });
        }

        private bool IsFunction(string script)
        {
            return script.Contains("=>");
        }

        public async Task<dynamic> QueryObjects(JSHandle prototypeHandle)
        {
            if (prototypeHandle.Disposed)
            {
                throw new ArgumentException("prototypeHandle is disposed", nameof(prototypeHandle));
            }

            if (!((IDictionary<string, object>)prototypeHandle.RemoteObject).ContainsKey("objectId"))
            {
                throw new ArgumentException("Prototype JSHandle must not be referencing primitive value",
                                            nameof(prototypeHandle));
            }

            dynamic response = await _client.SendAsync("Runtime.queryObjects", new Dictionary<string, object>()
            {
                {"prototypeObjectId", prototypeHandle.RemoteObject.objectId}
            });

            return ObjectHandleFactory(response.objects);
        }

    }
}
