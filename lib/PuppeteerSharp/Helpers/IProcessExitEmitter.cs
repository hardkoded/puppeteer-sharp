using System;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Abstraction over host-process exit notifications, matching Node's
    /// <c>process.once('exit')</c> / <c>process.off('exit')</c> for testability.
    /// </summary>
    internal interface IProcessExitEmitter
    {
        /// <summary>
        /// Gets the number of currently registered exit listeners.
        /// </summary>
        int ListenerCount { get; }

        /// <summary>
        /// Registers a one-shot exit listener.
        /// </summary>
        /// <param name="listener">The listener to invoke on process exit.</param>
        void Once(Action listener);

        /// <summary>
        /// Removes a previously registered exit listener.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        void Off(Action listener);
    }
}
