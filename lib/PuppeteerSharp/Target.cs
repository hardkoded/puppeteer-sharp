using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target : ITarget
    {
        internal Target(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory)
        {
            Session = session;
            TargetInfo = targetInfo;
            SessionFactory = sessionFactory;
            BrowserContext = context;
            TargetManager = targetManager;

            Initialize();
        }

        /// <inheritdoc/>
        public string Url => TargetInfo.Url;

        /// <inheritdoc/>
        public TargetType Type => TargetInfo.Type;

        /// <inheritdoc/>
        public string TargetId => TargetInfo.TargetId;

        /// <inheritdoc/>
        public ITarget Opener => TargetInfo.OpenerId != null ?
            ((Browser)Browser).TargetManager.GetAvailableTargets().GetValueOrDefault(TargetInfo.OpenerId) : null;

        /// <inheritdoc/>
        public IBrowser Browser => BrowserContext.Browser;

        /// <inheritdoc/>
        IBrowserContext ITarget.BrowserContext => BrowserContext;

        internal BrowserContext BrowserContext { get; }

        internal Task<bool> InitializedTask => InitializedTaskWrapper.Task;

        internal TaskCompletionSource<bool> InitializedTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task CloseTask => CloseTaskWrapper.Task;

        internal TaskCompletionSource<bool> CloseTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Func<bool, Task<CDPSession>> SessionFactory { get; private set; }

        internal ITargetManager TargetManager { get; }

        internal bool IsInitialized { get; set; }

        internal CDPSession Session { get; }

        internal TargetInfo TargetInfo { get; set; }

        /// <inheritdoc/>
        public virtual Task<IPage> PageAsync() => Task.FromResult<IPage>(null);

        /// <inheritdoc/>
        public virtual Task<Worker> WorkerAsync() => Task.FromResult<Worker>(null);

        /// <inheritdoc/>
        public async Task<ICDPSession> CreateCDPSessionAsync() => await SessionFactory(false).ConfigureAwait(false);

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            TargetInfo = targetInfo;
            CheckIfInitialized();
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        protected virtual void Initialize()
        {
            IsInitialized = true;
            InitializedTaskWrapper.TrySetResult(true);
        }

        /// <summary>
        /// Check is the target is not initialized.
        /// </summary>
        protected virtual void CheckIfInitialized()
        {
            IsInitialized = true;
            InitializedTaskWrapper.TrySetResult(true);
        }
    }
}
