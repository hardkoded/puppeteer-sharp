using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class JSHandle
    {
        private ExecutionContext _context;
        private Session _client;
        private dynamic _remoteObject;
        private bool _disposed = false;

        public JSHandle(ExecutionContext context, Session client, dynamic remoteObject)
        {
            _context = context;
            _client = client;
            _remoteObject = remoteObject;
        }

        public ExecutionContext ExecutionContext => _context;

        public async Task<Dictionary<string, object>> GetProperty(string propertyName)
        {
            dynamic response = await _client.Send("Runtime.getProperties", new Dictionary<string, object>()
            {
                {"objectId", _remoteObject.ObjectId},
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
            if(((IDictionary<string, object>)_remoteObject).ContainsKey("objectId"))
            {
                dynamic response = await _client.Send("Retunrime.callFunctionOn", new Dictionary<string, object>()
                {
                    {"functionDeclaration", "function() { return this; }"},
                    {"objectId", _remoteObject.objectId},
                    {"returnByValue", true},
                    {"awaitPromise", true}
                });
                return Helper.ValueFromRemoteObject(response.result);
            }

            return Helper.ValueFromRemoteObject(_remoteObject);
        }

        public ElementHandle AsElement() => null;

        public async Task Dispose()
        {
            if(_disposed)
            {
                return;
            }

            _disposed = true;
            await Helper.ReleaseObject(_client, _remoteObject);
        }

        public override string ToString()
        {
            if (((IDictionary<string, object>)_remoteObject).ContainsKey("objectId"))
            {
                var type = _remoteObject.subtype ?? _remoteObject.type;
                return "JSHandle@" + type;
            }

            return "JSHandle:" + Helper.ValueFromRemoteObject(_remoteObject);
        }
    }
}
