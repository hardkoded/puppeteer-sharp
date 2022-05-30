using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
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
        [PuppeteerTest("tracing.spec.ts", "Tracing", "should return null in case of Buffer error")]
        [PuppeteerTest("tracing.spec.ts", "Tracing", "should properly fail if readProtocolStream errors out")]
        [PuppeteerTest("fixtures.spec.ts", "Fixtures", "dumpio option should work with pipe option")]
        [PuppeteerTest("EventEmitter.spec.ts", "once", "only calls the listener once and then removes it")]
        [PuppeteerTest("EventEmitter.spec.ts", "once", "supports chaining")]
        [PuppeteerTest("EventEmitter.spec.ts", "emit", "calls all the listeners for an event")]
        [PuppeteerTest("EventEmitter.spec.ts", "emit", "passes data through to the listener")]
        [PuppeteerTest("EventEmitter.spec.ts", "emit", "returns true if the event has listeners")]
        [PuppeteerTest("EventEmitter.spec.ts", "emit", "returns false if the event has listeners")]
        [PuppeteerTest("EventEmitter.spec.ts", "listenerCount", "returns the number of listeners for the given event")]
        [PuppeteerTest("EventEmitter.spec.ts", "removeAllListeners", "removes every listener from all events by default")]
        [PuppeteerTest("EventEmitter.spec.ts", "removeAllListeners", "returns the emitter for chaining")]
        [PuppeteerTest("EventEmitter.spec.ts", "removeAllListeners", "can filter to remove only listeners for a given event name")]
        [PuppeteerTest("emulation.spec.ts", "Page.emulateMediaType", "should throw in case of bad argument")]
        [PuppeteerTest("emulation.spec.ts", "Page.emulateMediaFeatures", "should throw in case of bad argument")]
        [PuppeteerTest("emulation.spec.ts", "Page.emulateVisionDeficiency", "should throw for invalid vision deficiencies")]
        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should throw when unknown type")]
        [PuppeteerTest("waittask.spec.ts", "Page.waitFor", "should log a deprecation warning")]
        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should accept a string")]
        [PuppeteerTest("waittask.spec.ts", "Frame.waitForFunction", "should throw on bad polling value")]
        [PuppeteerTest("network.spec.ts", "Page.setExtraHTTPHeaders", "should throw for non-string header values")]
        [PuppeteerTest("page.spec.ts", "removing and adding event handlers", "should correctly fire event handlers as they are added and then removed")]
        [PuppeteerTest("page.spec.ts", "removing and adding event handlers", "should correctly added and removed request events")]
        [PuppeteerTest("page.spec.ts", "BrowserContext.overridePermissions", "should fail when bad permission is given")]
        [PuppeteerTest("page.spec.ts", "Page.select", "should throw if passed in non-strings")]
        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should support throwing \"null\"")]
        [PuppeteerTest("page.spec.ts", "Page.exposeFunction", "should work with function shorthands")]
        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should wait correctly with waitFor")]
        [PuppeteerFact]
        public void TheseTesstWontBeImplemented()
        {
        }
    }
}
