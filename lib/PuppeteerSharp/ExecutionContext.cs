using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<T> EvaluateAsync<T>(string script, params object[] args)
        {
            var handle = await EvaluateHandleAsync(script, args);
            dynamic result = await handle.JsonValue();
            await handle.Dispose();
            return result.ToObject<T>();
        }

        internal async Task<JSHandle> EvaluateHandleAsync(string script, object[] args)
        {
            if (string.IsNullOrEmpty(script))
            {
                return null;
            }

            if (IsFunction(script))
            {
                dynamic result = await _client.SendAsync("Runtime.callFunctionOn", new Dictionary<string, object>()
                {
                    {"functionDeclaration", script },
                    {"executionContextId", _contextId},
                    {"arguments", args},
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
