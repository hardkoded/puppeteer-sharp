using System;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WontImplementTests : PuppeteerPageBaseTest
    {
        public WontImplementTests(ITestOutputHelper output) : base(output)
        {
        }

        // We don't implement pipes
        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |pipe| option", "should support the pipe option")]
        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |pipe| option", "should support the pipe argument")]
        [PuppeteerTest("chromiumonly.spec.ts", "Puppeteer.launch |pipe| option", "should fire \"disconnected\" when closing with pipe")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public void TheseTesstWontBeImplemented()
        {
        }
    }
}
