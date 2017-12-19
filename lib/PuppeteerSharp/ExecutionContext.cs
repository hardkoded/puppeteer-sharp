using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class ExecutionContext
    {
        private Session _client;
        private string _contextID;

        public ExecutionContext(Session client, string contextID, Func<JSHandle, dynamic> objectHandleFactory)
        {
            _client = client;
            _contextID = contextID;
            ObjectHandleFactory = objectHandleFactory;
        }

        public Func<JSHandle, dynamic> ObjectHandleFactory { get; internal set; }

        public async Task<dynamic> Evaluate(string pageFunction, params object[] args)
        {
            var handle = await EvaluateHandle(pageFunction, args);
            dynamic result = await handle.JsonValue();
            await handle.Dispose();
            return result;
        }

        private async Task<JSHandle> EvaluateHandle(string pageFunction, object[] args)
        {
            if(!string.IsNullOrEmpty(pageFunction)) 
            {
                dynamic remoteObject;

                try
                {
                    remoteObject = await _client.SendAsync("Runtime.evaluate", new Dictionary<string, object>()
                    {
                        {"expression", _contextID},
                        {"returnByValue", false},
                        {"awaitPromise", true}
                    });

                    return ObjectHandleFactory(remoteObject);
                }
                catch(Exception ex)
                {
                    throw new EvaluationFailedException("Evaluation Failed", ex);
                }
            }

            return null;
        }

        public async Task<dynamic> QueryObjects(JSHandle prototypeHandle)
        {
            if(prototypeHandle.Disposed)
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
