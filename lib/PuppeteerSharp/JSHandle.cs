using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class JSHandle : IJSHandle
    {
        private bool _moved;

        internal JSHandle()
        {
        }

        /// <inheritdoc/>
        public bool Disposed { get; protected set; }

        internal Func<Task> DisposeAction { get; set; }

        internal abstract Realm Realm { get; }

        internal Frame Frame => Realm.Environment as Frame;

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
                    return Object.keys(object ?? {});
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
        public abstract Task<T> JsonValueAsync<T>();

        /// <inheritdoc/>
        public IJSHandle Move()
        {
            _moved = true;
            return this;
        }

        /// <inheritdoc/>
        public abstract ValueTask DisposeAsync();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            return Realm.EvaluateFunctionHandleAsync(pageFunction, [this, .. args]);
        }

        /// <inheritdoc/>
        public async Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
        {
            var adoptedThis = await Realm.AdoptHandleAsync(this).ConfigureAwait(false);
            return await Realm.EvaluateFunctionAsync<JsonElement?>(script, [adoptedThis, .. args])
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            return Realm.EvaluateFunctionAsync<T>(script, [this, .. args]);
        }

        /// <summary>
        /// Disposes the handle. By default, disposal is delegated to <see cref="DisposeAsync"/> without
        /// waiting for it to complete; any exception raised is swallowed, mirroring the fire-and-forget
        /// nature of a synchronous <c>Dispose</c> call over an asynchronous resource.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ = DisposeAsync().AsTask();
            }
        }

        /// <summary>
        /// Checks if the handle has been moved and should skip disposal.
        /// If moved, resets the flag and returns <c>true</c>.
        /// </summary>
        /// <returns><c>true</c> if disposal should be skipped.</returns>
        protected bool CheckAndResetMoved()
        {
            if (_moved)
            {
                _moved = false;
                return true;
            }

            return false;
        }
    }
}
