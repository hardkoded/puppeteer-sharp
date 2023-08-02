using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.Issues
{
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
