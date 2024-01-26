using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// DevTools target.
    /// </summary>
    public class DevToolsTarget : PageTarget
    {
        internal DevToolsTarget(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewport,
            TaskQueue screenshotTaskQueue)
            : base(targetInfo, session, context, targetManager, sessionFactory, ignoreHTTPSErrors, defaultViewport, screenshotTaskQueue)
        {
        }
    }
}
