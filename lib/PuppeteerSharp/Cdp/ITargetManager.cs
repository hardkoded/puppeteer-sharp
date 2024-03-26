using System;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
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
        AsyncDictionaryHelper<string, CdpTarget> GetAvailableTargets();

        /// <summary>
        /// Async tasks to be performed after calling the target manager constructor.
        /// </summary>
        /// <returns>A task that resolves when all the tasks are completed.</returns>
        Task InitializeAsync();
    }
}
