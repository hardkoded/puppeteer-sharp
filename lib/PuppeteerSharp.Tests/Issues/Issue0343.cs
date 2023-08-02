using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0343 : PuppeteerPageBaseTest
    {
        public Issue0343(): base()
        {
        }

        [Fact(Skip = "It seems something's changed in chromium and this is no longer valid")]
        public async Task ShouldSupportLongExpiresValueInCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var longExpiresValue = 3677410981.1125112d;
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456",
                Expires = longExpiresValue
            });
            var cookies = await Page.GetCookiesAsync();
            Assert.Equal(longExpiresValue, cookies.First(c => c.Name == "password").Expires);
        }
    }
}