using System.IO;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ExecutablePathTests
    {
        [Fact]
        public void ShouldWork()
        {
            var executablePath = PuppeteerSharp.Puppeteer.GetExecutablePath();
            Assert.True(File.Exists(executablePath));
        }
    }
}
