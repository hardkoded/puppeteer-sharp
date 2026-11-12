using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Event arguments for uncaught exceptions thrown in a page or worker.
    /// </summary>
    public class RemoteExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public RemoteExceptionEventArgs(RemoteException exception) => Exception = exception;

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public RemoteException Exception { get; }
    }
}
