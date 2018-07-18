using System.Collections.Generic;
using System.Threading.Tasks;

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
                await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>(){
                    {"type", "mouseMoved"},
                    {"button", _button},
                    {"x", fromX + ((_x - fromX) * ((decimal)i / steps))},
                    {"y", fromY + ((_y - fromY) * ((decimal)i / steps))},
                    {"modifiers", _keyboard.Modifiers}
                });
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

            await MoveAsync(x, y);
            await DownAsync(options);

            if (options.Delay > 0)
            {
                await Task.Delay(options.Delay);
            }
            await UpAsync(options);
        }

        /// <summary>
        /// Dispatches a <c>mousedown</c> event.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public async Task DownAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = options.Button;

            await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>(){
                {"type", "mousePressed"},
                {"button", _button},
                {"x", _x},
                {"y", _y},
                {"modifiers", _keyboard.Modifiers},
                {"clickCount", options.ClickCount }
            });
        }

        /// <summary>
        /// Dispatches a <c>mouseup</c> event.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task</returns>
        public async Task UpAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = MouseButton.None;

            await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>(){
                {"type", "mouseReleased"},
                {"button", options.Button},
                {"x", _x},
                {"y", _y},
                {"modifiers", _keyboard.Modifiers},
                {"clickCount", options.ClickCount }
            });
        }
    }
}
