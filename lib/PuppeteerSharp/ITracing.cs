using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// You can use <see cref="ITracing.StartAsync(TracingOptions)"/> and <see cref="ITracing.StopAsync"/> to create a trace file which can be opened in Chrome DevTools or timeline viewer.
    /// </summary>
    /// <example>
    /// <code>
    /// await Page.Tracing.StartAsync(new TracingOptions
    /// {
    ///     Screenshots = true,
    ///     Path = _file
    /// });
    /// await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
    /// await Page.Tracing.StopAsync();
    /// </code>
    /// </example>
    public interface ITracing
    {
        /// <summary>
        /// Starts tracing.
        /// </summary>
        /// <returns>Start task.</returns>
        /// <param name="options">Tracing options.</param>
        Task StartAsync(TracingOptions options = null);

        /// <summary>
        /// Stops tracing.
        /// </summary>
        /// <returns>Stop task.</returns>
        Task<string> StopAsync();
    }
}
