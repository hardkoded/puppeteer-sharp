using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    public class Touchscreen
    {
        private Session _client;
        private Keyboard _keyboard;

        public Touchscreen(Session client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

		public async Task Up(decimal x, decimal y)
		{
            var touchPoints = new[]{
                new {x= Math.Round(x), y = Math.Round(y)}
            };

			await _client.Send("Input.dispatchTouchEvent", new Dictionary<string, object>(){
				{"type", "tochStart"},
                {"touchPoints", touchPoints},
				{"modifiers", _keyboard.Modifiers},
			});

			await _client.Send("Input.dispatchTouchEvent", new Dictionary<string, object>(){
				{"type", "touchEnd"},
				{"touchPoints", touchPoints},
				{"modifiers", _keyboard.Modifiers},
			});
		}
    }
}
