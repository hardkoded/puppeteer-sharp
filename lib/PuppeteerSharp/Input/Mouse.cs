using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Mouse
    {
        private Session _client;
        private Keyboard _keyboard;
        private decimal _x = 0;
        private decimal _y = 0;
        private string _button = "none";

        public Mouse(Session client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        public async Task Move(decimal x, decimal y, Dictionary<string, object> options = null)
        {
            options = options ?? new Dictionary<string, object>();

            decimal fromX = _x;
            decimal fromY = _y;
            _x = x;
            _y = y;
            int steps = options.ContainsKey("steps") ? (int)options["steps"] : 1;

            for (var i = 1; i <= steps; i++)
            {
                await _client.SendAsync("Input.dispatchMouseEvent", new Dictionary<string, object>(){
                    {"type", "mouseMoved"},
                    {"button", _button},
                    {"x", fromX + (_x - fromX) * (i / steps)},
                    {"y", fromY + (_y - fromY) * (i / steps)},
                    {"modifiers", _keyboard.Modifiers}
                });
            }
        }

        public async Task Click(decimal x, decimal y, ClickOptions options)
        {
            options = options ?? new ClickOptions();

            await Move(x, y);
            await Down(options);

            if (options.Delay != null)
            {
                await Task.Delay((int)options.Delay);
            }
            await Up(options);
        }

        public async Task Down(ClickOptions options)
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

        public async Task Up(ClickOptions options)
        {
            options = options ?? new ClickOptions();

            _button = "none";

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
