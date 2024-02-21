using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0616 : PuppeteerPageBaseTest
    {
        public Issue0616() : base()
        {
        }

        public async Task ShouldBeAbleToChangeToPost()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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

            Assert.AreEqual(TestConstants.ServerUrl + "/grid.html", response.Url);
            Assert.AreEqual("foo", await response.TextAsync());
        }
    }
}
