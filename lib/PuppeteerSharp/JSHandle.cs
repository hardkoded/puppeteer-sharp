using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class JSHandle
    {
        private ExecutionContext _context;
        protected readonly CDPSession _client;
        protected readonly ILogger _logger;

        public JSHandle(ExecutionContext context, CDPSession client, object remoteObject)
        {
            _context = context;
            _client = client;
            _logger = _client.Connection.LoggerFactory.CreateLogger(this.GetType());
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

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}"/> with property names as keys and <see cref="JSHandle"/> instances for the property values.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Dictionary{TKey, TValue}"/></returns>
        /// <example>
        /// <code>
        /// var handle = await page.EvaluateExpressionHandle("({window, document})");
        /// var properties = await handle.GetPropertiesAsync();
        /// var windowHandle = properties["window"];
        /// var documentHandle = properties["document"];
        /// await handle.DisposeAsync();
        /// </code>
        /// </example>
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

        /// <summary>
        /// Returns a JSON representation of the object
        /// </summary>
        /// <returns>Task</returns>
        /// <remarks>
        /// The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references
        /// </remarks>
        public async Task<object> JsonValueAsync() => await JsonValueAsync<object>();

        /// <summary>
        /// Returns a JSON representation of the object
        /// </summary>
        /// <typeparam name="T">A strongly typed object to parse to</typeparam>
        /// <returns>Task</returns>
        /// <remarks>
        /// The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references
        /// </remarks>
        public async Task<T> JsonValueAsync<T>()
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
                return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(response.result);
            }

            return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(RemoteObject);
        }

        public async Task DisposeAsync()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            await RemoteObjectHelper.ReleaseObject(_client, RemoteObject, _logger);
        }

        public override string ToString()
        {
            if (((JObject)RemoteObject)["objectId"] != null)
            {
                var type = RemoteObject.subtype ?? RemoteObject.type;
                return "JSHandle@" + type;
            }

            return "JSHandle:" + RemoteObjectHelper.ValueFromRemoteObject<object>(RemoteObject)?.ToString();
        }

        internal object FormatArgument(ExecutionContext context)
        {
            if (ExecutionContext != context)
                throw new PuppeteerException("JSHandles can be evaluated only in the context they were created!");
            if (Disposed)
                throw new PuppeteerException("JSHandle is disposed!");
            if (RemoteObject.unserializableValue != null)
                return new { RemoteObject.unserializableValue };
            if (RemoteObject.objectId == null)
                return new { RemoteObject.value };
            return new { RemoteObject.objectId };
        }
    }
}
