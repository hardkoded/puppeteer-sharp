using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserRequestTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"request\")", "should report requests from pages")]
        public async Task ShouldReportRequestsFromPages()
        {
            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnRequest(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.Request += OnRequest;
            try
            {
                await Page.GoToAsync(TestConstants.EmptyPage);
                var request = await requestTcs.Task.WithTimeout();

                Assert.That(request.Page, Is.EqualTo(Page));
                Assert.That(request.Frame, Is.EqualTo(Page.MainFrame));
            }
            finally
            {
                Browser.Request -= OnRequest;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"request\")", "should emit context events before browser events")]
        public async Task ShouldEmitContextEventsBeforeBrowserEvents()
        {
            var order = new List<string>();

            void OnBrowserRequest(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    order.Add("browser");
                }
            }

            void OnContextRequest(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    order.Add("context");
                }
            }

            Browser.Request += OnBrowserRequest;
            Context.Request += OnContextRequest;
            try
            {
                await Page.GoToAsync(TestConstants.EmptyPage);

                Assert.That(order, Is.EqualTo(new[] { "context", "browser" }));
            }
            finally
            {
                Browser.Request -= OnBrowserRequest;
                Context.Request -= OnContextRequest;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"request\")", "should report requests from dedicated workers")]
        public async Task ShouldReportRequestsFromDedicatedWorkers()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            var worker = await workerCreatedTcs.Task.WithTimeout();

            var url = TestConstants.ServerUrl + "/one-style.css";
            var requestTcs = new TaskCompletionSource<IRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnRequest(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == url)
                {
                    requestTcs.TrySetResult(e.Request);
                }
            }

            Browser.Request += OnRequest;
            try
            {
                await worker.EvaluateFunctionAsync("url => fetch(url)", url);
                var request = await requestTcs.Task.WithTimeout();

                Assert.That(request.Worker, Is.SameAs(worker));
                Assert.That(request.Page, Is.Null);
            }
            finally
            {
                Browser.Request -= OnRequest;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"request\")", "should not interfere with request interception")]
        public async Task ShouldNotInterfereWithRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += (_, e) => _ = e.Request.ContinueAsync();

            var requests = new List<IRequest>();

            void OnRequest(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == TestConstants.EmptyPage)
                {
                    requests.Add(e.Request);
                }
            }

            Browser.Request += OnRequest;
            try
            {
                var response = await Page.GoToAsync(TestConstants.EmptyPage);

                Assert.That(response.Ok, Is.True);
                Assert.That(requests, Has.Exactly(1).Items);
                Assert.That(requests[0].Page, Is.EqualTo(Page));
                Assert.That(requests[0].IsInterceptResolutionHandled, Is.True);
            }
            finally
            {
                Browser.Request -= OnRequest;
            }
        }
    }
}
