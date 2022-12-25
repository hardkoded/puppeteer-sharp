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
    }
}
