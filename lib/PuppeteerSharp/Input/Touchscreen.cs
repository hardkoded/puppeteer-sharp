using System;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom.Input
{
    /// <summary>
    /// Provides methods to interact with the touch screen
    /// </summary>
    public class Touchscreen
    {
        private readonly DevToolsConnection _connection;
        private readonly Keyboard _keyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="Touchscreen"/> class.
        /// </summary>
        /// <param name="connection">The connection</param>
        /// <param name="keyboard">The keyboard</param>
        public Touchscreen(DevToolsConnection connection, Keyboard keyboard)
        {
            _connection = connection;
            _keyboard = keyboard;
        }

        /// <summary>
        /// Dispatches a <c>touchstart</c> and <c>touchend</c> event.
        /// </summary>
        /// <param name="x">The touch X location.</param>
        /// <param name="y">The touch Y location.</param>
        /// <returns>Task</returns>
        /// <seealso cref="IDevToolsContext.TapAsync(string)"/>
        public async Task TapAsync(decimal x, decimal y)
        {
            // Touches appear to be lost during the first frame after navigation.
            // This waits a frame before sending the tap.
            // @see https://crbug.com/613219
            await _connection.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "new Promise(x => requestAnimationFrame(() => requestAnimationFrame(x)))",
                AwaitPromise = true
            }).ConfigureAwait(false);

            var touchPoints = new[] { new TouchPoint { X = Math.Round(x), Y = Math.Round(y) } };
            await _connection.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = touchPoints,
                Modifiers = _keyboard.Modifiers
            }).ConfigureAwait(false);
            await _connection.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchEnd",
                TouchPoints = Array.Empty<TouchPoint>(),
                Modifiers = _keyboard.Modifiers
            }).ConfigureAwait(false);
        }
    }
}
