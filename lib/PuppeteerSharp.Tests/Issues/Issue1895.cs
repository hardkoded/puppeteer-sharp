using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue1895 : PuppeteerPageBaseTest
    {
        public Issue1895() : base()
        {
        }

        public async Task ItNavigatesToSannySoft()
        {
            await Page.GoToAsync("https://bot.sannysoft.com/");
        }
    }
}
