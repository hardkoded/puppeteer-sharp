using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Popup event arguments. <see cref="IPage.Popup"/>.
    /// </summary>
    public class PopupEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the popup page.
        /// </summary>
        public IPage PopupPage { get; internal set; }
    }
}
