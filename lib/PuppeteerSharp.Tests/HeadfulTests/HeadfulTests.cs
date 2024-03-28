using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.HeadfulTests
{
    public class HeadfulTests : PuppeteerBaseTest
    {
        [Test, Retry(2), PuppeteerTest("headful.spec", "headful tests HEADFUL", "headless should be able to read cookies written by headful")]
        public async Task HeadlessShouldBeAbleToReadCookiesWrittenByHeadful()
        {
            using var userDataDir = new TempDirectory();
            var launcher = new Launcher(TestConstants.LoggerFactory);
            var options = TestConstants.DefaultBrowserOptions();
            options.UserDataDir = userDataDir.Path;
            options.Headless = false;
            await using (var headfulBrowser = await launcher.LaunchAsync(options))
            await using (var headfulPage = await headfulBrowser.NewPageAsync())
            {
                await headfulPage.GoToAsync(TestConstants.EmptyPage);
                await headfulPage.EvaluateExpressionAsync(
                    "document.cookie = 'foo=true; expires=Fri, 31 Dec 9999 23:59:59 GMT'");
            }

            await TestUtils.WaitForCookieInChromiumFileAsync(userDataDir.Path, "foo");

            options.Headless = true;
            await using (var headlessBrowser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory))
            {
                var headlessPage = await headlessBrowser.NewPageAsync();
                await headlessPage.GoToAsync(TestConstants.EmptyPage);
                Assert.AreEqual("foo=true", await headlessPage.EvaluateExpressionAsync<string>("document.cookie"));
            }
        }
    }
}
