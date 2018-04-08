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
        private static readonly Dictionary<string, DialogType> _dialogTypeMap = new Dictionary<string, DialogType>
        {
            ["alert"] = DialogType.Alert,
            ["prompt"] = DialogType.Prompt,
            ["confirm"] = DialogType.Confirm,
            ["beforeunload"] = DialogType.BeforeUnload
        };

        public Dialog(Session client, string type, string message, string defaultValue)
        {
            _client = client;
            DialogType = GetDialogType(type);
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

        public static DialogType GetDialogType(string dialogType)
        {
            if (_dialogTypeMap.ContainsKey(dialogType))
            {
                return _dialogTypeMap[dialogType];
            }

            throw new PuppeteerException($"Unknown javascript dialog type {dialogType}");
        }
    }
}
