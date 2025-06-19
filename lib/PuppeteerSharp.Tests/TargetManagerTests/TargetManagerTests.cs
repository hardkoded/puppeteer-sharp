using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TargetManagerTests
{
    public class TargetManagerTests : PuppeteerBrowserContextBaseTest
    {
        public TargetManagerTests() : base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.Args = new[]
            {
                "--site-per-process",
                "--remote-debugging-port=21222",
                "--host-rules=\"MAP * 127.0.0.1\"",
            };
        }

        [Test, PuppeteerTest("TargetManager.spec", "TargetManager", "should handle targets")]
        public async Task ShouldHandleTargets()
        {
            var targetManager = (Browser as CdpBrowser)!.TargetManager;
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(2));

            Assert.That(await Context.PagesAsync(), Is.Empty);
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(2));

            var page = await Context.NewPageAsync();
            Assert.That((await Context.PagesAsync()), Has.Length.EqualTo(1));
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(3));

            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.That((await Context.PagesAsync()), Has.Length.EqualTo(1));
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(3));

            var frameTask = page.WaitForFrameAsync(target => target.Url == TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(page, "frame1", TestConstants.EmptyPage);
            await frameTask.WithTimeout();
            Assert.That((await Context.PagesAsync()), Has.Length.EqualTo(1));
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(3));
            Assert.That(page.Frames, Has.Length.EqualTo(2));

            frameTask = page.WaitForFrameAsync(target => target.Url == TestConstants.CrossProcessUrl + "/empty.html");
            await FrameUtils.AttachFrameAsync(page, "frame2", TestConstants.CrossProcessUrl + "/empty.html");
            await frameTask.WithTimeout();
            Assert.That((await Context.PagesAsync()), Has.Length.EqualTo(1));
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(4));
            Assert.That(page.Frames, Has.Length.EqualTo(3));

            frameTask = page.WaitForFrameAsync(target => target.Url == TestConstants.CrossProcessUrl + "/empty.html");
            await FrameUtils.AttachFrameAsync(page, "frame3", TestConstants.CrossProcessUrl + "/empty.html");
            await frameTask.WithTimeout();
            Assert.That((await Context.PagesAsync()), Has.Length.EqualTo(1));
            Assert.That(targetManager.GetAvailableTargets().Values, Has.Count.EqualTo(5));
            Assert.That(page.Frames, Has.Length.EqualTo(4));
        }
    }
}
