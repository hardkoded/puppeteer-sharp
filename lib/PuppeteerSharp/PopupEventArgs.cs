using System;

namespace CefSharp.Dom
{
    /// <summary>
    /// Popup event arguments. <see cref="IDevToolsContext.Popup"/>
    /// </summary>
    public class PopupEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the popup page.
        /// </summary>
        public IDevToolsContext PopupPage { get; internal set; }
    }
}
