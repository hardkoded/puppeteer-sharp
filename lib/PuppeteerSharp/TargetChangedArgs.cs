namespace PuppeteerSharp
{
    /// <summary>
    ///  Event arguments used by target related events.
    /// </summary>
    /// <seealso cref="IBrowser.TargetChanged"/>
    /// <seealso cref="IBrowser.TargetCreated"/>
    /// <seealso cref="IBrowser.TargetDestroyed"/>
    public class TargetChangedArgs
    {
        /// <summary>
        /// Gets the target info.
        /// </summary>
        /// <value>The target info.</value>
        public TargetInfo TargetInfo { get; internal set; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public Target Target { get; internal set; }
    }
}
