using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Dialog
    {
        private Session _client;
        public DialogType DialogType { get; set; }
        public string DefaultValue { get; set; }
        public string Message { get; set; }

        public Dialog(Session client, DialogType type, string message, string defaultValue)
        {
            _client = client;
            DialogType = type;
            Message = message;
            DefaultValue = defaultValue;
        }

        public async Task Accept(string promptText = "")
        {
            await _client.SendAsync("Page.handleJavaScriptDialog", new Dictionary<string, object>
            {
                {"accept", true},
                {"promptText", promptText}
            });
        }

        public async Task Dismiss()
        {
            await _client.SendAsync("Page.handleJavaScriptDialog", new Dictionary<string, object>
            {
                {"accept", false}
            });
        }

        public class Type
        {
            public const string Alert = "alert";
            public const string BeforeUnload = "beforeunload";
            public const string Confirm = "confirm";
            public const string Prompt = "prompt";
        }
    }
}
