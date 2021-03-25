using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class LaunchOptionsTests
    {
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
