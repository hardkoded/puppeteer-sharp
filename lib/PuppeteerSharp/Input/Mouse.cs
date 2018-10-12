using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Provides methods to interact with the mouse
    /// </summary>
    public class Mouse
    {
        private readonly CDPSession _client;
        private readonly Keyboard _keyboard;

        private decimal _x = 0;
        private decimal _y = 0;
        private MouseButton _button = MouseButton.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mouse"/> class.
        /// </summary>
        /// <param name="client">The client</param>
        /// <param name="keyboard">The keyboard</param>
        public Mouse(CDPSession client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        /// <summary>
        /// Dispatches a <c>mousemove</c> event.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public async Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
        {
            options = options ?? new MoveOptions();

            decimal fromX = _x;
            decimal fromY = _y;
            _x = x;
            _y = y;
            int steps = options.Steps;

            for (var i = 1; i <= steps; i++)
            {
                await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>
                {
                    { MessageKeys.Type, "mouseMoved" },
                    { MessageKeys.Button, _button },
                    { MessageKeys.X, fromX + ((_x - fromX) * ((decimal)i / steps)) },
                    { MessageKeys.Y, fromY + ((_y - fromY) * ((decimal)i / steps)) },
                    { MessageKeys.Modifiers, _keyboard.Modifiers}
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Shortcut for <see cref="MoveAsync(decimal, decimal, MoveOptions)"/>, <see cref="DownAsync(ClickOptions)"/> and <see cref="UpAsync(ClickOptions)"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public async Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            await MoveAsync(x, y).ConfigureAwait(false);
            await DownAsync(options).ConfigureAwait(false);

            if (options.Delay > 0)
            {
                await Task.Delay(options.Delay).ConfigureAwait(false);
            }
            await UpAsync(options).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispatches a <c>mousedown</c> event.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public Task DownAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = options.Button;

            return _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>()
            {
                { MessageKeys.Type, "mousePressed" },
                { MessageKeys.Button, _button },
                { MessageKeys.X, _x },
                { MessageKeys.Y, _y },
                { MessageKeys.Modifiers, _keyboard.Modifiers },
                { MessageKeys.ClickCount, options.ClickCount }
            });
        }

        /// <summary>
        /// Dispatches a <c>mouseup</c> event.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public Task UpAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = MouseButton.None;

            return _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>()
            {
                { MessageKeys.Type, "mouseReleased" },
                { MessageKeys.Button, options.Button },
                { MessageKeys.X, _x },
                { MessageKeys.Y, _y },
                { MessageKeys.Modifiers, _keyboard.Modifiers },
                { MessageKeys.ClickCount, options.ClickCount }
            });
        }
    }
}
