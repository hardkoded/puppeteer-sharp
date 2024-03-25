using System;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;

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
            Func<bool, Task<CDPSession>> sessionFactory,
            TaskQueue screenshotTaskQueue)
            : base(targetInfo, session, context, targetManager, sessionFactory, screenshotTaskQueue)
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
            return new CdpWebWorker(
                client,
                TargetInfo.Url,
                TargetInfo.TargetId,
                TargetInfo.Type,
                (_, _, _) => Task.CompletedTask,
                _ => { });
        }
    }
}
