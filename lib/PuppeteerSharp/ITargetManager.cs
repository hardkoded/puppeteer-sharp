using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    internal delegate void TargetInterceptor(Target createdTarget, Target parentTarget);

    /// <summary>
    /// Target manager.
    /// </summary>
    internal interface ITargetManager
    {
        /// <summary>
        /// Raised when a target is available.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetAvailable;

        /// <summary>
        /// Raised when a target is gone.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetGone;

        /// <summary>
        /// Raise when the info of a target has changed.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetChanged;

        /// <summary>
        /// Raised when a new target is sent by the browser.
        /// </summary>
        event EventHandler<TargetChangedArgs> TargetDiscovered;

        /// <summary>
        /// All the available targets.
        /// </summary>
        /// <returns>A dictionary with the available targets.</returns>
        ConcurrentDictionary<string, Target> GetAvailableTargets();

        /// <summary>
        /// Async tasks to be performed after calling the target manager constructor.
        /// </summary>
        /// <returns>A task that resolves when all the tasks are completed.</returns>
        Task InitializeAsync();

        /// <summary>
        /// Adds a target interceptor.
        /// </summary>
        /// <param name="session">Session to listen for new targets.</param>
        /// <param name="interceptor">Interceptor function.</param>
        void AddTargetInterceptor(CDPSession session, TargetInterceptor interceptor);

        /// <summary>
        /// Removes a target interceptor.
        /// </summary>
        /// <param name="session">Session to remove the interceptor from.</param>
        /// <param name="interceptor">Interceptor function to remove.</param>
        void RemoveTargetInterceptor(CDPSession session, TargetInterceptor interceptor);
    }
}