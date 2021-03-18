using PuppeteerSharp.Tests.Attributes;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue0648 : PuppeteerPageBaseTest
    {
        public Issue0648(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync("https://www.google.com/search?q=firewall&oq=firewall&ie=UTF-8");
        }
    }
}
