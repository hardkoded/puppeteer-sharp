using CefSharp.OffScreen;
using CefSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Xunit;
using PuppeteerSharp.TestServer;

namespace PuppeteerSharp.Tests
{
    public class DevToolsContextLoaderFixture : IDisposable, IAsyncLifetime
    {
        private readonly AsyncContextThread contextThread;

        public static SimpleServer Server { get; private set; }
        public static SimpleServer HttpsServer { get; private set; }

        public DevToolsContextLoaderFixture()
        {
            contextThread = new AsyncContextThread();

            //TODO: Improve this
            SetupAsync().GetAwaiter().GetResult();
        }

        private void InitializeAsyncInternal()
        {
            if (Cef.IsInitialized == null)
            {
                var isDefault = AppDomain.CurrentDomain.IsDefaultAppDomain();
                if (!isDefault)
                {
                    throw new Exception(@"Appdomains must be disabled. See https://xunit.net/docs/configuration-files#appDomain");
                }

                Cef.EnableWaitForBrowsersToClose();

                CefSharpSettings.ShutdownOnExit = false;
                var settings = new CefSettings();

                //The location where cache data will be stored on disk. If empty an in-memory cache will be used for some features and a temporary disk cache for others.
                //HTML5 databases such as localStorage will only persist across sessions if a cache path is specified. 
                settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Tests\\Cache");
                settings.RootCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Tests");
                //settings.CefCommandLineArgs.Add("renderer-startup-dialog");
                settings.CefCommandLineArgs.Add("disable-request-handling-for-testing");

                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            }

            if(Cef.IsInitialized == false)
            {
                var exitCode = Cef.GetExitCode();
                throw new Exception($"CEF failed with exit code {exitCode}");
            }
        }

        private void DisposeAsyncInternal()
        {
            if (Cef.IsInitialized == true)
            {
                Cef.Shutdown();
            }
        }

        public Task InitializeAsync()
        {
            return contextThread.Factory.StartNew(InitializeAsyncInternal);
        }

        public Task DisposeAsync()
        {
            return contextThread.Factory.StartNew(DisposeAsyncInternal);
        }

        public void Dispose()
        {
            contextThread.Dispose();

            Task.WaitAll(Server.StopAsync(), HttpsServer.StopAsync());
        }

        private async Task SetupAsync()
        {
            Server = SimpleServer.Create(TestConstants.Port, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));
            HttpsServer = SimpleServer.CreateHttps(TestConstants.HttpsPort, TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            var serverStart = Server.StartAsync();
            var httpsServerStart = HttpsServer.StartAsync();

            await Task.WhenAll(serverStart, httpsServerStart);
        }
    }
}
