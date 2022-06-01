using System;
using System.Threading.Tasks;
using System.Timers;
using CefSharp.OffScreen;

namespace PuppeteerSharp.Tests
{
    public static class WebBrowserExtensions
    {
        public static Task WaitForRenderIdle(this ChromiumWebBrowser chromiumWebBrowser, int idleTime = 500)
        {
            var renderIdleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var idleTimer = new Timer
            {
                Interval = idleTime,
                AutoReset = false
            };

            EventHandler<OnPaintEventArgs> handler = null;

            idleTimer.Elapsed += (sender, args) =>
            {
                chromiumWebBrowser.Paint -= handler;

                idleTimer.Stop();
                idleTimer.Dispose();

                renderIdleTcs.TrySetResult(true);
            };

            handler = (s, args) =>
            {
                idleTimer.Stop();
                idleTimer.Start();
            };

            idleTimer.Start();

            chromiumWebBrowser.Paint += handler;

            return renderIdleTcs.Task;
        }
    }
}
