using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerPageBaseTest : PuppeteerBrowserContextBaseTest
    {
        protected IPage Page { get; set; }

        [SetUp]
        public async Task CreatePageAsync()
        {
            Page = await Context.NewPageAsync();
            Page.DefaultTimeout = Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultPuppeteerTimeout;
        }

        [TearDown]
        public async Task ClosePageAsync()
        {
            if (Page is not null)
            {
                await Page.CloseAsync();
            }
        }

        protected Task WaitForError()
        {
            var wrapper = new TaskCompletionSource<bool>();

            void ErrorEvent(object sender, ErrorEventArgs e)
            {
                wrapper.SetResult(true);
                Page.Error -= ErrorEvent;
            }

            Page.Error += ErrorEvent;

            return wrapper.Task;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        protected async Task<ITarget> FindPopupTargetAsync(IPage excludePage)
        {
            var targets = excludePage.BrowserContext.Targets();
            var pageTargets = targets.Where(t => t.Type == TargetType.Page).ToArray();

            foreach (var target in pageTargets)
            {
                var targetPage = await target.PageAsync();
                if (targetPage != null && targetPage != excludePage)
                {
                    return target;
                }
            }

            return null;
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
