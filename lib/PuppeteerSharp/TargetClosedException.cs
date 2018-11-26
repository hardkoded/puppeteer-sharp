namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by the <see cref="Connection"/> when it detects that the target was closed.
    /// </summary>
    public class TargetClosedException : PuppeteerException
    {
        /// <summary>
        /// Close Reason.
        /// </summary>
        /// <value>The close reason.</value>
        public string CloseReason { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="closeReason">Close reason.</param>
        public TargetClosedException(string message, string closeReason) : base(message) => CloseReason = closeReason;
    }
}