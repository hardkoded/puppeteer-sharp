using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class DialogEventArgs : EventArgs
    {
        public Dialog Dialog { get; }

        public DialogEventArgs(Dialog dialog) => Dialog = dialog;
    }
}