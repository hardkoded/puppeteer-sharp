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

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
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

            Server.SetRoute("/grid.html", async (context) =>
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    var request = await reader.ReadToEndAsync();
                    await context.Response.WriteAsync(request);
                }
            });
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            Assert.Equal(TestConstants.ServerUrl + "/grid.html", response.Url);
            Assert.Equal("foo", await response.TextAsync());
        }
    }
}