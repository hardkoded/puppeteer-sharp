using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Page
    {
        private Session _session;
        private bool _ignoreHTTPSErrors;
        private bool _appMode;

        private Page(Session session, bool ignoreHTTPSErrors, bool appMode)
        {
            _session = session;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _appMode = appMode;
        }

        internal static async Task<Page> CreateAsync(Session session, bool ignoreHTTPSErrors, bool appMode)
        {
            var page = new Page(session, ignoreHTTPSErrors, appMode);
            await session.SendAsync("Page.enable", null);
            return page;
        } 

        public async Task<dynamic> GoToAsync(string url)
        {
            throw new NotImplementedException();
        }
    }
}
