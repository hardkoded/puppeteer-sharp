using System;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public class Touchscreen : ITouchscreen
    {
        private readonly CDPSession _client;
        private readonly Keyboard _keyboard;

        /// <inheritdoc cref="Touchscreen"/>
        public Touchscreen(CDPSession client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        /// <inheritdoc/>
        public async Task TapAsync(decimal x, decimal y)
        {
            // Touches appear to be lost during the first frame after navigation.
            // This waits a frame before sending the tap.
            // @see https://crbug.com/613219
            await _client.SendAsync("Runtime.evaluate", new RuntimeEvaluateRequest
            {
                Expression = "new Promise(x => requestAnimationFrame(() => requestAnimationFrame(x)))",
                AwaitPromise = true,
            }).ConfigureAwait(false);

            var touchPoints = new[] { new TouchPoint { X = Math.Round(x), Y = Math.Round(y) } };
            await _client.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = touchPoints,
                Modifiers = _keyboard.Modifiers,
            }).ConfigureAwait(false);
            await _client.SendAsync("Input.dispatchTouchEvent", new InputDispatchTouchEventRequest
            {
                Type = "touchEnd",
                TouchPoints = Array.Empty<TouchPoint>(),
                Modifiers = _keyboard.Modifiers,
            }).ConfigureAwait(false);
        }
    }
}
