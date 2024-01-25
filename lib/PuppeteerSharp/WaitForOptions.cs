namespace PuppeteerSharp
{
    /// <summary>
    /// Timeout options.
    /// </summary>
    public class WaitForOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForOptions"/> class.
        /// </summary>
        public WaitForOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForOptions"/> class.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.</param>
        public WaitForOptions(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
        /// The default value can be changed by setting the <see cref="IPage.DefaultTimeout"/> property.
        /// </summary>
        public int? Timeout { get; set; }
    }
}
