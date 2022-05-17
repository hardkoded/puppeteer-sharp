using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.Dialog"/> arguments.
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogEventArgs"/> class.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public DialogEventArgs(Dialog dialog) => Dialog = dialog;

        /// <summary>
        /// Dialog data.
        /// </summary>
        public Dialog Dialog { get; }
    }
}
