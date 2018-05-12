using PuppeteerSharp.TestServer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBaseTest : IDisposable
    {
        protected string BaseDirectory { get; set; }
        protected Browser Browser { get; set; }

        protected SimpleServer Server => PuppeteerLoaderFixture.Server;
        protected SimpleServer HttpsServer => PuppeteerLoaderFixture.HttpsServer;

        protected virtual async Task InitializeAsync()
        {
            Browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions(), TestConstants.ChromiumRevision);
            Server.Reset();
            HttpsServer.Reset();
        }

        protected virtual async Task DisposeAsync() => await Browser.CloseAsync();

        public PuppeteerBaseTest()
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            InitializeAsync().GetAwaiter().GetResult();
        }

        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();

        protected static Task<dynamic> WaitForEvents(Session emitter, string eventName, int eventCount = 1)
        {
            var completion = new TaskCompletionSource<dynamic>();
            void handler(object sender, MessageEventArgs e)
            {
                if (e.MessageID != eventName) return;

                --eventCount;
                if (eventCount > 0) return;

                emitter.MessageReceived -= handler;
                completion.SetResult(e.MessageData);
            };

            emitter.MessageReceived += handler;

            return completion.Task;
        }
    }
}