using PuppeteerSharp.Cdp;

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
        private TargetInfo _targetInfo;

        internal TargetChangedArgs()
        {
        }

        internal TargetChangedArgs(Target target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets the target info.
        /// </summary>
        /// <value>The target info.</value>
        public TargetInfo TargetInfo
        {
            get => _targetInfo ?? (Target as CdpTarget)?.TargetInfo;
            internal set => _targetInfo = value;
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public Target Target { get; internal set; }
    }
}
