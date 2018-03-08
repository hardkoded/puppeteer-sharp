using System.IO;
using Xunit;

namespace PuppeteerSharp.Tests.Puppeteer
{
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
