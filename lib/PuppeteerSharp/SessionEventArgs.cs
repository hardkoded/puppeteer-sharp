namespace PuppeteerSharp
{
    /// <summary>
    /// Session event arguments.
    /// </summary>
    public class SessionEventArgs
    {
        internal SessionEventArgs(ICDPSession session)
        {
            Session = session;
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        public ICDPSession Session { get; set; }
    }
}
