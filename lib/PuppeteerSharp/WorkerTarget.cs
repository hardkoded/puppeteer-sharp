using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Worker target.
    /// </summary>
    public class WorkerTarget : Target
    {
        private Task<Worker> _workerTask;

        internal WorkerTarget(TargetInfo targetInfo, CDPSession session, BrowserContext context, ITargetManager targetManager, Func<bool, Task<CDPSession>> sessionFactory) : base(targetInfo, session, context, targetManager, sessionFactory)
        {
        }

        /// <inheritdoc/>
        public override Task<Worker> WorkerAsync()
        {
            _workerTask ??= WorkerInternalAsync();
            return _workerTask;
        }

        private async Task<Worker> WorkerInternalAsync()
        {
            var client = Session ?? await SessionFactory(false).ConfigureAwait(false);
            return new Worker(
                client,
                TargetInfo.Url,
                (_, _, _) => Task.CompletedTask,
                _ => { });
        }
    }
}
