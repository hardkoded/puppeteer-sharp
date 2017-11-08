using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class JSHandle
    {
        private ExecutionContext _context;
        private Session _client;
        public JSHandle(ExecutionContext context, Session client, dynamic remoteObject)
        {
            _context = context;
            _client = client;
            RemoteObject = remoteObject;
        }

        public ExecutionContext ExecutionContext => _context;
        public bool Disposed { get; set; }
        public dynamic RemoteObject { get; internal set; }

        public async Task<Dictionary<string, object>> GetProperty(string propertyName)
        {
            dynamic response = await _client.Send("Runtime.getProperties", new Dictionary<string, object>()
            {
                {"objectId", RemoteObject.ObjectId},
                {"ownProperties", true}
            });
            var result = new Dictionary<string, object>();
            foreach(var property in response.result)
            {
                result.Add(property.name.ToString(), _context.ObjectHandleFactory(property.value));
            }

            return result;
        }

        public async Task<object> JsonValue()
        {
            if(((IDictionary<string, object>)RemoteObject).ContainsKey("objectId"))
            {
                dynamic response = await _client.Send("Retunrime.callFunctionOn", new Dictionary<string, object>()
                {
                    {"functionDeclaration", "function() { return this; }"},
                    {"objectId", RemoteObject.objectId},
                    {"returnByValue", true},
                    {"awaitPromise", true}
                });
                return Helper.ValueFromRemoteObject(response.result);
            }

            return Helper.ValueFromRemoteObject(RemoteObject);
        }

        public ElementHandle AsElement() => null;

        public async Task Dispose()
        {
            if(Disposed)
            {
                return;
            }

            Disposed = true;
            await Helper.ReleaseObject(_client, RemoteObject);
        }

        public override string ToString()
        {
            if (((IDictionary<string, object>)RemoteObject).ContainsKey("objectId"))
            {
                var type = RemoteObject.subtype ?? RemoteObject.type;
                return "JSHandle@" + type;
            }

            return "JSHandle:" + Helper.ValueFromRemoteObject(RemoteObject);
        }
    }
}
