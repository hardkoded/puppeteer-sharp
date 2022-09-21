using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public class BrowserContext : IBrowserContext
    {
        private readonly Connection _connection;
        private readonly string _id;

        internal BrowserContext(Connection connection, Browser browser, string contextId)
        {
            _connection = connection;
            Browser = browser;
            _id = contextId;
        }

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <inheritdoc/>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <inheritdoc/>
        public bool IsIncognito => _id != null;

        /// <inheritdoc/>
        public Browser Browser { get; }

        /// <inheritdoc/>
        IBrowser IBrowserContext.Browser => Browser;

        /// <inheritdoc/>
        public ITarget[] Targets() => Array.FindAll(Browser.Targets(), target => target.BrowserContext == this);

        /// <inheritdoc/>
        public Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
            => Browser.WaitForTargetAsync((target) => target.BrowserContext == this && predicate(target), options);

        /// <inheritdoc/>
        public async Task<IPage[]> PagesAsync()
        => (await Task.WhenAll(
            Targets().Where(t => t.Type == TargetType.Page).Select(t => t.PageAsync())).ConfigureAwait(false))
            .Where(p => p != null).ToArray();

        /// <inheritdoc/>
        public Task<IPage> NewPageAsync() => Browser.CreatePageInContextAsync(_id);

        /// <inheritdoc/>
        public Task CloseAsync()
        {
            if (_id == null)
            {
                throw new PuppeteerException("Non-incognito profiles cannot be closed!");
            }

            return Browser.DisposeContextAsync(_id);
        }

        /// <inheritdoc/>
        public Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
            => _connection.SendAsync("Browser.grantPermissions", new BrowserGrantPermissionsRequest
            {
                Origin = origin,
                BrowserContextId = _id,
                Permissions = permissions.ToArray()
            });

        /// <inheritdoc/>
        public Task ClearPermissionOverridesAsync()
            => _connection.SendAsync("Browser.resetPermissions", new BrowserResetPermissionsRequest
            {
                BrowserContextId = _id
            });

        internal void OnTargetCreated(Browser browser, TargetChangedArgs args) => TargetCreated?.Invoke(browser, args);

        internal void OnTargetDestroyed(Browser browser, TargetChangedArgs args) => TargetDestroyed?.Invoke(browser, args);

        internal void OnTargetChanged(Browser browser, TargetChangedArgs args) => TargetChanged?.Invoke(browser, args);
    }
}
