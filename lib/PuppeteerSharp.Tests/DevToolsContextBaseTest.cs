using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using CefSharp.DevTools.Dom;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class DevToolsContextBaseTest : PuppeteerBaseTest, IAsyncLifetime
    {
        private readonly bool _ignoreHTTPSerrors;
        private readonly string _initialUrl;

        public DevToolsContextBaseTest(ITestOutputHelper output, bool ignoreHTTPSerrors = true, string initialUrl = TestConstants.ServerIpUrl) : base(output)
        {
            _ignoreHTTPSerrors = ignoreHTTPSerrors;
            _initialUrl = initialUrl;
        }

        protected DevToolsContext DevToolsContext { get; set; }
        protected ChromiumWebBrowser ChromiumWebBrowser { get; private set; }

        public async Task InitializeAsync()
        {
            var requestContext = new RequestContext();
            ChromiumWebBrowser = new ChromiumWebBrowser(_initialUrl, requestContext : requestContext);

            await ChromiumWebBrowser.WaitForInitialLoadAsync();

            var loggerFactory = new LoggerFactory().AddFile("Logs/tests-{Date}.txt", LogLevel.Trace);

            DevToolsContext = await ChromiumWebBrowser.CreateDevToolsContextAsync(_ignoreHTTPSerrors, loggerFactory).ConfigureAwait(false);
            DevToolsContext.DefaultTimeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultPuppeteerTimeout;

            await DevToolsContext.SetViewportAsync(ViewPortOptions.Default).ConfigureAwait(false);
            
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
