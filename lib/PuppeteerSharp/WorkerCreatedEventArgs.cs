using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Workder created event arguments.
    /// </summary>
    public class WorkerCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Worker
        /// </summary>
        /// <value>The worker.</value>
        public Worker Worker { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerCreatedEventArgs"/> class.
        /// </summary>
        /// <param name="worker">Worker.</param>
        public WorkerCreatedEventArgs(Worker worker) => Worker = worker;
    }
}