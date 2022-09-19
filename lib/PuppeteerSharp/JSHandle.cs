using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    [JsonConverter(typeof(JSHandleMethodConverter))]
    public class JSHandle : IJSHandle
    {
        private readonly IExecutionContext _executionContext;

        internal JSHandle(IExecutionContext context, CDPSession client, RemoteObject remoteObject)
        {
            _executionContext = context;
            Client = client;
            Logger = Client.Connection.LoggerFactory.CreateLogger(GetType());
            RemoteObject = remoteObject;
        }

        /// <inheritdoc/>
        public IExecutionContext ExecutionContext => _executionContext;
        /// <inheritdoc/>
        public bool Disposed { get; private set; }
        /// <inheritdoc/>
        public RemoteObject RemoteObject { get; }
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        protected CDPSession Client { get; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task<IJSHandle> GetPropertyAsync(string propertyName)
        {
            var objectHandle = await EvaluateFunctionHandleAsync(
                @"(object, propertyName) => {
                    const result = { __proto__: null};
                    result[propertyName] = object[propertyName];
                    return result;
                }",
                propertyName).ConfigureAwait(false);
            var properties = await objectHandle.GetPropertiesAsync().ConfigureAwait(false);
            properties.TryGetValue(propertyName, out var result);
            await objectHandle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
        {
            var response = await Client.SendAsync<RuntimeGetPropertiesResponse>("Runtime.getProperties", new RuntimeGetPropertiesRequest
            {
                ObjectId = RemoteObject.ObjectId,
                OwnProperties = true
            }).ConfigureAwait(false);

            var result = new Dictionary<string, IJSHandle>();

            foreach (var property in response.Result)
            {
                if (property.Enumerable == null)
                {
                    continue;
                }

                result.Add(property.Name, ((ExecutionContext)_executionContext).CreateJSHandle(property.Value));
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<object> JsonValueAsync() => await JsonValueAsync<object>().ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task<T> JsonValueAsync<T>()
        {
            var objectId = RemoteObject.ObjectId;

            if (objectId != null)
            {
                var response = await Client.SendAsync<RuntimeCallFunctionOnResponse>("Runtime.callFunctionOn", new RuntimeCallFunctionOnRequest
                {
                    FunctionDeclaration = "function() { return this; }",
                    ObjectId = objectId,
                    ReturnByValue = true,
                    AwaitPromise = true
                }).ConfigureAwait(false);
                return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(response.Result);
            }

            return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(RemoteObject);
        }

        /// <summary>
        /// Disposes the Handle. It will mark the JSHandle as disposed and release the <see cref="IJSHandle.RemoteObject"/>
        /// </summary>
        /// <returns>The async.</returns>
        public async ValueTask DisposeAsync()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            await RemoteObjectHelper.ReleaseObjectAsync(Client, RemoteObject, Logger).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (RemoteObject.ObjectId != null)
            {
                var type = RemoteObject.Subtype != RemoteObjectSubtype.Other
                    ? RemoteObject.Subtype.ToString()
                    : RemoteObject.Type.ToString();
                return "JSHandle@" + type.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            }

            return "JSHandle:" + RemoteObjectHelper.ValueFromRemoteObject<object>(RemoteObject, true)?.ToString();
        }

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return _executionContext.EvaluateFunctionHandleAsync(pageFunction, list.ToArray());
        }

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return _executionContext.EvaluateFunctionAsync<JToken>(script, list.ToArray());
        }

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return _executionContext.EvaluateFunctionAsync<T>(script, list.ToArray());
        }

        internal object FormatArgument(ExecutionContext context)
        {
            if (_executionContext != context)
            {
                throw new PuppeteerException("JSHandles can be evaluated only in the context they were created!");
            }

            if (Disposed)
            {
                throw new PuppeteerException("JSHandle is disposed!");
            }

            var unserializableValue = RemoteObject.UnserializableValue;

            if (unserializableValue != null)
            {
                return new { unserializableValue };
            }

            if (RemoteObject.ObjectId == null)
            {
                return new { RemoteObject.Value };
            }

            var objectId = RemoteObject.ObjectId;

            return new { objectId };
        }
    }
}
