using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Workder created event arguments.
    /// </summary>
    public class WorkerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkerEventArgs"/> class.
        /// </summary>
        /// <param name="worker">Worker.</param>
        public WorkerEventArgs(Worker worker) => Worker = worker;

        /// <summary>
        /// Worker.
        /// </summary>
        /// <value>The worker.</value>
        public Worker Worker { get; set; }
    }
}
