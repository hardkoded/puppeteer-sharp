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
            Page.DefaultTimeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultPuppeteerTimeout;
        }

        [TearDown]
        public Task ClosePageAsync() => Page.CloseAsync();

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
    }
}
