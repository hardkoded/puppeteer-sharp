using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class OtherTarget : Target
    {
        public OtherTarget(TargetInfo targetInfo, CDPSession session, BrowserContext context, ITargetManager targetManager, Func<bool, Task<CDPSession>> createSession) : base(targetInfo, session, context, targetManager, createSession)
        {
        }
    }
}
