using System;
using System.Threading.Tasks;

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
            ViewPortOptions defaultViewport
        )
            : base(targetInfo, session, context, targetManager, sessionFactory, defaultViewport) { }
    }
}
