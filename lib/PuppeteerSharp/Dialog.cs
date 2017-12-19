using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Dialog
    {
        private Session _client;
        private string _type;

        public Dialog(Session client, string type, string message)
        {
            _client = client;
            _type = type;
            Message = message;
        }

        public string Message { get; set; }
        public string DefaultValue { get; set; }

        public async Task Accept(string promptText)
        {
            await _client.SendAsync("Page.handleJavaScriptDialog", new Dictionary<string, object>(){
                {"accept", true},
                {"promptText", promptText}
            });
        }

        public async Task Dismiss(string promptText)
        {
            await _client.SendAsync("Page.handleJavaScriptDialog", new Dictionary<string, object>(){
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
