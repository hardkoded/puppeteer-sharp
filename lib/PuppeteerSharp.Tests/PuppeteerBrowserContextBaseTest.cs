using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserContextBaseTest : PuppeteerBrowserBaseTest
    {
        public PuppeteerBrowserContextBaseTest(): base()
        {
        }

        protected IBrowserContext Context { get; set; }
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Context = await Browser.CreateIncognitoBrowserContextAsync();
        }
    }
}
