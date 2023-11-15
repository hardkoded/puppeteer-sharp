using System;
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
        internal JSHandle(IsolatedWorld world, RemoteObject remoteObject)
        {
            Realm = world;
            RemoteObject = remoteObject;
            Logger = Client.Connection.LoggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc/>
        public bool Disposed { get; private set; }

        /// <inheritdoc/>
        public RemoteObject RemoteObject { get; }

        internal CDPSession Client => Realm.Environment.Client;

        internal Func<Task> DisposeAction { get; set; }

        internal IsolatedWorld Realm { get; }

        internal Frame Frame => Realm.Environment as Frame;

        internal string Id => RemoteObject.ObjectId;

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public virtual Task<IJSHandle> GetPropertyAsync(string propertyName)
            => EvaluateFunctionHandleAsync(
                @"(object, propertyName) => {
                        return object[propertyName];
                    }",
                propertyName);

        /// <inheritdoc/>
        public virtual async Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
        {
            var propertyNames = await EvaluateFunctionAsync<string[]>(@"object => {
                    const enumerableProperties = [];
                    const descriptors = Object.getOwnPropertyDescriptors(object);
                    for (const propertyName in descriptors) {
                        if (descriptors[propertyName]?.enumerable)
                        {
                            enumerableProperties.push(propertyName);
                        }
                    }
                    return enumerableProperties;
                }").ConfigureAwait(false);

            var dic = new Dictionary<string, IJSHandle>();

            foreach (var key in propertyNames)
            {
                var handleItem = await GetPropertyAsync(key).ConfigureAwait(false);
                if (handleItem is not null)
                {
                    dic.Add(key, handleItem);
                }
            }

            return dic;
        }

        /// <inheritdoc/>
        public async Task<object> JsonValueAsync() => await JsonValueAsync<object>().ConfigureAwait(false);

        /// <inheritdoc/>
        public virtual async Task<T> JsonValueAsync<T>()
        {
            var objectId = RemoteObject.ObjectId;

            if (objectId == null)
            {
                return (T)RemoteObjectHelper.ValueFromRemoteObject<T>(RemoteObject);
            }

            var value = await EvaluateFunctionAsync<T>("object => object").ConfigureAwait(false);

            return value == null ? throw new PuppeteerException("Could not serialize referenced object") : value;
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;

            if (DisposeAction != null)
            {
                await DisposeAction().ConfigureAwait(false);
            }

            await RemoteObjectHelper.ReleaseObjectAsync(Client, RemoteObject, Logger).ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (RemoteObject.ObjectId == null)
            {
                return "JSHandle:" + RemoteObjectHelper.ValueFromRemoteObject<object>(RemoteObject, true);
            }

            var type = RemoteObject.Subtype != RemoteObjectSubtype.Other
                ? RemoteObject.Subtype.ToString()
                : RemoteObject.Type.ToString();
            return "JSHandle@" + type.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return Realm.EvaluateFunctionHandleAsync(pageFunction, list.ToArray());
        }

        /// <inheritdoc/>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
            => EvaluateFunctionAsync(script, args, false);

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return Realm.EvaluateFunctionAsync<T>(script, list.ToArray());
        }

        internal async Task<JToken> EvaluateFunctionAsync(string script, object[] args, bool adopt)
        {
            var list = new List<object>(args);
            var adoptedThis = await Frame.IsolatedRealm.AdoptHandleAsync(this).ConfigureAwait(false);
            list.Insert(0, adoptedThis);
            return await Frame.IsolatedRealm.EvaluateFunctionAsync<JToken>(script, list.ToArray())
                .ConfigureAwait(false);
        }
    }
}
