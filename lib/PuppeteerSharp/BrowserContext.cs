using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has
    /// a single <see cref="BrowserContext"/> used by default. The method <see cref="Browser.NewPageAsync"/> creates a <see cref="Page"/> in the default <see cref="BrowserContext"/>
    /// </summary>
    public class BrowserContext
    {
        private readonly IConnection _connection;
        private readonly string _id;

        internal BrowserContext(IConnection connection, Browser browser, string contextId)
        {
            _connection = connection;
            Browser = browser;
            _id = contextId;
        }

        /// <summary>
        /// Raised when the url of a target changes
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a target is created, for example when a new page is opened by <c>window.open</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"/> or <see cref="NewPageAsync"/>.
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetCreated;

        /// <summary>
        /// Raised when a target is destroyed, for example when a page is closed
        /// </summary>
        public event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <summary>
        /// Returns whether BrowserContext is incognito
        /// The default browser context is the only non-incognito browser context
        /// </summary>
        /// <remarks>
        /// The default browser context cannot be closed
        /// </remarks>
        public bool IsIncognito => _id != null;

        /// <summary>
        /// Gets the browser this browser context belongs to
        /// </summary>
        public Browser Browser { get; }

        /// <summary>
        /// Gets an array of all active targets inside the browser context 
        /// </summary>
        /// <returns>An array of all active targets inside the browser context</returns>
        public Target[] Targets() => Array.FindAll(Browser.Targets(), target => target.BrowserContext == this);
 
        /// <summary>
        /// This searches for a target in this specific browser context.
        /// <example>
        /// <code>
        /// <![CDATA[
        /// await page.EvaluateAsync("() => window.open('https://www.example.com/')");
        /// var newWindowTarget = await browserContext.WaitForTargetAsync((target) => target.Url == "https://www.example.com/");
        /// ]]>
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="predicate">A function to be run for every target</param>
        /// <param name="options">options</param>
        /// <returns>Resolves to the first target found that matches the predicate function.</returns>
        public Task<Target> WaitForTargetAsync(Func<Target, bool> predicate, WaitForOptions options = null) 
            => Browser.WaitForTargetAsync((target) => target.BrowserContext == this && predicate(target), options);

        /// <summary>
        /// An array of all pages inside the browser context.
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages. 
        /// Non visible pages, such as <c>"background_page"</c>, will not be listed here. 
        /// You can find them using <see cref="Target.PageAsync"/>.</returns>
        public async Task<Page[]> PagesAsync()
        => (await Task.WhenAll(
            Targets().Where(t => t.Type == TargetType.Page).Select(t => t.PageAsync())).ConfigureAwait(false)
           ).Where(p => p != null).ToArray();

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="Page"/> object</returns>
        public Task<Page> NewPageAsync() => Browser.CreatePageInContextAsync(_id);

        /// <summary>
        /// Closes the browser context. All the targets that belong to the browser context will be closed
        /// </summary>
        /// <returns>Task</returns>
        public Task CloseAsync()
        {
            if (_id == null)
            {
                throw new PuppeteerException("Non-incognito profiles cannot be closed!");
            }
            return Browser.DisposeContextAsync(_id);
        }

        /// <summary>
        /// Overrides the browser context permissions.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="origin">The origin to grant permissions to, e.g. "https://example.com"</param>
        /// <param name="permissions">
        /// An array of permissions to grant. All permissions that are not listed here will be automatically denied.
        /// </param>
        /// <example>
        /// <![CDATA[
        /// var context = browser.DefaultBrowserContext;
        /// await context.OverridePermissionsAsync("https://html5demos.com", new List<string> {"geolocation"});
        /// ]]>
        /// </example>
        /// <seealso href="https://developer.mozilla.org/en-US/docs/Glossary/Origin"/>
        public Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
            => _connection.SendAsync("Browser.grantPermissions", new
            {
                origin,
                browserContextId = _id,
                permissions
            });

        /// <summary>
        /// Clears all permission overrides for the browser context.
        /// </summary>
        /// <returns>The task.</returns>
        public Task ClearPermissionOverridesAsync()
            => _connection.SendAsync("Browser.resetPermissions", new { browserContextId = _id });

        internal void OnTargetCreated(Browser browser, TargetChangedArgs args) => TargetCreated?.Invoke(browser, args);

        internal void OnTargetDestroyed(Browser browser, TargetChangedArgs args) => TargetDestroyed?.Invoke(browser, args);

        internal void OnTargetChanged(Browser browser, TargetChangedArgs args) => TargetChanged?.Invoke(browser, args);
    }
}
