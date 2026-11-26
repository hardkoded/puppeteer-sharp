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
            // Checking that the target is not closing and publishing the worker task is one step under
            // the lock CloseAsync() uses, so the sessions it snapshots include this worker's session.
            lock (CloseLock)
            {
                if (_workerTask == null)
                {
                    // CloseAsync() detaches the sessions that are attached when Chrome closes
                    // the target, so a session attached later would keep a closed service
                    // worker alive.
                    if (IsClosing())
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

        /// <inheritdoc/>
        internal override async Task<IReadOnlyList<CDPSession>> AttachedSessionsAsync()
        {
            var sessions = new List<CDPSession>(
                await base.AttachedSessionsAsync().ConfigureAwait(false)
            );

            // Puppeteer detaches from service workers unless network rules are set,
            // so WorkerAsync() may attach a session of its own. A failed WorkerAsync() call
            // already rejected for its caller.
            WebWorker worker = null;
            try
            {
                worker = _workerTask != null ? await _workerTask.ConfigureAwait(false) : null;
            }
            catch (Exception)
            {
                // Ignore: the caller of WorkerAsync() already got the error.
            }

            if (worker is CdpWebWorker cdpWorker && cdpWorker.CdpCDPSession is { } workerSession && !sessions.Contains(workerSession))
            {
                sessions.Add(workerSession);
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
