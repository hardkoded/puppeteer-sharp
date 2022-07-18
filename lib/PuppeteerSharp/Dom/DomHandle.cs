using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <inheritdoc />
    public abstract class DomHandle : IDomHandle
    {
        /// <inheritdoc />
        public string ClassName { get; private set; }

        /// <inheritdoc />
        public JSHandle Handle { get; private set; }

        /// <inheritdoc />
        public bool IsDisposed
        {
            get { return Handle.Disposed; }
        }

        internal DomHandle(string className, JSHandle jSHandle)
        {
            ClassName = className;
            Handle = jSHandle;
        }

        /// <summary>
        /// Disposes the underlying <see cref="Handle"/>.
        /// </summary>
        /// <returns>The async.</returns>
        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);

            await Handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal async Task<T> EvaluateFunctionInternalAsync<T>(string script, params object[] args)
        {
            var list = new List<object>(args.Length);

            // TODO: If https://github.com/hardkoded/puppeteer-sharp/issues/1925
            // is implemented hopefully a IJSHandle interface is added and we can implement that
            // directly

            var type = typeof(DomHandle);

            foreach (var arg in args)
            {
                if (arg != null && type.IsAssignableFrom(arg.GetType()))
                {
                    var handle = (DomHandle)arg;
                    list.Add(handle.Handle);
                }
                else
                {
                    list.Add(arg);
                }
            }

            var returnType = typeof(T);

            if (returnType.IsEnum)
            {
                var result = await Handle.EvaluateFunctionAsync(script, list.ToArray()).ConfigureAwait(false);

                var typeConverter = TypeDescriptor.GetConverter(returnType);

                return (T)typeConverter.ConvertFrom(result.ToString());
            }

            return await Handle.EvaluateFunctionAsync<T>(script, list.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal Task EvaluateFunctionInternalAsync(string script, params object[] args)
        {
            var list = new List<object>(args.Length);

            // TODO: If https://github.com/hardkoded/puppeteer-sharp/issues/1925
            // is implemented hopefully a IJSHandle interface is added and we can implement that
            // directly

            var type = typeof(DomHandle);

            foreach (var arg in args)
            {
                if (arg != null && type.IsAssignableFrom(arg.GetType()))
                {
                    var handle = (DomHandle)arg;
                    list.Add(handle.Handle);
                }
                else
                {
                    list.Add(arg);
                }
            }

            return Handle.EvaluateFunctionAsync(script, list.ToArray());
        }

        internal async Task<T> EvaluateFunctionHandleInternalAsync<T>(string script, params object[] args)
            where T : DomHandle
        {
            var handle = await Handle.EvaluateFunctionHandleAsync(script, args).ConfigureAwait(false);

            if (handle == null)
            {
                return default;
            }

            return handle.ToDomHandle<T>();
        }

        internal async Task<IEnumerable<T>> GetArray<T>()
            where T : DomHandle
        {
            var properties = await Handle.GetPropertiesAsync().ConfigureAwait(false);
            var result = new List<T>();

            foreach (var jSHandle in properties.Values)
            {
                if (jSHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                var obj = jSHandle.ToDomHandle<T>();

                result.Add(obj);
            }
            return result;
        }

        internal async Task<IEnumerable<string>> GetStringArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<string>();

            foreach (var jsHandle in response.Values)
            {
                if (jsHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                result.Add(jsHandle.RemoteObject.Value.ToString());
            }
            return result;
        }

        internal async Task<IEnumerable<KeyValuePair<string, string>>> GetStringMapArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<KeyValuePair<string, string>>();

            foreach (var kvp in response)
            {
                var jsHandle = kvp.Value;

                if (jsHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                result.Add(new KeyValuePair<string, string>(kvp.Key, jsHandle.RemoteObject.Value.ToString()));

                await jsHandle.DisposeAsync().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Implicit operator, convert <see cref="DomHandle"/> to <see cref="JSHandle"/>
        /// </summary>
        /// <param name="domHandle">DomHandle</param>
        public static implicit operator JSHandle(DomHandle domHandle)
        {
            if (domHandle == null)
            {
                return null;
            }
            return domHandle.Handle;
        }
    }
}
