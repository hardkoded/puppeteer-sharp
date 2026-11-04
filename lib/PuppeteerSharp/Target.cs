using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public abstract class Target : ITarget
    {
        private readonly ConcurrentSet<ITarget> _childTargets = [];

        internal Target()
        {
        }

        /// <inheritdoc/>
        public abstract string Url { get; }

        /// <inheritdoc/>
        public abstract TargetType Type { get; }

        /// <inheritdoc/>
        public abstract ITarget Opener { get; }

        /// <inheritdoc/>
        IBrowser ITarget.Browser { get; }

        /// <inheritdoc/>
        IBrowserContext ITarget.BrowserContext => BrowserContext;

        /// <inheritdoc/>
        IEnumerable<ITarget> ITarget.ChildTargets => _childTargets;

        internal abstract BrowserContext BrowserContext { get; }

        internal abstract Browser Browser { get; }

        /// <inheritdoc/>
        public virtual Task<IPage> PageAsync() => Task.FromResult<IPage>(null);

        /// <inheritdoc/>
        public virtual Task<WebWorker> WorkerAsync() => Task.FromResult<WebWorker>(null);

        /// <inheritdoc/>
        public abstract Task<IPage> AsPageAsync();

        /// <inheritdoc/>
        public abstract Task<ICDPSession> CreateCDPSessionAsync();

        internal void AddChildTarget(CdpTarget target) => _childTargets.Add(target);

        internal void RemoveChildTarget(CdpTarget target) => _childTargets.Remove(target);
    }
}
