using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests
{
    public class WontImplementTests : PuppeteerPageBaseTest
    {
        // We don't implement pipes
        [PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |pipe| option", "should support the pipe option")]
        [PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |pipe| option", "should support the pipe argument")]
        [PuppeteerTest("chromiumonly.spec", "Puppeteer.launch |pipe| option", "should fire \"disconnected\" when closing with pipe")]
        [PuppeteerTest("navigation.spec", "Page.goto", "should not leak listeners during navigation")]
        [PuppeteerTest("navigation.spec", "Page.goto", "should not leak listeners during bad navigation")]
        [PuppeteerTest("navigation.spec", "Page.goto", "should not leak listeners during navigation of 11 pages")]
        [PuppeteerTest("navigation.spec", "Page.goto", "should throw if networkidle is passed as an option")]
        [PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "should report the correct product")] //We don't use the product in this way
        [PuppeteerTest("launcher.spec", "Launcher specs Puppeteer Puppeteer.launch", "falls back to launching chrome if there is an unknown product but logs a warning")]
        [PuppeteerTest("tracing.spec", "Tracing", "should return null in case of Buffer error")]
        [PuppeteerTest("tracing.spec", "Tracing", "should properly fail if readProtocolStream errors out")]
        [PuppeteerTest("fixtures.spec", "Fixtures", "dumpio option should work with pipe option")]
        [PuppeteerTest("EventEmitter.spec", "once", "only calls the listener once and then removes it")]
        [PuppeteerTest("EventEmitter.spec", "once", "supports chaining")]
        [PuppeteerTest("EventEmitter.spec", "emit", "calls all the listeners for an event")]
        [PuppeteerTest("EventEmitter.spec", "emit", "passes data through to the listener")]
        [PuppeteerTest("EventEmitter.spec", "emit", "returns true if the event has listeners")]
        [PuppeteerTest("EventEmitter.spec", "emit", "returns false if the event has listeners")]
        [PuppeteerTest("EventEmitter.spec", "listenerCount", "returns the number of listeners for the given event")]
        [PuppeteerTest("EventEmitter.spec", "removeAllListeners", "removes every listener from all events by default")]
        [PuppeteerTest("EventEmitter.spec", "removeAllListeners", "returns the emitter for chaining")]
        [PuppeteerTest("EventEmitter.spec", "removeAllListeners", "can filter to remove only listeners for a given event name")]
        [PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should work with function shorthands")]
        [PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaType", "should throw in case of bad argument")]
        [PuppeteerTest("emulation.spec", "Emulation Page.emulateMediaFeatures", "should throw in case of bad argument")]
        [PuppeteerTest("emulation.spec", "Emulation Page.emulateVisionDeficiency", "should throw for invalid vision deficiencies")]
        [PuppeteerTest("waittask.spec", "waittask specs Page.waitFor", "should throw when unknown type")]
        [PuppeteerTest("waittask.spec", "waittask specs Page.waitFor", "should log a deprecation warning")]
        [PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should accept a string")]
        [PuppeteerTest("waittask.spec", "waittask specs Frame.waitForFunction", "should throw on bad polling value")]
        [PuppeteerTest("network.spec", "network Page.setExtraHTTPHeaders", "should throw for non-string header values")]
        [PuppeteerTest("page.spec", "Page removing and adding event handlers", "should correctly fire event handlers as they are added and then removed")]
        [PuppeteerTest("page.spec", "Page removing and adding event handlers", "should correctly added and removed request events")]
        [PuppeteerTest("page.spec", "Page BrowserContext.overridePermissions", "should fail when bad permission is given")]
        [PuppeteerTest("page.spec", "Page Page.select", "should throw if passed in non-strings")]
        [PuppeteerTest("page.spec", "Page Page.exposeFunction", "should support throwing \"null\"")]
        [PuppeteerTest("page.spec", "Page Page.exposeFunction", "should work with function shorthands")]
        [PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should wait correctly with waitFor")]
        [PuppeteerTest("tracing.spec", "Tracing", "should return undefined in case of Buffer error")]
        [PuppeteerTest("DeviceRequestPrompt.test.ts", "waitForDevicePrompt", "should listen and shortcut when there are no watchdogs")]
        [PuppeteerTest("DeviceRequestPrompt.test.ts", "DeviceRequestPrompt.waitForDevice", "should listen and shortcut when there are no watchdogs")]
        public void TheseTestWontBeImplemented()
        {
        }
    }
}
