using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.TestServer;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBaseTest
    {
        protected string BaseDirectory { get; set; }

        protected SimpleServer Server => TestServerSetup.Server;

        protected SimpleServer HttpsServer => TestServerSetup.HttpsServer;

        [SetUp]
        public void SetUp()
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Server.Reset();
            HttpsServer.Reset();
        }

        protected static Task<JToken> WaitEvent(ICDPSession emitter, string eventName)
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

        protected static Task WaitForBrowserDisconnect(IBrowser browser)
        {
            var disconnectedTask = new TaskCompletionSource<bool>();
            browser.Disconnected += (_, _) => disconnectedTask.TrySetResult(true);
            return disconnectedTask.Task;
        }
    }
}
