using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue1447 : PuppeteerPageBaseTest
    {
        public Issue1447(ITestOutputHelper output) : base(output)
        {
            
        }

        [Fact]
        public void Example()
        {
            PuppeteerSharp.LaunchOptions opts = new PuppeteerSharp.LaunchOptions()
            {
                Headless = false,
                ExecutablePath = @"C:\Users\user\AppData\Local\Chromium\Application\chrome.exe",
                DefaultViewport = null,
                UserDataDir = @"d:\32323"
            };

            opts.IgnoredDefaultArgs = new string[] { "--enable-automation" };

            PuppeteerSharp.Launcher launcher = new PuppeteerSharp.Launcher();

            var browser = launcher.LaunchAsync(opts).GetAwaiter().GetResult();

            var pages = browser.PagesAsync().GetAwaiter().GetResult();

            var page = pages.ElementAt(0);

            for (int i = 0; i < 20; i++)
            {
                Navigate(page, "https://distilnetworks.com");
                Navigate(page, "https://mail.com");
                Navigate(page, "https://distilnetworks.com");
                Navigate(page, "https://vk.com");
                Navigate(page, "https://distilnetworks.com");
                Navigate(page, "https://mail.com");
                Navigate(page, "https://distilnetworks.com");
                Navigate(page, "https://mail.com");
                Navigate(page, "about:blank");
            }
        }

        public void Navigate(PuppeteerSharp.Page page, string url)
        {
            page.MainFrame.GoToAsync(url, new PuppeteerSharp.NavigationOptions() { Timeout = 0, WaitUntil = new PuppeteerSharp.WaitUntilNavigation[] { PuppeteerSharp.WaitUntilNavigation.DOMContentLoaded } }).GetAwaiter().GetResult();
        }
    }
}
