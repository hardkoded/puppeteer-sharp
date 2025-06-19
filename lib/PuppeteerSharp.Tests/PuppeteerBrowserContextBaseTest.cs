using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserContextBaseTest : PuppeteerBrowserBaseTest
    {
        protected IBrowserContext Context { get; set; }

        [SetUp]
        public async Task CreateContextAsync()
        {
            Context = await Browser.CreateBrowserContextAsync();
        }
    }
}
