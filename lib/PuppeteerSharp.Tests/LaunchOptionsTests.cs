using PuppeteerSharp.Tests.Attributes;
using Xunit;

namespace PuppeteerSharp.Tests
{
    public class LaunchOptionsTests
    {
        [PuppeteerFact]
        public void DisableHeadlessWhenDevtoolsEnabled()
        {
            var options = new LaunchOptions
            {
                Devtools = true
            };

            Assert.True(options.Devtools);
            Assert.False(options.Headless);
        }
    }
}
