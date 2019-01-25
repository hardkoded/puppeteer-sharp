using System;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

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
            await _client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "new Promise(x => requestAnimationFrame(() => requestAnimationFrame(x)))",
                AwaitPromise = true
            }).ConfigureAwait(false);

            var touchPoints = new[] { new TouchPoint { X = Math.Round(x), Y = Math.Round(y) } };
            await _client.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = touchPoints,
                Modifiers = _keyboard.Modifiers
            }).ConfigureAwait(false);
            await _client.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchEnd",
                TouchPoints = Array.Empty<TouchPoint>(),
                Modifiers = _keyboard.Modifiers
            }).ConfigureAwait(false);
        }
    }
}
