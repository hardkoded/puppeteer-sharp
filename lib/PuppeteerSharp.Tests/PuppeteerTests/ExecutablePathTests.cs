using System.IO;
using Xunit;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ExecutablePathTests
    {
        [Fact]
        public void ShouldWork()
        {
            var executablePath = Puppeteer.GetExecutablePath();
            Assert.True(File.Exists(executablePath));
            Assert.Equal(new FileInfo(executablePath).FullName, executablePath);
        }
    }
}
