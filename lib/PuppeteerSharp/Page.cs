using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Page
    {
        private Session _client;
        private bool _ignoreHTTPSErrors;
        private NetworkManager _networkManager;
        private FrameManager _frameManager;
        private TaskQueue _screenshotTaskQueue;
        private EmulationManager _emulationManager;
        private ViewPortOptions _viewport;

        private Page(Session client, FrameTree frameTree, bool ignoreHTTPSErrors, TaskQueue screenshotTaskQueue)
        {
            _client = client;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _frameManager = new FrameManager(_client, frameTree, this);
            _screenshotTaskQueue = screenshotTaskQueue;
            _emulationManager = new EmulationManager(client);

            _networkManager = new NetworkManager(client);
        }

        internal static async Task<Page> CreateAsync(Session client, bool ignoreHTTPSErrors, bool appMode,
                                                     TaskQueue screenshotTaskQueue)
        {
            await client.SendAsync("Page.enable", null);
            dynamic frameTree = await client.SendAsync<FrameTree>("Page.getFrameTree", null);
            var page = new Page(client, frameTree, ignoreHTTPSErrors, screenshotTaskQueue);

            await Task.WhenAll(
                client.SendAsync("Page.setLifecycleEventsEnabled", new Dictionary<string, object>
                {
                    {"enabled", true }
                }),
                client.SendAsync("Network.enable", null),
                client.SendAsync("Runtime.enable", null),
                client.SendAsync("Security.enable", null),
                client.SendAsync("Performance.enable", null)
            );

            if (ignoreHTTPSErrors)
            {
                await client.SendAsync("Security.setOverrideCertificateErrors", new Dictionary<string, object>
                {
                    {"override", true}
                });
            }

            // Initialize default page size.
            if (!appMode)
            {
                await page.SetViewport(new ViewPortOptions
                {
                    Width = 800,
                    Height = 600
                });
            }
            return page;
        }

        private async Task SetViewport(ViewPortOptions viewport)
        {
            var needsReload = await _emulationManager.EmulateViewport(_client, viewport);
            _viewport = viewport;
            if (needsReload)
            {
                await Reload();
            }
        }

        private Task Reload()
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GoToAsync(string url, Dictionary<string, string> options)
        {
            var referrer = _networkManager.ExtraHTTPHeaders["referer"];
            var requests = new Dictionary<string, Request>();

            EventHandler<RequestEventArgs> createRequestEventListener = (object sender, RequestEventArgs e) =>
                requests.Add(e.Request.Url, e.Request);

            _networkManager.RequestCreated += createRequestEventListener;

            var mainFrame = _frameManager.MainFrame();
            var watcher = new NavigationWatcher(_frameManager, mainFrame, options);

            var navigateTask = Navigate(_client, url, referrer);

            await Task.WhenAll(
                navigateTask,
                watcher.NavigationTask
            );

            var error = !string.IsNullOrEmpty(navigateTask.Result) ? navigateTask.Result : watcher.NavigationTask.Result;

            watcher.Cancel();
            _networkManager.RequestCreated -= createRequestEventListener;

            if (!string.IsNullOrEmpty(error))
            {
                throw new NavigationException(error);
            }

            Request request = null;

            if (requests.ContainsKey(_frameManager.MainFrame().Url))
            {
                request = requests[_frameManager.MainFrame().Url];
            }

            return request?.Response;
        }

        private async Task<string> Navigate(Session client, string url, string referrer)
        {
            try
            {
                dynamic response = await client.SendAsync("Page.navigate", new Dictionary<string, object>
                {
                    { "url", url},
                    {"referrer", referrer}
                });

                return response.ErrorText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
