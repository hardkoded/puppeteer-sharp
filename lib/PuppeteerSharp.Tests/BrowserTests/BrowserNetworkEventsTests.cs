using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserNetworkEventsTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should report responses from pages")]
        public async Task ShouldReportResponsesFromPages()
        {
            var responseTcs = new TaskCompletionSource<IResponse>();
            void OnResponse(object sender, ResponseCreatedEventArgs e)
            {
                if (e.Response.Url == TestConstants.EmptyPage)
                {
                    responseTcs.TrySetResult(e.Response);
                }
            }

            Browser.Response += OnResponse;
            try
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
                var response = await responseTcs.Task;

                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Request.Frame.Page, Is.EqualTo(Page));
                Assert.That(response.Request.Frame, Is.EqualTo(Page.MainFrame));
            }
            finally
            {
                Browser.Response -= OnResponse;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should report finished requests from pages")]
        public async Task ShouldReportFinishedRequestsFromPages()
        {
            var requestTcs = new TaskCompletionSource<IRequest>();
            void OnRequestFinished(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.RequestFinished += OnRequestFinished;
            try
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
                var request = await requestTcs.Task;

                Assert.That(request.Frame.Page, Is.EqualTo(Page));
                Assert.That(request.Response, Is.Not.Null);
            }
            finally
            {
                Browser.RequestFinished -= OnRequestFinished;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should report failed requests from pages")]
        public async Task ShouldReportFailedRequestsFromPages()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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

            var requestTcs = new TaskCompletionSource<IRequest>();
            void OnRequestFailed(object sender, RequestEventArgs e)
            {
                if (e.Request.Url.EndsWith("one-style.css"))
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.RequestFailed += OnRequestFailed;
            try
            {
                await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
                var request = await requestTcs.Task;

                Assert.That(request.Frame.Page, Is.EqualTo(Page));
                Assert.That(request.Response, Is.Null);
                Assert.That(request.FailureText, Is.Not.Null.And.Not.Empty);
            }
            finally
            {
                Browser.RequestFailed -= OnRequestFailed;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should report requests served from cache")]
        public async Task ShouldReportRequestsServedFromCache()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-script.html");
            await Task.Delay(1000);

            var requestTcs = new TaskCompletionSource<IRequest>();
            void OnCached(object sender, RequestEventArgs e)
            {
                if (e.Request.Url.EndsWith("one-script.js"))
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.RequestServedFromCache += OnCached;
            try
            {
                await Page.ReloadAsync();
                var request = await requestTcs.Task;

                Assert.That(request.Frame.Page, Is.EqualTo(Page));
            }
            finally
            {
                Browser.RequestServedFromCache -= OnCached;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should emit context events before browser events")]
        public async Task ShouldEmitContextEventsBeforeBrowserEvents()
        {
            var order = new Dictionary<string, List<string>>
            {
                ["response"] = new List<string>(),
                ["requestfinished"] = new List<string>(),
            };
            var finishedTcs = new TaskCompletionSource<bool>();

            void RecordResponse(string source, IResponse response)
            {
                if (response.Url == TestConstants.EmptyPage)
                {
                    order["response"].Add(source);
                }
            }

            void RecordFinished(string source, IRequest request)
            {
                if (request.Url == TestConstants.EmptyPage)
                {
                    order["requestfinished"].Add(source);
                }
            }

            void OnContextResponse(object sender, ResponseCreatedEventArgs e) => RecordResponse("context", e.Response);
            void OnBrowserResponse(object sender, ResponseCreatedEventArgs e) => RecordResponse("browser", e.Response);
            void OnContextFinished(object sender, RequestEventArgs e) => RecordFinished("context", e.Request);
            void OnBrowserFinished(object sender, RequestEventArgs e)
            {
                RecordFinished("browser", e.Request);
                finishedTcs.TrySetResult(true);
            }

            Page.BrowserContext.Response += OnContextResponse;
            Browser.Response += OnBrowserResponse;
            Page.BrowserContext.RequestFinished += OnContextFinished;
            Browser.RequestFinished += OnBrowserFinished;
            try
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
                await finishedTcs.Task;

                // Only the order across emitters is checked: the relative order of
                // response and requestfinished is not guaranteed.
                Assert.That(order["response"], Is.EqualTo(new[] { "context", "browser" }));
                Assert.That(order["requestfinished"], Is.EqualTo(new[] { "context", "browser" }));
            }
            finally
            {
                Page.BrowserContext.Response -= OnContextResponse;
                Browser.Response -= OnBrowserResponse;
                Page.BrowserContext.RequestFinished -= OnContextFinished;
                Browser.RequestFinished -= OnBrowserFinished;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should emit each event once per page event")]
        public async Task ShouldEmitEachEventOncePerPageEvent()
        {
            Server.SetRedirect("/foo.html", "/empty.html");
            var pageSeen = new List<string>();
            var browserSeen = new List<string>();

            void OnPageResponse(object sender, ResponseCreatedEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    pageSeen.Add($"response:{e.Response.Url}");
                }
            }

            void OnPageFinished(object sender, RequestEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    pageSeen.Add($"requestfinished:{e.Request.Url}");
                }
            }

            void OnBrowserResponse(object sender, ResponseCreatedEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    browserSeen.Add($"response:{e.Response.Url}");
                }
            }

            void OnBrowserFinished(object sender, RequestEventArgs e)
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    browserSeen.Add($"requestfinished:{e.Request.Url}");
                }
            }

            Page.Response += OnPageResponse;
            Page.RequestFinished += OnPageFinished;
            Browser.Response += OnBrowserResponse;
            Browser.RequestFinished += OnBrowserFinished;
            try
            {
                await Page.GoToAsync(TestConstants.ServerUrl + "/foo.html");

                Assert.That(pageSeen.Count, Is.GreaterThanOrEqualTo(3));
                Assert.That(browserSeen, Is.EqualTo(pageSeen));
            }
            finally
            {
                Page.Response -= OnPageResponse;
                Page.RequestFinished -= OnPageFinished;
                Browser.Response -= OnBrowserResponse;
                Browser.RequestFinished -= OnBrowserFinished;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser network events", "should report responses from dedicated workers")]
        public async Task ShouldReportResponsesFromDedicatedWorkers()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            var worker = await workerCreatedTcs.Task.WithTimeout();
            var url = TestConstants.ServerUrl + "/one-style.css";

            var responseTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnResponse(object sender, ResponseCreatedEventArgs e)
            {
                if (e.Response.Url == url)
                {
                    responseTcs.TrySetResult(e.Response);
                }
            }

            void OnRequestFinished(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == url)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.Response += OnResponse;
            Browser.RequestFinished += OnRequestFinished;
            try
            {
                await worker.EvaluateFunctionAsync("async (url) => { await (await fetch(url)).text(); }", url);
                var response = await responseTcs.Task.WithTimeout();
                var request = await requestTcs.Task.WithTimeout();

                Assert.That(response.Request.Worker, Is.SameAs(worker));
                Assert.That(request.Worker, Is.SameAs(worker));
                Assert.That(request.Page, Is.Null);
            }
            finally
            {
                Browser.Response -= OnResponse;
                Browser.RequestFinished -= OnRequestFinished;
            }
        }
    }
}
