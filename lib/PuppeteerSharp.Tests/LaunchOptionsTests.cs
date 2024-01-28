using NUnit.Framework;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests
{
    public class LaunchOptionsTests
    {
        [PuppeteerTimeout]
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
