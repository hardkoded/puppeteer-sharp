using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public abstract class PuppeteerBrowserContextBaseTest : PuppeteerBrowserBaseTest
    {
        protected IBrowserContext Context { get; set; }

        [SetUp]
        public async Task CreateContextAsync()
        {
            Context = await Browser.CreateBrowserContextAsync();
        }
    }
}
