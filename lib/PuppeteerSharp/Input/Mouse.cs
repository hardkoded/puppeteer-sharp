using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Mouse
    {
        private readonly Session _client;
        private readonly Keyboard _keyboard;

        private decimal _x = 0;
        private decimal _y = 0;
        private MouseButton _button = MouseButton.None;

        public Mouse(Session client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        public async Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
        {
            options = options ?? new MoveOptions();

            decimal fromX = _x;
            decimal fromY = _y;
            _x = x;
            _y = y;
            int steps = options.Steps != null ? (int)options.Steps : 1;

            for (var i = 1; i <= steps; i++)
            {
                await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>(){
                    {"type", "mouseMoved"},
                    {"button", _button},
                    {"x", fromX + (_x - fromX) * ((decimal)i / steps)},
                    {"y", fromY + (_y - fromY) * ((decimal)i / steps)},
                    {"modifiers", _keyboard.Modifiers}
                });
            }
        }

        /// <summary>
        /// Shortcut for <see cref="Mouse.MoveAsync(decimal, decimal, MoveOptions)"/>, <see cref="Mouse.DownAsync(ClickOptions)"/> and <see cref="Mouse.UpAsync(ClickOptions)"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
