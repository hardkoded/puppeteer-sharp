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

        private Page(Session session, bool ignoreHTTPSErrors, bool appMode)
        {
            _session = session;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _appMode = appMode;

            this._networkManager = new NetworkManager(session);
        }

        internal static async Task<Page> CreateAsync(Session session, bool ignoreHTTPSErrors, bool appMode)
        {
            var page = new Page(session, ignoreHTTPSErrors, appMode);
            await session.SendAsync("Page.enable", null);
            return page;
        }

        public async Task<dynamic> GoToAsync(string url)
        {
            var referrer = _networkManager.ExtraHTTPHeaders["referer"];
            var requests = new Dictionary<string, Request>();

            /*
            const eventListeners = [
              helper.addEventListener(this._networkManager, NetworkManager.Events.Request, request => requests.set(request.url, request))
            ];

            const mainFrame = this._frameManager.mainFrame();
            const watcher = new NavigatorWatcher(this._frameManager, mainFrame, options);
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
             * */
        }
    }
}
