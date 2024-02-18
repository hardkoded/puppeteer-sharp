using NUnit.Framework;

namespace PuppeteerSharp.Tests
{
    public class LaunchOptionsTests
    {
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
