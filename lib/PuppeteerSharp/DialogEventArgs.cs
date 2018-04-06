using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class DialogEventArgs : EventArgs
    {
        public DialogInfo DialogInfo { get; }

        public DialogEventArgs(DialogInfo dialogInfo) => DialogInfo = dialogInfo;
    }
}