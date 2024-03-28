using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

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
        public bool IsIncognito => Id != null;

        /// <inheritdoc/>
        public bool IsClosed => Browser.BrowserContexts().Contains(this);

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
        public abstract Task<IPage> NewPageAsync();

        /// <inheritdoc/>
        public abstract Task CloseAsync();

        /// <inheritdoc/>
        public abstract Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions);

        /// <inheritdoc/>
        public abstract Task ClearPermissionOverridesAsync();

        internal void OnTargetCreated(Browser browser, TargetChangedArgs args) => TargetCreated?.Invoke(browser, args);

        internal void OnTargetDestroyed(Browser browser, TargetChangedArgs args) => TargetDestroyed?.Invoke(browser, args);

        internal void OnTargetChanged(Browser browser, TargetChangedArgs args) => TargetChanged?.Invoke(browser, args);
    }
}
