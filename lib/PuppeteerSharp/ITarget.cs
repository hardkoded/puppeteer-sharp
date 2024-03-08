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
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        string TargetId { get; }

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
    }
}
