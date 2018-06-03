using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="Page.Dialog"/> arguments.
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        /// <summary>
        /// Dialog data.
        /// </summary>
        /// <value>Dialog data.</value>
        public Dialog Dialog { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEventArgs"/> class.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public DialogEventArgs(Dialog dialog) => Dialog = dialog;
    }
}