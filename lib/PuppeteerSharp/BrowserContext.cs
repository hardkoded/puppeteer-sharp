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
        public abstract Task SetPermissionAsync(string origin, params PermissionEntry[] permissions);

        /// <inheritdoc/>
        public abstract Task ClearPermissionOverridesAsync();

        /// <inheritdoc/>
        public abstract Task<CookieParam[]> GetCookiesAsync();

        /// <inheritdoc/>
        public abstract Task SetCookieAsync(params CookieData[] cookies);

        /// <inheritdoc/>
        public async Task DeleteCookieAsync(params CookieParam[] cookies)
        {
            await SetCookieAsync(
                cookies.Select(cookie => new CookieData
                {
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Secure = cookie.Secure,
                    HttpOnly = cookie.HttpOnly,
                    SameSite = cookie.SameSite,
                    Expires = 1,
                    Priority = cookie.Priority,
                    SourceScheme = cookie.SourceScheme,
                    PartitionKey = cookie.PartitionKey,
                }).ToArray()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DeleteMatchingCookiesAsync(params DeleteCookiesRequest[] filters)
        {
            var cookies = await GetCookiesAsync().ConfigureAwait(false);
            var cookiesToDelete = cookies.Where(cookie =>
                filters.Any(filter =>
                {
                    if (filter.Name != cookie.Name)
                    {
                        return false;
                    }

                    if (filter.Domain != null && filter.Domain == cookie.Domain)
                    {
                        return true;
                    }

                    if (filter.Path != null && filter.Path == cookie.Path)
                    {
                        return true;
                    }

                    if (filter.PartitionKey != null && cookie.PartitionKey != null)
                    {
                        if (filter.PartitionKey.SourceOrigin == cookie.PartitionKey.SourceOrigin)
                        {
                            return true;
                        }
                    }

                    if (filter.Url != null)
                    {
                        if (Uri.TryCreate(filter.Url, UriKind.Absolute, out var url))
                        {
                            if (url.Host == cookie.Domain && url.AbsolutePath == cookie.Path)
                            {
                                return true;
                            }
                        }
                    }

                    return true;
                })).ToArray();
            await DeleteCookieAsync(cookiesToDelete).ConfigureAwait(false);
        }

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
