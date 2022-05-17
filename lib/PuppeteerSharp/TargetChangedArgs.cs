namespace PuppeteerSharp
{
    /// <summary>
    ///  Event arguments used by target related events.
    /// </summary>
    /// <seealso cref="Browser.TargetChanged"/>
    /// <seealso cref="Browser.TargetCreated"/>
    /// <seealso cref="Browser.TargetDestroyed"/>
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
        public ITarget Target { get; internal set; }
    }
}
