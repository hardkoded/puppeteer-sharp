using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class BrowserContext : IBrowserContext
    {
        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <inheritdoc/>
        public string Id { get; protected init; }

        /// <inheritdoc/>
        public bool IsClosed => !Browser.BrowserContexts().Contains(this);

        /// <inheritdoc cref="Browser"/>
        public Browser Browser { get; protected init; }

        /// <inheritdoc/>
        IBrowser IBrowserContext.Browser => Browser;

        /// <inheritdoc/>
        public abstract ITarget[] Targets();

        /// <inheritdoc/>
        public Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
            => Browser.WaitForTargetAsync((target) => target.BrowserContext == this && predicate(target), options);

        /// <inheritdoc />
        public abstract Task<IPage[]> PagesAsync();

        /// <inheritdoc/>
        public abstract Task<IPage> NewPageAsync(CreatePageOptions options = null);

        /// <inheritdoc/>
        public abstract Task CloseAsync();

        /// <inheritdoc/>
        public abstract Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions);

        /// <inheritdoc/>
        public abstract Task ClearPermissionOverridesAsync();

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes the browser context. All the targets that belong to the browser context will be closed.
        /// </summary>
        /// <returns>ValueTask.</returns>
        public async ValueTask DisposeAsync()
        {
            await CloseAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        internal void OnTargetCreated(TargetChangedArgs args) => TargetCreated?.Invoke(this, args);

        internal void OnTargetDestroyed(TargetChangedArgs args) => TargetDestroyed?.Invoke(this, args);

        internal void OnTargetChanged(TargetChangedArgs args) => TargetChanged?.Invoke(this, args);

        /// <summary>
        /// Closes the browser context. All the targets that belong to the browser context will be closed.
        /// </summary>
        /// <param name="disposing">Indicates whether disposal was initiated by <see cref="Dispose()"/> operation.</param>
        protected virtual void Dispose(bool disposing) => _ = CloseAsync();
    }
}
