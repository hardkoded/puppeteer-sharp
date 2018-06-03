using PuppeteerSharp.TestServer;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBaseTest
    {
        protected string BaseDirectory { get; set; }

        protected SimpleServer Server => PuppeteerLoaderFixture.Server;
        protected SimpleServer HttpsServer => PuppeteerLoaderFixture.HttpsServer;

        public PuppeteerBaseTest(ITestOutputHelper output)
        {
            TestConstants.SetupLogging(output);

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

        protected static Task<dynamic> WaitForEvents(CDPSession emitter, string eventName, int eventCount = 1)
        {
            var completion = new TaskCompletionSource<dynamic>();
            void handler(object sender, MessageEventArgs e)
            {
                if (e.MessageID != eventName)
                {
                    return;
                }

                --eventCount;
                if (eventCount > 0)
                {
                    return;
                }

                emitter.MessageReceived -= handler;
                completion.SetResult(e.MessageData);
            }

            emitter.MessageReceived += handler;

            return completion.Task;
        }
    }
}