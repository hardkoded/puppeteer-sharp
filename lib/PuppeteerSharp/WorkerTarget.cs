using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Worker target.
    /// </summary>
    public class WorkerTarget : Target
    {
        private Task<WebWorker> _workerTask;

        internal WorkerTarget(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory)
            : base(targetInfo, session, context, targetManager, sessionFactory)
        {
        }

        /// <inheritdoc/>
        public override Task<WebWorker> WorkerAsync()
        {
            _workerTask ??= WorkerInternalAsync();
            return _workerTask;
        }

        private async Task<WebWorker> WorkerInternalAsync()
        {
            var client = Session ?? await SessionFactory(false).ConfigureAwait(false);
            return new WebWorker(
                client,
                TargetInfo.Url,
                TargetInfo.TargetId,
                TargetInfo.Type,
                (_, _, _) => Task.CompletedTask,
                _ => { });
        }
    }
}
