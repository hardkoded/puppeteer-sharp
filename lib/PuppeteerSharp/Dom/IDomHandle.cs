using System;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// DomHandle is a base clas providing for all DOM/HTML Elements
    /// </summary>
    public interface IDomHandle : IAsyncDisposable
    {
        /// <summary>
        /// Class Name
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Javascript Handle
        /// </summary>
        JSHandle Handle { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DomHandle"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        bool IsDisposed { get; }
    }
}
