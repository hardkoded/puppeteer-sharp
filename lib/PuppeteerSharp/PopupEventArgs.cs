using System;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Popup event arguments. <see cref="DevToolsContext.Popup"/>
    /// </summary>
    public class PopupEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the popup page.
        /// </summary>
        public DevToolsContext PopupPage { get; internal set; }
    }
}
