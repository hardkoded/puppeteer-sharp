using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has
    /// a single <see cref="IBrowserContext"/> used by default. The method <see cref="IBrowser.NewPageAsync"/> creates a <see cref="IPage"/> in the default <see cref="IBrowserContext"/>.
    /// </summary>
    public interface IBrowserContext
    {
        /// <summary>
        /// Raised when the url of a target changes
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a target is created, for example when a new page is opened by <c>window.open</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"/> or <see cref="NewPageAsync"/>.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetCreated;

        /// <summary>
        /// Raised when a target is destroyed, for example when a page is closed
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetDestroyed;

        /// <summary>
        /// Browser Context Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the browser this browser context belongs to.
        /// </summary>
        IBrowser Browser { get; }

        /// <summary>
        /// Whether this <see href="IBrowserContext"/> is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Returns whether BrowserContext is incognito
        /// The default browser context is the only non-incognito browser context.
        /// </summary>
        /// <remarks>
        /// The default browser context cannot be closed.
        /// </remarks>
        [Obsolete("In Chrome, the default browser context can also be \"icognito\" if configured via the arguments and in such cases this getter returns wrong results. Also, the term \"incognito\" is not applicable to other browsers. To migrate, check the default browser context instead: in Chrome all non-default contexts are incognito, and the default context might be incognito if you provide the `--incognito` argument when launching the browser.")]
        bool IsIncognito { get; }

        /// <summary>
        /// Clears all permission overrides for the browser context.
        /// </summary>
        /// <returns>The task.</returns>
        Task ClearPermissionOverridesAsync();

        /// <summary>
        /// Closes the browser context. All the targets that belong to the browser context will be closed.
        /// </summary>
        /// <returns>Task.</returns>
        Task CloseAsync();

        /// <summary>
        /// Creates a new page.
        /// </summary>
        /// <returns>Task which resolves to a new <see cref="IPage"/> object.</returns>
        Task<IPage> NewPageAsync();

        /// <summary>
        /// Overrides the browser context permissions.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="origin">The origin to grant permissions to, e.g. "https://example.com".</param>
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
        Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions);

        /// <summary>
        /// An array of all pages inside the browser context.
        /// </summary>
        /// <returns>Task which resolves to an array of all open pages.
        /// Non visible pages, such as <c>"background_page"</c>, will not be listed here.
        /// You can find them using <see cref="ITarget.PageAsync"/>.</returns>
        Task<IPage[]> PagesAsync();

        /// <summary>
        /// Gets an array of all active targets inside the browser context.
        /// </summary>
        /// <returns>An array of all active targets inside the browser context.</returns>
        ITarget[] Targets();

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
        /// <param name="predicate">A function to be run for every target.</param>
        /// <param name="options">options.</param>
        /// <returns>Resolves to the first target found that matches the predicate function.</returns>
        Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null);
    }
}
