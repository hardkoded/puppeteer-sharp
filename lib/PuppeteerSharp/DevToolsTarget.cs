using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// DevTools target.
    /// </summary>
    public class DevToolsTarget : PageTarget
    {
        internal DevToolsTarget(TargetInfo targetInfo, CDPSession session, BrowserContext context, ITargetManager targetManager, Func<bool, Task<CDPSession>> createSession) : base(targetInfo, session, context, targetManager, createSession)
        {
        }
    }
}
