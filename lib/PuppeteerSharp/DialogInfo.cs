using System;

namespace PuppeteerSharp
{
    public class DialogInfo
    {
        public DialogType DialogType { get; set; }
        public string DefaultValue { get; set; }
        public string Message { get; set; }

        public void Accept(string promptText = null)
        {
            throw new NotImplementedException();
        }

        public void Dismiss()
        {
            throw new NotImplementedException();
        }
    }
}