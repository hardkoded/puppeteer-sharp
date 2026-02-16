using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp
{
    /// <summary>
    /// DevTools target.
    /// </summary>
    public class CdpDevToolsTarget : CdpPageTarget
    {
        internal CdpDevToolsTarget(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory,
            ViewPortOptions defaultViewport,
            TaskQueue screenshotTaskQueue)
            : base(targetInfo, session, context, targetManager, sessionFactory, defaultViewport, screenshotTaskQueue)
        {
        }

        /// <inheritdoc/>
        public override TargetType Type => TargetType.Other;
    }
}
