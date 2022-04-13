using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue1878 : PuppeteerBrowserContextBaseTest
    {
        public Issue1878(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task MultiplePagesShouldNotShareSameScreenshotTaskQueue()
        {
            // 1st page
            using (Page page = await Context.NewPageAsync())
            {
                var html = "...some html..";
                await page.GoToAsync($"data:text/html,{html}", new NavigationOptions
                {
                    WaitUntil = new[]
                    {
                        WaitUntilNavigation.DOMContentLoaded,
                        WaitUntilNavigation.Load,
                        WaitUntilNavigation.Networkidle0,
                        WaitUntilNavigation.Networkidle2
                    }
                });

                await page.ScreenshotAsync("...some path...");
            }

            //2nd page
            using (Page page = await Context.NewPageAsync())
            {
                var html = "...some html...";
                await page.GoToAsync($"data:text/html,{html}", new NavigationOptions
                {
                    WaitUntil = new[]
                    {
                        WaitUntilNavigation.DOMContentLoaded,
                        WaitUntilNavigation.Load,
                        WaitUntilNavigation.Networkidle0,
                        WaitUntilNavigation.Networkidle2
                    }
                });

                await page.ScreenshotAsync("...some path..."); //<- will throw because Browser TaskQueue is disposed when 1st Page is disposed
            }
        }
    }
}
