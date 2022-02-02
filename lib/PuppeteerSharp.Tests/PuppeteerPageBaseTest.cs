using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Puppeteer;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerPageBaseTest : PuppeteerBaseTest, IAsyncLifetime
    {
        private readonly bool _ignoreHTTPSerrors;

        public PuppeteerPageBaseTest(ITestOutputHelper output, bool ignoreHTTPSerrors = true) : base(output)
        {
            _ignoreHTTPSerrors = ignoreHTTPSerrors;
        }

        protected DevToolsContext DevToolsContext { get; set; }
        protected ChromiumWebBrowser ChromiumWebBrowser { get; private set; }

        public async Task InitializeAsync()
        {
            var requestContext = new RequestContext();
            ChromiumWebBrowser = new ChromiumWebBrowser(TestConstants.ServerIpUrl, requestContext : requestContext);

            await ChromiumWebBrowser.WaitForInitialLoadAsync();

            var loggerFactory = new LoggerFactory().AddFile("Logs/tests-{Date}.txt", LogLevel.Trace);

            DevToolsContext = await ChromiumWebBrowser.GetDevToolsContextAsync(_ignoreHTTPSerrors, loggerFactory);;
            DevToolsContext.DefaultTimeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultPuppeteerTimeout;
        }

        public virtual Task DisposeAsync()
        {
            ChromiumWebBrowser.Dispose();

            return Task.CompletedTask;            
        }

        protected Task WaitForError()
        {
            var wrapper = new TaskCompletionSource<bool>(TaskContinuationOptions.RunContinuationsAsynchronously);

            void errorEvent(object sender, ErrorEventArgs e)
            {
                wrapper.TrySetResult(true);
                DevToolsContext.Error -= errorEvent;
            }

            DevToolsContext.Error += errorEvent;

            return wrapper.Task;
        }
    }
}
