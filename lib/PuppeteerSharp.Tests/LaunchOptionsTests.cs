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

            Assert.That(options.Devtools, Is.True);
            Assert.That(options.Headless, Is.False);
        }
    }
}
