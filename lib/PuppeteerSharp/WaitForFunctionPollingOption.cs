namespace PuppeteerSharp
{
    /// <summary>
    /// An interval at which the <c>pageFunction</c> is executed.
    /// </summary>
    public enum WaitForFunctionPollingOption
    {
        /// <summary>
        /// To constantly execute <c>pageFunction</c> in <c>requestAnimationFrame</c> callback.
        /// This is the tightest polling mode which is suitable to observe styling changes.
        /// </summary>
        Raf,

        /// <summary>
        /// To execute <c>pageFunction</c> on every DOM mutation.
        /// </summary>
        Mutation,
    }
}
