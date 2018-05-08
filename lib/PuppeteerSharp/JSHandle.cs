using Newtonsoft.Json.Linq;
using System;
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

        public Task<Dictionary<string, object>> GetProperty(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Dictionary<string, JSHandle>> GetPropertiesAsync()
        {
            var response = await _client.SendAsync("Runtime.getProperties", new
            {
                objectId = RemoteObject.objectId.ToString(),
                ownProperties = true
            });
            var result = new Dictionary<string, JSHandle>();
            foreach (var property in response.result)
            {
                if (property.enumerable == null)
                    continue;
                result.Add(property.name.ToString(), _context.ObjectHandleFactory(property.value));
            }
            return result;
        }

        public async Task<object> JsonValue() => await JsonValue<object>();

        public async Task<T> JsonValue<T>()
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
                return (T)Helper.ValueFromRemoteObject<T>(response.result);
            }

            return (T)Helper.ValueFromRemoteObject<T>(RemoteObject);
        }

        public virtual ElementHandle AsElement() => null;

        public async Task DisposeAsync()
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

            return Helper.ValueFromRemoteObject<object>(RemoteObject)?.ToString();
        }

        internal object FormatArgument()
        {
            if (objectHandle.ExecutionContext != this)
                throw new PuppeteerException("JSHandles can be evaluated only in the context they were created!");
            if (objectHandle.Disposed)
                throw new PuppeteerException("JSHandle is disposed!");
            if (objectHandle.RemoteObject.unserializableValue != null)
                return new { objectHandle.RemoteObject.unserializableValue };
            if (objectHandle.RemoteObject.objectId == null)
                return new { objectHandle.RemoteObject.value };
            return new { objectHandle.RemoteObject.objectId };
        }
    }
}
