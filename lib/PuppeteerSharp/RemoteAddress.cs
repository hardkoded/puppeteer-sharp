namespace PuppeteerSharp
{
    /// <summary>
    /// Remote server address.
    /// </summary>
    /// <seealso cref="IResponse.RemoteAddress"/>
    public class RemoteAddress
    {
        /// <summary>
        /// The IP address of the remote server.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// The port used to connect to the remote server.
        /// </summary>
        public int Port { get; set; }
    }
}
