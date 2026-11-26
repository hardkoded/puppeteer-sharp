using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class CdpJSHandleTests
    {
        [
            Test,
            PuppeteerTest(
                "JSHandle.test.ts",
                "CdpJSHandle",
                "should throw if getProperties() is called on a disposed handle"
            )
        ]
        public async Task ShouldThrowIfGetPropertiesIsCalledOnADisposedHandle()
        {
            var client = Substitute.For<ICDPSession>();
            var worker = Substitute.For<WebWorker>(string.Empty);
            worker.Client.Returns(client);
            await using var world = new IsolatedWorld(null, worker, new TimeoutSettings(), false);

            var handle = new CdpJSHandle(
                world,
                new RemoteObject { Type = RemoteObjectType.Object, ObjectId = "obj-1" }
            );

            await handle.DisposeAsync();
            Assert.That(handle.Disposed, Is.True);

            var exception = Assert.ThrowsAsync<PuppeteerException>(() =>
                handle.GetPropertiesAsync()
            );
            Assert.That(exception.Message, Is.EqualTo("Attempted to use disposed CdpJSHandle."));
        }
    }
}
