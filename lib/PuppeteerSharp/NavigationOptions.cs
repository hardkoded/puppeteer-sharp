namespace PuppeteerSharp
{
    /// <summary>
    /// Navigation options used by <see cref="Page.WaitForNavigationAsync(NavigationOptions)"/> and <see cref="Page.SetContentAsync(string, NavigationOptions)"/>
    /// </summary>
    public class NavigationOptions
    {
        /// <summary>
        /// Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. 
        /// </summary>
        /// <remarks>
        /// The default value can be changed by setting the <see cref="Page.DefaultNavigationTimeout"/> property.
        /// </remarks>
        public int? Timeout { get; set; }

        /// <summary>
        /// When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired
        /// </summary>
        public WaitUntilNavigation[] WaitUntil { get; set; }

        /// <summary>
        /// Referer header value. If provided it will take prefrence over the referer header value set by <see cref="Page.SetExtraHttpHeadersAsync(System.Collections.Generic.Dictionary{string, string})"/>
        /// </summary>
        /// <value>The referer.</value>
        public string Referer { get; set; }
    }
}