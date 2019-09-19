using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue0343 : PuppeteerPageBaseTest
    {
        public Issue0343(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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