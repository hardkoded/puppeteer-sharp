using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    public interface ITarget
    {
        /// <summary>
        /// Get the browser the target belongs to.
        /// </summary>
        IBrowser Browser { get; }

        /// <summary>
        /// Get the browser context the target belongs to.
        /// </summary>
        IBrowserContext BrowserContext { get; }

        /// <summary>
        /// Get the target that opened this target.
        /// </summary>
        /// <remarks>
        /// Top-level targets return <c>null</c>.
        /// </remarks>
        ITarget Opener { get; }

        /// <summary>
        /// Gets the type. It will be <see cref="PuppeteerSharp.TargetInfo.Type"/>.
        /// Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.
        /// </summary>
        /// <value>The type.</value>
        TargetType Type { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; }

        /// <summary>
        /// Gets the target's child targets.
        /// </summary>
        IEnumerable<ITarget> ChildTargets { get; }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="ICDPSession"/>.</returns>
        Task<ICDPSession> CreateCDPSessionAsync();

        /// <summary>
        /// Returns the <see cref="IPage"/> associated with the target. If the target is not <c>"page"</c>, <c>"webview"</c> or <c>"background_page"</c> returns <c>null</c>.
        /// </summary>
        /// <returns>a task that returns a <see cref="IPage"/>.</returns>
        Task<IPage> PageAsync();

        /// <summary>
        /// If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.
        /// </summary>
        /// <returns>A task that returns a <see cref="WebWorker"/>.</returns>
        Task<WebWorker> WorkerAsync();

        /// <summary>
        /// Forcefully creates a page for a target of any type. It is useful if you
        /// want to handle a CDP target of type `other` as a page. If you deal with a
        /// regular page target, use <see cref="ITarget.PageAsync"/>.
        /// </summary>
        /// <returns>A task that returns a <see cref="IPage"/>.</returns>
        Task<IPage> AsPageAsync();

        /// <summary>
        /// Closes the target.
        /// </summary>
        /// <remarks>
        /// It does not create a <see cref="IPage"/> for the target, so it also closes a page target whose page could not be created.
        /// It does not run <c>beforeunload</c> handlers.
        /// Throws if the target does not support closing, for example the browser target, the target of a frame or a dedicated worker.
        /// Chrome destroys a service worker only after all CDP sessions attached to it detach.
        /// While a different session, for example one from <see cref="CreateCDPSessionAsync"/>, stays attached to a service worker, <see cref="CloseAsync"/> might not complete.
        /// </remarks>
        /// <returns>A task that completes when the target is closed.</returns>
        Task CloseAsync();
    }
}
