using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class PageSetRequestInterceptionInternalRedirectsTests : PuppeteerBaseTest
    {
        // @see https://github.com/puppeteer/puppeteer/issues/12183
        [Test, PuppeteerTest("network.spec", "network Page.setRequestInterception", "should work with internal redirects not paused by Fetch.requestPaused")]
        public async Task ShouldWorkWithInternalRedirectsNotPausedByFetchRequestPaused()
        {
            var certPath = Path.Combine(TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"), "testCert.cer");
            using var cert = X509CertificateLoader.LoadCertificateFromFile(certPath);
            var spkiHash = Convert.ToBase64String(SHA256.HashData(cert.PublicKey.ExportSubjectPublicKeyInfo()));

            // Chrome ignores `Strict-Transport-Security` headers on connections with
            // certificate errors, even when `acceptInsecureCerts: true` or
            // `--ignore-certificate-errors` is used. Passing the test server cert's
            // SPKI hash via `--ignore-certificate-errors-spki-list` makes Chrome
            // treat the certificate as trusted without errors so HSTS is registered.
            var options = TestConstants.DefaultBrowserOptions();
            options.Args =
            [
                "--host-resolver-rules=\"MAP domain1.test 127.0.0.1,MAP domain2.test 127.0.0.1,MAP domain3.test 127.0.0.1\"",
                $"--ignore-certificate-errors-spki-list={spkiHash}",
            ];
            await using var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();

            HttpsServer.SetRoute("/hsts", context =>
            {
                context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
                return context.Response.WriteAsync("ok");
            });
            var httpsOrigin = $"https://domain1.test:{HttpsServer.Port}";
            var httpEmptyPage = $"http://domain1.test:{HttpsServer.Port}/empty.html";
            var httpsEmptyPage = $"https://domain1.test:{HttpsServer.Port}/empty.html";
            HttpsServer.SetRedirect("/start", httpEmptyPage);

            await page.GoToAsync(httpsOrigin + "/hsts");

            var requests = new List<IRequest>();
            var responses = new List<int>();
            await page.SetRequestInterceptionAsync(true);
            page.Request += async (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }

                await e.Request.ContinueAsync();
            };
            page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    responses.Add((int)e.Response.Status);
                }
            };

            var response = await page.GoToAsync(httpsOrigin + "/start");

            Assert.That(response.Status, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(requests.Select(r => r.Url), Is.EqualTo(new[] { httpsOrigin + "/start", httpEmptyPage, httpsEmptyPage }));
            Assert.That(responses, Is.EqualTo(new[] { 302, 307, 200 }));
            Assert.That(response.Request.RedirectChain, Is.EqualTo(new[] { requests[0], requests[1] }));
        }
    }
}
