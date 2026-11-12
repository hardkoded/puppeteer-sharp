using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class BrowserContextNetworkEventsTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browsercontext.spec", "BrowserContext BrowserContext network events", "should only report network events from its own pages")]
        public async Task ShouldOnlyReportNetworkEventsFromItsOwnPages()
        {
            await using var otherContext = await Browser.CreateBrowserContextAsync();
            var otherPage = await otherContext.NewPageAsync();

            var seen = new Dictionary<string, List<IPage>>
            {
                ["response"] = new List<IPage>(),
                ["requestfinished"] = new List<IPage>(),
                ["requestfailed"] = new List<IPage>(),
                ["requestservedfromcache"] = new List<IPage>(),
            };

            void OnResponse(object sender, ResponseCreatedEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    seen["response"].Add(e.Response.Request.Frame?.Page);
                }
            }

            void OnFinished(object sender, RequestEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    seen["requestfinished"].Add(e.Request.Frame?.Page);
                }
            }

            void OnFailed(object sender, RequestEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    seen["requestfailed"].Add(e.Request.Frame?.Page);
                }
            }

            void OnCached(object sender, RequestEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    seen["requestservedfromcache"].Add(e.Request.Frame?.Page);
                }
            }

            Context.Response += OnResponse;
            Context.RequestFinished += OnFinished;
            Context.RequestFailed += OnFailed;
            Context.RequestServedFromCache += OnCached;

            try
            {
                await TriggerNetworkEventsAsync(otherPage);
                await TriggerNetworkEventsAsync(Page);

                foreach (var (eventName, pages) in seen)
                {
                    Assert.That(pages.Contains(Page), Is.True, $"{eventName} should be reported for the context page");
                    Assert.That(pages.Count(p => p != Page), Is.EqualTo(0), $"{eventName} should not be reported for pages of other contexts");
                }
            }
            finally
            {
                Context.Response -= OnResponse;
                Context.RequestFinished -= OnFinished;
                Context.RequestFailed -= OnFailed;
                Context.RequestServedFromCache -= OnCached;
            }
        }

        // Triggers response, requestfinished, requestfailed and requestservedfromcache on the given page.
        private async Task TriggerNetworkEventsAsync(IPage target)
        {
            await target.SetRequestInterceptionAsync(true);
            EventHandler<RequestEventArgs> onRequest = async (_, e) =>
            {
                if (e.Request.Url.EndsWith("css"))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };

            target.Request += onRequest;
            await target.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            target.Request -= onRequest;
            await target.SetRequestInterceptionAsync(false);

            await target.GoToAsync(TestConstants.ServerUrl + "/cached/one-script.html");
            await Task.Delay(1000);
            await target.ReloadAsync();
        }
    }
}
