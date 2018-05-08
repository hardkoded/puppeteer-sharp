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

        /// <summary>
        /// Fetches a single property from the referenced object
        /// </summary>
        /// <param name="propertyName">property to get</param>
        /// <returns>Task of <see cref="JSHandle"/></returns>
        public async Task<JSHandle> GetPropertyAsync(string propertyName)
        {
            var objectHandle = await ExecutionContext.EvaluateFunctionHandleAsync(@"(object, propertyName) => {
              const result = { __proto__: null};
              result[propertyName] = object[propertyName];
              return result;
            }", this, propertyName);
            var properties = await objectHandle.GetPropertiesAsync();
            properties.TryGetValue(propertyName, out var result);
            await objectHandle.DisposeAsync();
            return result;
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

            return "JSHandle:" + Helper.ValueFromRemoteObject<object>(RemoteObject)?.ToString();
        }
    }
}
