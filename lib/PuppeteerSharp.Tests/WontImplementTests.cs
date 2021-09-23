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
        [PuppeteerTest("navigation.spec.ts", "should not leak listeners during navigation")]
        [PuppeteerTest("navigation.spec.ts", "should not leak listeners during bad navigation")]
        [PuppeteerTest("navigation.spec.ts", "should not leak listeners during navigation of 11 pages")]
        [PuppeteerTest("navigation.spec.ts", "should throw if networkidle is passed as an option")]
        [PuppeteerTest("launcher.spec.ts", "Puppeteer.launch", "should report the correct product")] //We don't use the product in this way
        [PuppeteerTest("launcher.spec.ts", "Puppeteer.launch", "falls back to launching chrome if there is an unknown product but logs a warning")]
        [PuppeteerTest("devtoolstracing.spec.ts", "Tracing", "should return null in case of Buffer error")]
        [PuppeteerTest("devtoolstracing.spec.ts", "Tracing", "should properly fail if readProtocolStream errors out")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public void TheseTesstWontBeImplemented()
        {
        }
    }
}
