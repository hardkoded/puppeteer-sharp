using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Provides methods to interact with the touch screen.
    /// </summary>
    public interface ITouchscreen
    {
        /// <summary>
        /// Dispatches a <c>touchstart</c> and <c>touchend</c> event.
        /// </summary>
        /// <param name="x">The touch X location.</param>
        /// <param name="y">The touch Y location.</param>
        /// <returns>Task.</returns>
        /// <seealso cref="IPage.TapAsync(string)"/>
        Task TapAsync(decimal x, decimal y);

        /// <summary>
        /// Dispatches a <c>touchstart</c> event.
        /// </summary>
        /// <param name="x">The touch X location.</param>
        /// <param name="y">The touch Y location.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchStartAsync(decimal x, decimal y);

        /// <summary>
        /// Dispatches a <c>touchmove</c> event.
        /// </summary>
        /// <param name="x">The touch X location.</param>
        /// <param name="y">The touch Y location.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchMoveAsync(decimal x, decimal y);

        /// <summary>
        /// /// Dispatches a <c>touchendt</c> event.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchEndAsync();
    }
}
