namespace PuppeteerSharp
{
    /// <summary>
    /// Options used by <see cref="Page.SetContentAsync(string, SetContentOptions)"/> and <see cref="Frame.SetContentAsync(string, SetContentOptions)"/>
    /// </summary>
    public class SetContentOptions
    {
        /// <summary>
        /// Maximum navigation time in milliseconds, defaults to 30 seconds, pass <c>0</c> to disable timeout. 
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// When to consider navigation succeeded, defaults to <see cref="WaitUntilNavigation.Load"/>. Given an array of <see cref="WaitUntilNavigation"/>, navigation is considered to be successful after all events have been fired
        /// </summary>
        public WaitUntilNavigation[] WaitUntil { get; set; }
    }
}