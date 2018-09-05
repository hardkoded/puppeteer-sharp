namespace PuppeteerSharp
{
    /// <summary>
    /// Exception thrown by the <see cref="Connection"/> when it detects that the target was closed.
    /// </summary>
    public class TargetClosedException : PuppeteerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetClosedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public TargetClosedException(string message) : base(message)
        {
        }
    }
}