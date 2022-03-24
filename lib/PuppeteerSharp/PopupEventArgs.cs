using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Popup event arguments. <see cref="Page.Popup"/>
    /// </summary>
    public class PopupEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the popup page.
        /// </summary>
        public Page PopupPage { get; internal set; }
    }
}
