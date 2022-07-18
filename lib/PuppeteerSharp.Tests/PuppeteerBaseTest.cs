using Newtonsoft.Json.Linq;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.TestServer;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBaseTest
    {
        protected string BaseDirectory { get; set; }

        protected SimpleServer Server => DevToolsContextLoaderFixture.Server;
        protected SimpleServer HttpsServer => DevToolsContextLoaderFixture.HttpsServer;

        public PuppeteerBaseTest(ITestOutputHelper output)
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Initialize();
        }

        protected void Initialize()
        {
            Server.Reset();
            HttpsServer.Reset();
        }

        protected static Task<JToken> WaitEvent(DevToolsConnection emitter, string eventName)
        {
            var completion = new TaskCompletionSource<JToken>();
            void handler(object sender, MessageEventArgs e)
            {
                if (e.MessageID != eventName)
                {
                    return;
                }
                emitter.MessageReceived -= handler;
                completion.SetResult(e.MessageData);
            }

            emitter.MessageReceived += handler;
            return completion.Task;
        }

        protected static Task WaitForBrowserDisconnect(DevToolsConnection browser)
        {
            var disconnectedTask = new TaskCompletionSource<bool>();
            browser.Disconnected += (_, _) => disconnectedTask.TrySetResult(true);
            return disconnectedTask.Task;
        }
    }
}
