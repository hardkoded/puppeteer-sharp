using System;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public class Touchscreen : ITouchscreen
    {
        private readonly Keyboard _keyboard;
        private CDPSession _client;

        /// <inheritdoc cref="Touchscreen"/>
        public Touchscreen(CDPSession client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        /// <inheritdoc />
        public async Task TapAsync(decimal x, decimal y)
        {
            await TouchStartAsync(x, y).ConfigureAwait(false);
            await TouchEndAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task TouchStartAsync(decimal x, decimal y)
        {
            var touchPoints = new[] { new TouchPoint { X = Math.Round(x), Y = Math.Round(y) } };
            return _client.SendAsync(
                "Input.dispatchTouchEvent",
                new InputDispatchTouchEventRequest
                {
                    Type = "touchStart",
                    TouchPoints = touchPoints,
                    Modifiers = _keyboard.Modifiers,
                });
        }

        /// <inheritdoc />
        public Task TouchMoveAsync(decimal x, decimal y)
        {
            var touchPoints = new[] { new TouchPoint { X = Math.Round(x), Y = Math.Round(y) } };
            return _client.SendAsync(
                "Input.dispatchTouchEvent",
                new InputDispatchTouchEventRequest
                {
                    Type = "touchStart",
                    TouchPoints = touchPoints,
                    Modifiers = _keyboard.Modifiers,
                });
        }

        /// <inheritdoc />
        public Task TouchEndAsync()
        {
            return _client.SendAsync(
                "Input.dispatchTouchEvent",
                new InputDispatchTouchEventRequest
                {
                    Type = "touchEnd",
                    TouchPoints = [],
                    Modifiers = _keyboard.Modifiers,
                });
        }

        internal void UpdateClient(CDPSession newSession) => _client = newSession;
    }
}
