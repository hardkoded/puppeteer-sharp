namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Options to be used in <see cref="IDevToolsContext.AuthenticateAsync(Credentials)"/>
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Gets or sets the username to be used for authentication
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password to be used for authentication
        /// </summary>
        public string Password { get; set; }
    }
}
