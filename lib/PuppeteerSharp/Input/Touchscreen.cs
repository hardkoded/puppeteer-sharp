using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Provides methods to interact with the touch screen
    /// </summary>
    public class Touchscreen
    {
        private readonly CDPSession _client;
        private readonly Keyboard _keyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="Touchscreen"/> class.
        /// </summary>
        /// <param name="client">The client</param>
        /// <param name="keyboard">The keyboard</param>
        public Touchscreen(CDPSession client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        /// <summary>
        /// Dispatches a <c>touchstart</c> and <c>touchend</c> event.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Task</returns>
        /// <seealso cref="Page.TapAsync(string)"/>
        public async Task TapAsync(decimal x, decimal y)
        {
            // Touches appear to be lost during the first frame after navigation.
            // This waits a frame before sending the tap.
            // @see https://crbug.com/613219
            await _client.SendAsync("Runtime.evaluate", new
            {
                expression = "new Promise(x => requestAnimationFrame(() => requestAnimationFrame(x)))",
                awaitPromise = true
            });

            var touchPoints = new[] { new { x = Math.Round(x), y = Math.Round(y) } };
            await _client.SendAsync("Input.dispatchTouchEvent", new
            {
                type = "touchStart",
                touchPoints,
                modifiers = _keyboard.Modifiers
            });
            await _client.SendAsync("Input.dispatchTouchEvent", new
            {
                type = "touchEnd",
                touchPoints = Array.Empty<object>(),
                modifiers = _keyboard.Modifiers
            });
        }
    }
}
