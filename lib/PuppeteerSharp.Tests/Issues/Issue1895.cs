using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue1895 : PuppeteerPageBaseTest
    {
        public Issue1895(): base()
        {
        }

        [PuppeteerFact]
        public async Task ItNavigatesToSannySoft()
        {
            await Page.GoToAsync("https://bot.sannysoft.com/");
        }
    }
}
