using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class DialogEventArgs : EventArgs
    {
        public Dialog DialogInfo { get; }

        public DialogEventArgs(Dialog dialogInfo) => DialogInfo = dialogInfo;
    }
}