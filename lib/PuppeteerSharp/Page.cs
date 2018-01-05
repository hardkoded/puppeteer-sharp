using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Page
    {
        private Session _session;
        private bool _ignoreHTTPSErrors;
        private bool _appMode;
        private NetworkManager _networkManager;
        private FrameManager _frameManager;

        private Page(Session session, FrameTree frameTree, bool ignoreHTTPSErrors, bool appMode)
        {
            _session = session;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _appMode = appMode;
            _frameManager = new FrameManager(session, frameTree, this);

            this._networkManager = new NetworkManager(session);
        }

        internal static async Task<Page> CreateAsync(Session session, bool ignoreHTTPSErrors, bool appMode)
        {
            return null;
        }

        public async Task<dynamic> GoToAsync(string url, Dictionary<string, string> options)
        {
            var referrer = _networkManager.ExtraHTTPHeaders["referer"];
            var requests = new Dictionary<string, Request>();

            Action<object, RequestEventArgs> createRequestEventListener = (object sender, RequestEventArgs e) =>
                requests.Add(e.Request.Url, e.Request);

            _networkManager.RequestCreated += new EventHandler<RequestEventArgs>(createRequestEventListener);

            var mainFrame = _frameManager.MainFrame();
            var watcher = new NavigationWatcher(_frameManager, mainFrame, options);

            var navigateTask = Navigate(_session, url, referrer);

            var error = Task.WaitAny(
                navigateTask,
                watcher.NavigationTask
            );


            /*
           
            const navigationPromise = watcher.navigationPromise();
            let error = await Promise.race([
              navigate(this._client, url, referrer),
              navigationPromise,
            ]);
            if (!error)
              error = await navigationPromise;
            watcher.cancel();
            helper.removeEventListeners(eventListeners);
            if (error)
              throw error;
            const request = requests.get(this.mainFrame().url());
            return request ? request.response() : null;


                    async function navigate(client, url, referrer)
                    {
                        try
                        {
                            const response = await client.send('Page.navigate', { url, referrer});
                    return response.errorText ? new Error(response.errorText) : null;
                } catch (error) {
                return error;
              }
            }
         */
        }

        private Task Navigate(Session session, string url, string referrer)
        {
            throw new NotImplementedException();
        }
    }
}
