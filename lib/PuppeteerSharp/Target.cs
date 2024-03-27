using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public abstract class Target : ITarget
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

            if (session != null)
            {
                session.Target = this;
            }
        }

        /// <inheritdoc/>
        public string Url => TargetInfo.Url;

        /// <inheritdoc/>
        public virtual TargetType Type => TargetInfo.Type;

        /// <inheritdoc/>
        public string TargetId => TargetInfo.TargetId;

        /// <inheritdoc/>
        public abstract ITarget Opener { get; }

        /// <inheritdoc/>
        IBrowser ITarget.Browser => Browser;

        /// <inheritdoc/>
        IBrowserContext ITarget.BrowserContext => BrowserContext;

        internal BrowserContext BrowserContext { get; }

        internal Browser Browser => BrowserContext.Browser;

        internal Task<InitializationStatus> InitializedTask => InitializedTaskWrapper.Task;

        internal TaskCompletionSource<InitializationStatus> InitializedTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

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
        public virtual Task<WebWorker> WorkerAsync() => Task.FromResult<WebWorker>(null);

        /// <inheritdoc/>
        public abstract Task<IPage> AsPageAsync();

        /// <inheritdoc/>
        public abstract Task<ICDPSession> CreateCDPSessionAsync();
    }
}
