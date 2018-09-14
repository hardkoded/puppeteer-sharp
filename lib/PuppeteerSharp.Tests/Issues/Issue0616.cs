using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0616 : PuppeteerPageBaseTest
    {
        public Issue0616(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldBeAbleToChangeToPost()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                var payload = new Payload()
                {
                    Method = HttpMethod.Post,
                    PostData = "foo"
                };
                await e.Request.ContinueAsync(payload);
            };

            var requestTask = Server.WaitForRequest<string>("/grid.html", request =>
            {
                return request.Form.ToString();
            });
            var gotoTask = Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            await Task.WhenAll(
                requestTask,
                gotoTask
            );

            Assert.Equal("http://google.com/", requestTask.Result);
        }
    }
}