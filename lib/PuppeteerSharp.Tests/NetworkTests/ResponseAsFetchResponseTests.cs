using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseAsFetchResponseTests : PuppeteerPageBaseTest
    {
        public ResponseAsFetchResponseTests() : base()
        {
        }

        [Test, PuppeteerTest("HTTPResponse.test.ts", "HTTPResponse asFetchResponse", "should convert to a standard Fetch Response")]
        public async Task ShouldConvertToStandardFetchResponse()
        {
            Server.SetRoute("/fetch-target", context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("hello world");
            });

            var response = await FetchFromPageAsync("/fetch-target");
            using var fetchResponse = await response.AsFetchResponseAsync();

            Assert.That(fetchResponse, Is.InstanceOf<HttpResponseMessage>());
            Assert.That(fetchResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(fetchResponse.ReasonPhrase, Is.EqualTo("OK"));
            Assert.That(fetchResponse.Content.Headers.ContentType.MediaType, Is.EqualTo("text/plain"));
            Assert.That(await fetchResponse.Content.ReadAsStringAsync(), Is.EqualTo("hello world"));
        }

        [Test, PuppeteerTest("HTTPResponse.test.ts", "HTTPResponse asFetchResponse", "should properly parse multi-line set-cookie headers")]
        public async Task ShouldParseMultiLineSetCookieHeaders()
        {
            Server.SetRoute("/fetch-target", context =>
            {
                context.Response.Headers.Append("Set-Cookie", "session=xyz; Secure");
                context.Response.Headers.Append("Set-Cookie", "theme=dark");
                return context.Response.WriteAsync("ok");
            });

            var response = await FetchFromPageAsync("/fetch-target");
            using var fetchResponse = await response.AsFetchResponseAsync();

            Assert.That(fetchResponse.Headers.GetValues("Set-Cookie").ToArray(), Is.EqualTo(new[] { "session=xyz; Secure", "theme=dark" }));
        }

        [Test, PuppeteerTest("HTTPResponse.test.ts", "HTTPResponse asFetchResponse", "should handle null body status codes without body")]
        public async Task ShouldHandleNullBodyStatusCodesWithoutBody()
        {
            foreach (var status in new[] { 204, 205, 304 })
            {
                var path = $"/fetch-target-{status}";
                Server.SetRoute(path, context =>
                {
                    context.Response.StatusCode = status;
                    return context.Response.WriteAsync("should not be included");
                });

                var response = await FetchFromPageAsync(path);
                using var fetchResponse = await response.AsFetchResponseAsync();

                Assert.That(fetchResponse.StatusCode, Is.EqualTo((HttpStatusCode)status));
                Assert.That(await fetchResponse.Content.ReadAsByteArrayAsync(), Is.Empty);
            }
        }

        private async Task<IResponse> FetchFromPageAsync(string path)
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var responseTask = Page.WaitForResponseAsync(r => r.Url.EndsWith(path));
            await Task.WhenAll(responseTask, Page.EvaluateExpressionAsync($"fetch('{path}')"));
            return await responseTask;
        }
    }
}
