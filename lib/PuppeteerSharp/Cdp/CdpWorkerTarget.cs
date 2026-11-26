using System;
using System.Collections.Generic;
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
            TaskQueue screenshotTaskQueue
        )
            : base(
                targetInfo,
                (CdpCDPSession)session,
                (CdpBrowserContext)context,
                targetManager,
                sessionFactory,
                screenshotTaskQueue
            ) { }

        /// <inheritdoc/>
        public override Task<WebWorker> WorkerAsync()
        {
            lock (CloseLock)
            {
                if (_workerTask == null)
                {
                    // CloseAsync() detaches the sessions that are attached when Chrome closes
                    // the target, so a session attached later would keep a closed service
                    // worker alive.
                    if (IsClosing)
                    {
                        return Task.FromException<WebWorker>(
                            new TargetClosedException("Target closed")
                        );
                    }

                    _workerTask = WorkerInternalAsync();
                }

                return _workerTask;
            }
        }

        internal override async Task<IReadOnlyCollection<CdpCDPSession>> AttachedSessionsAsync()
        {
            var sessions = new HashSet<CdpCDPSession>(
                await base.AttachedSessionsAsync().ConfigureAwait(false)
            );

            Task<WebWorker> workerTask;
            lock (CloseLock)
            {
                workerTask = _workerTask;
            }

            // Puppeteer detaches from service workers unless network rules are set,
            // so WorkerAsync() may attach a session of its own.
            if (workerTask != null)
            {
                try
                {
                    var worker = await workerTask.ConfigureAwait(false);
                    sessions.Add((CdpCDPSession)worker.Client);
                }
                catch (Exception)
                {
                    // A failed WorkerAsync() call already faulted for its caller.
                }
            }

            return sessions;
        }

        private async Task<WebWorker> WorkerInternalAsync()
        {
            var client = Session ?? await SessionFactory(false).ConfigureAwait(false);
            var worker = new CdpWebWorker(client, this, (_, _, _) => Task.CompletedTask, _ => { });

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
