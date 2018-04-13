using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class JSHandle
    {
        private ExecutionContext _context;
        protected readonly Session _client;

        public JSHandle(ExecutionContext context, Session client, object remoteObject)
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
            dynamic response = await _client.SendAsync("Runtime.getProperties", new Dictionary<string, object>()
            {
                {"objectId", RemoteObject.ObjectId},
                {"ownProperties", true}
            });
            var result = new Dictionary<string, object>();
            foreach (var property in response.result)
            {
                result.Add(property.name.ToString(), _context.ObjectHandleFactory(property.value));
            }

            return result;
        }

        public async Task<object> JsonValue()
        {
            if (RemoteObject.objectId != null)
            {
                dynamic response = await _client.SendAsync("Runtime.callFunctionOn", new Dictionary<string, object>()
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

        public virtual ElementHandle AsElement() => null;

        public async Task Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            await Helper.ReleaseObject(_client, RemoteObject);
        }

        public override string ToString()
        {
            if (((JObject)RemoteObject)["objectId"] != null)
            {
                var type = RemoteObject.subtype ?? RemoteObject.type;
                return "JSHandle@" + type;
            }

            return Helper.ValueFromRemoteObject(RemoteObject);
        }
    }
}
