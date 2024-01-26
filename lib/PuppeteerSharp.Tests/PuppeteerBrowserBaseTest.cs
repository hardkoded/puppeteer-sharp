using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerBrowserBaseTest : PuppeteerBaseTest, IDisposable, IAsyncDisposable
    {
        protected IBrowser Browser { get; set; }

        protected LaunchOptions DefaultOptions { get; set; }

        [SetUp]
        public virtual async Task InitializeAsync()
            => Browser = await Puppeteer.LaunchAsync(
                DefaultOptions ?? TestConstants.DefaultBrowserOptions(),
                TestConstants.LoggerFactory);

        [TearDown]
        public virtual async Task DisposeAsync() => await Browser.CloseAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => _ = DisposeAsync();

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return Browser.DisposeAsync();
        }
    }
}
