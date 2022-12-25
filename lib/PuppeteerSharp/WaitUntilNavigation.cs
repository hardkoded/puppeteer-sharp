namespace PuppeteerSharp
{
    /// <summary>
    /// Wait until navigation.
    /// </summary>
    public enum WaitUntilNavigation
    {
        /// <summary>
        /// Consider navigation to be finished when the <c>load</c> event is fired.
        /// </summary>
        Load,

        /// <summary>
        /// Consider navigation to be finished when the <c>DOMContentLoaded</c> event is fired.
        /// </summary>
        DOMContentLoaded,

        /// <summary>
        /// Consider navigation to be finished when there are no more than 0 network connections for at least <c>500</c> ms.
        /// </summary>
        Networkidle0,

        /// <summary>
        /// Consider navigation to be finished when there are no more than 2 network connections for at least <c>500</c> ms.
        /// </summary>
        Networkidle2,
    }
}
