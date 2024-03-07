using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Other target.
    /// </summary>
    public class OtherTarget : Target
    {
        internal OtherTarget(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> createSession,
            TaskQueue screenshotTaskQueue) : base(targetInfo, session, context, targetManager, createSession, screenshotTaskQueue)
        {
        }
    }
}
