using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0648 : PuppeteerPageBaseTest
    {
        public Issue0648() : base()
        {
        }

        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync("https://www.google.com/search?q=firewall&oq=firewall&ie=UTF-8");
        }
    }
}
