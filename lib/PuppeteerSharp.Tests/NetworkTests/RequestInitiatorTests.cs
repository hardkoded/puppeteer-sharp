using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestInitiatorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.initiator", "should return the initiator")]
        public async Task ShouldReturnTheInitiator()
        {
            var initiators = new Dictionary<string, Initiator>();
            Page.Request += (_, e) =>
            {
                initiators[e.Request.Url.Split('/').Last()] = e.Request.Initiator;
            };
            await Page.GoToAsync(TestConstants.ServerUrl + "/initiator.html");

            Assert.That(initiators["initiator.html"].Type, Is.EqualTo(InitiatorType.Other));
            Assert.That(initiators["initiator.js"].Type, Is.EqualTo(InitiatorType.Parser));
            Assert.That(initiators["initiator.js"].Url, Is.EqualTo(TestConstants.ServerUrl + "/initiator.html"));
            Assert.That(initiators["frame.html"].Type, Is.EqualTo(InitiatorType.Parser));
            Assert.That(initiators["frame.html"].Url, Is.EqualTo(TestConstants.ServerUrl + "/initiator.html"));
            Assert.That(initiators["script.js"].Type, Is.EqualTo(InitiatorType.Parser));
            Assert.That(initiators["script.js"].Url, Is.EqualTo(TestConstants.ServerUrl + "/frames/frame.html"));
            Assert.That(initiators["style.css"].Type, Is.EqualTo(InitiatorType.Parser));
            Assert.That(initiators["style.css"].Url, Is.EqualTo(TestConstants.ServerUrl + "/frames/frame.html"));
            Assert.That(initiators["injectedfile.js"].Type, Is.EqualTo(InitiatorType.Script));
            Assert.That(initiators["injectedfile.js"].Stack.CallFrames[0].Url, Is.EqualTo(TestConstants.ServerUrl + "/initiator.js"));
            Assert.That(initiators["injectedstyle.css"].Type, Is.EqualTo(InitiatorType.Script));
            Assert.That(initiators["injectedstyle.css"].Stack.CallFrames[0].Url, Is.EqualTo(TestConstants.ServerUrl + "/initiator.js"));
        }
    }
}
