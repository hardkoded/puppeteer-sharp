using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp
{
    /// <summary>
    /// Worker target.
    /// </summary>
    public class CdpWorkerTarget : CdpTarget
    {
        private Task<WebWorker> _workerTask;

        internal CdpWorkerTarget(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory,
            TaskQueue screenshotTaskQueue)
            : base(targetInfo, (CdpCDPSession)session, (CdpBrowserContext)context, targetManager, sessionFactory, screenshotTaskQueue)
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
            var worker = new CdpWebWorker(
                client,
                TargetInfo.Url,
                TargetInfo.TargetId,
                TargetInfo.Type,
                (_, _, _) => Task.CompletedTask,
                _ => { });

            // Service and shared workers are not owned by a page, so their console messages are bubbled up
            // to the browser context and the browser here.
            worker.Console += (_, e) => OnWorkerConsole(e);
            return worker;
        }

        private void OnWorkerConsole(ConsoleEventArgs e)
        {
            BrowserContext.OnConsole(e);
            Browser.OnConsole(e);
        }
    }
}
