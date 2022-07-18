using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers;
using CefSharp.DevTools.Dom.Helpers.Json;
using CefSharp.DevTools.Dom.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// JSHandle represents an in-page JavaScript object. JSHandles can be created with the <see cref="IDevToolsContext.EvaluateExpressionHandleAsync(string)"/> and <see cref="IDevToolsContext.EvaluateFunctionHandleAsync(string, object[])"/> methods.
    /// </summary>
    [JsonConverter(typeof(JSHandleMethodConverter))]
    public class JSHandle
    {
        internal JSHandle(ExecutionContext context, DevToolsConnection client, RemoteObject remoteObject)
        {
            ExecutionContext = context;
            Connection = client;
            Logger = Connection.LoggerFactory.CreateLogger(GetType());
            RemoteObject = remoteObject;
        }

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        /// <value>The execution context.</value>
        public ExecutionContext ExecutionContext { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="JSHandle"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed { get; private set; }
        /// <summary>
        /// Gets or sets the remote object.
        /// </summary>
        /// <value>The remote object.</value>
        public RemoteObject RemoteObject { get; }
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        protected DevToolsConnection Connection { get; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected ILogger Logger { get; }

        /// <summary>
        /// Fetches a single property from the referenced object
        /// </summary>
        /// <param name="propertyName">property to get</param>
        /// <returns>Task of <see cref="JSHandle"/></returns>
        public async Task<JSHandle> GetPropertyAsync(string propertyName)
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

        /// <summary>
        /// Gets the property value for a single property
        /// </summary>
        /// <typeparam name="T">Property Value Type e.g. string, int</typeparam>
        /// <param name="propertyName">property to get</param>
        /// <returns>Task of <typeparamref name="T"/></returns>
        /// <exception cref="PuppeteerException">Thrown if no matching property is found</exception>
        public async Task<T> GetPropertyValueAsync<T>(string propertyName)
        {
            var property = await GetPropertyAsync(propertyName).ConfigureAwait(false);

            if (property.RemoteObject.Type == RemoteObjectType.Undefined)
            {
                throw new PuppeteerException($"Property {propertyName} was not found. Property names are case sensitive e.g. innerText not innertext. Make sure to check your spelling.");
            }

            return await property.JsonValueAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the corrosponding value of the object
        /// </summary>
        /// <typeparam name="T">A strongly typed object to parse to</typeparam>
        /// <returns>Task</returns>
        /// <remarks>
        /// The method will return default(T) if the referenced object is not stringifiable. It will throw an error if the object has circular references
        /// </remarks>
        public async Task<T> GetValueAsync<T>()
        {
            var objectId = RemoteObject.ObjectId;

            if (objectId != null)
            {
                var response = await Connection.SendAsync<RuntimeCallFunctionOnResponse>("Runtime.callFunctionOn", new RuntimeCallFunctionOnRequest
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
        /// Returns a <see cref="Dictionary{TKey, TValue}"/> with property names as keys and <see cref="JSHandle"/> instances for the property values.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Dictionary{TKey, TValue}"/></returns>
        /// <example>
        /// <code>
        /// var handle = await devToolsContext.EvaluateExpressionHandle("({window, document})");
        /// var properties = await handle.GetPropertiesAsync();
        /// var windowHandle = properties["window"];
        /// var documentHandle = properties["document"];
        /// await handle.DisposeAsync();
        /// </code>
        /// </example>
        public async Task<Dictionary<string, JSHandle>> GetPropertiesAsync()
        {
            var response = await Connection.SendAsync<RuntimeGetPropertiesResponse>("Runtime.getProperties", new RuntimeGetPropertiesRequest
            {
                ObjectId = RemoteObject.ObjectId,
                OwnProperties = true
            }).ConfigureAwait(false);

            var result = new Dictionary<string, JSHandle>();

            foreach (var property in response.Result)
            {
                if (property.Enumerable == null)
                {
                    continue;
                }

                result.Add(property.Name, ExecutionContext.CreateJSHandle(property.Value));
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
        public async Task<object> JsonValueAsync() => await JsonValueAsync<object>().ConfigureAwait(false);

        /// <summary>
        /// Returns a JSON representation of the object
        /// </summary>
        /// <typeparam name="T">A strongly typed object to parse to</typeparam>
        /// <returns>Task</returns>
        /// <remarks>
        /// The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references
        /// </remarks>
        public Task<T> JsonValueAsync<T>()
        {
            return GetValueAsync<T>();
        }

        /// <summary>
        /// Disposes the Handle. It will mark the JSHandle as disposed and release the <see cref="JSHandle.RemoteObject"/>
        /// </summary>
        /// <returns>The async.</returns>
        public async Task DisposeAsync()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            await RemoteObjectHelper.ReleaseObjectAsync(Connection, RemoteObject, Logger).ConfigureAwait(false);
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

        /// <summary>
        /// Executes a script in browser context
        /// </summary>
        /// <param name="pageFunction">Script to be evaluated in browser context</param>
        /// <param name="args">Function arguments</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return ExecutionContext.EvaluateFunctionHandleAsync(pageFunction, list.ToArray());
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return ExecutionContext.EvaluateFunctionAsync<JToken>(script, list.ToArray());
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="JSHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var list = new List<object>(args);
            list.Insert(0, this);
            return ExecutionContext.EvaluateFunctionAsync<T>(script, list.ToArray());
        }

        /// <summary>
        /// Creates a Html Element from the <see cref="JSHandle"/>
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Element or null</returns>
        public T ToDomHandle<T>()
            where T : DomHandle
        {
            var remoteObject = RemoteObject;

            // Types like FileList are of SubType other
            if (remoteObject.Type == Messaging.RemoteObjectType.Object &&
                (remoteObject.Subtype == Messaging.RemoteObjectSubtype.Node ||
                remoteObject.Subtype == Messaging.RemoteObjectSubtype.Array ||
                remoteObject.Subtype == Messaging.RemoteObjectSubtype.Other))
            {
                return HtmlObjectFactory.CreateObject<T>(remoteObject.ClassName, this);
            }

            return default;
        }

        internal object FormatArgument(ExecutionContext context)
        {
            if (ExecutionContext.Id != context.Id)
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
