using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    public class FrameWaitForSelectorTests : PuppeteerPageBaseTest
    {
        private const string AddElement = "tag => document.body.appendChild(document.createElement(tag))";

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should immediately resolve promise if node exists")]
        [PuppeteerTimeout]
        public async Task ShouldImmediatelyResolveTaskIfNodeExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            await frame.WaitForSelectorAsync("*");
            await frame.EvaluateFunctionAsync(AddElement, "div");
            await frame.WaitForSelectorAsync("div");
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should work with removed MutationObserver")]
        public async Task ShouldWorkWithRemovedMutationObserver()
        {
            await Page.EvaluateExpressionAsync("delete window.MutationObserver");
            var waitForSelector = Page.WaitForSelectorAsync(".zombo");

            await Task.WhenAll(
                waitForSelector,
                Page.SetContentAsync("<div class='zombo'>anything</div>"));

            Assert.AreEqual("anything", await Page.EvaluateFunctionAsync<string>("x => x.textContent", await waitForSelector));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should resolve promise when node is added")]
        [PuppeteerTimeout]
        public async Task ShouldResolveTaskWhenNodeIsAdded()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var watchdog = frame.WaitForSelectorAsync("div");
            await frame.EvaluateFunctionAsync(AddElement, "br");
            await frame.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await watchdog;
            var property = await eHandle.GetPropertyAsync("tagName");
            var tagName = await property.JsonValueAsync<string>();
            Assert.AreEqual("DIV", tagName);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should work when node is added through innerHTML")]
        [PuppeteerTimeout]
        public async Task ShouldWorkWhenNodeIsAddedThroughInnerHtml()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var watchdog = Page.WaitForSelectorAsync("h3 div");
            await Page.EvaluateFunctionAsync(AddElement, "span");
            await Page.EvaluateExpressionAsync("document.querySelector('span').innerHTML = '<h3><div></div></h3>'");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "Page.waitForSelector is shortcut for main frame")]
        [PuppeteerTimeout]
        public async Task PageWaitForSelectorAsyncIsShortcutForMainFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var otherFrame = Page.FirstChildFrame();
            var watchdog = Page.WaitForSelectorAsync("div");
            await otherFrame.EvaluateFunctionAsync(AddElement, "div");
            await Page.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await watchdog;
            Assert.AreEqual(Page.MainFrame, eHandle.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should run in specified frame")]
        [PuppeteerTimeout]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.FirstChildFrame();
            var frame2 = Page.Frames.ElementAt(2);
            var waitForSelectorPromise = frame2.WaitForSelectorAsync("div");
            await frame1.EvaluateFunctionAsync(AddElement, "div");
            await frame2.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await waitForSelectorPromise;
            Assert.AreEqual(frame2, eHandle.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should throw when frame is detached")]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.FirstChildFrame();
            var waitTask = frame.WaitForSelectorAsync(".box");
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var waitException = Assert.ThrowsAsync<WaitTaskTimeoutException>(() => waitTask);

            Assert.NotNull(waitException);
            StringAssert.Contains("waitForFunction failed: frame got detached.", waitException.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should survive cross-process navigation")]
        [PuppeteerTimeout]
        public async Task ShouldSurviveCrossProcessNavigation()
        {
            var boxFound = false;
            var waitForSelector = Page.WaitForSelectorAsync(".box").ContinueWith(_ => boxFound = true);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(boxFound);
            await Page.ReloadAsync();
            Assert.False(boxFound);
            await Page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/grid.html");
            await waitForSelector;
            Assert.True(boxFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be visible (display)")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForVisibleDisplay()
        {
            var divFound = false;
            var waitForSelector = Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div style='display: none;'>text</div>");
            await Task.Delay(100);
            Assert.False(divFound);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('display')");
            Assert.True(await waitForSelector);
            Assert.True(divFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be visible (visibility)")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForVisibleVisibility()
        {
            var divFound = false;
            var waitForSelector = Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div style='visibility: hidden;'>text</div>");
            await Task.Delay(100);
            Assert.False(divFound);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.setProperty('visibility', 'collapse')");
            await Task.Delay(100);
            Assert.False(divFound);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('visibility')");
            Assert.True(await waitForSelector);
            Assert.True(divFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be visible (bounding box)")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForVisibleBoundingBox()
        {
            var divFound = false;
            var _ = Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div style='width: 0;'>text</div>");
            await Task.Delay(100);
            Assert.False(divFound);
            await Page.EvaluateFunctionAsync(@"() => {
                const div = document.querySelector('div');
                div.style.setProperty('height', '0');
                div.style.removeProperty('width');
            }");
            await Task.Delay(100);
            Assert.False(divFound);
            await Page.EvaluateFunctionAsync(@"() => {
                const div = document.querySelector('div');
                div.style.removeProperty('height');
            }");
            await Task.Delay(100);
            Assert.True(divFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for visible recursively")]
        [PuppeteerTimeout]
        public async Task ShouldWaitForVisibleRecursively()
        {
            var divVisible = false;
            var waitForSelector = Page.WaitForSelectorAsync("div#inner", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divVisible = true);
            await Page.SetContentAsync("<div style='display: none; visibility: hidden;'><div id='inner'>hi</div></div>");
            Assert.False(divVisible);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('display')");
            Assert.False(divVisible);
            await Page.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('visibility')");
            Assert.True(await waitForSelector);
            Assert.True(divVisible);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be hidden (visibility)")]
        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be hidden (display)")]
        [TestCase("visibility", "hidden")]
        [TestCase("display", "none")]
        public async Task HiddenShouldWaitForVisibility(string propertyName, string propertyValue)
        {
            var divHidden = false;
            await Page.SetContentAsync("<div style='display: block;'>text</div>");
            var waitForSelector = Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await Page.WaitForSelectorAsync("div"); // do a round trip
            Assert.False(divHidden);
            await Page.EvaluateExpressionAsync($"document.querySelector('div').style.setProperty('{propertyName}', '{propertyValue}')");
            Assert.True(await waitForSelector);
            Assert.True(divHidden);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for element to be hidden (removal) ")]
        [PuppeteerTimeout]
        public async Task HiddenShouldWaitForRemoval()
        {
            await Page.SetContentAsync("<div>text</div>");
            var divRemoved = false;
            var waitForSelector = Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divRemoved = true);
            await Page.WaitForSelectorAsync("div"); // do a round trip
            Assert.False(divRemoved);
            await Page.EvaluateExpressionAsync("document.querySelector('div').remove()");
            Assert.True(await waitForSelector);
            Assert.True(divRemoved);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should return null if waiting to hide non-existing element")]
        [PuppeteerTimeout]
        public async Task ShouldReturnNullIfWaitingToHideNonExistingElement()
        {
            var handle = await Page.WaitForSelectorAsync("non-existing", new WaitForSelectorOptions { Hidden = true });
            Assert.Null(handle);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should respect timeout")]
        [PuppeteerTimeout]
        public void ShouldRespectTimeout()
        {
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Timeout = 10 }));

            StringAssert.Contains("Waiting for selector `div` failed: Waiting failed: 10ms exceeded", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should have an error message specifically for awaiting an element to be hidden")]
        [PuppeteerTimeout]
        public async Task ShouldHaveAnErrorMessageSpecificallyForAwaitingAnElementToBeHidden()
        {
            await Page.SetContentAsync("<div>text</div>");
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await Page.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true, Timeout = 10 }));

            StringAssert.Contains("Waiting for selector `div` failed: Waiting failed: 10ms exceeded", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should respond to node attribute mutation")]
        [PuppeteerTimeout]
        public async Task ShouldRespondToNodeAttributeMutation()
        {
            var divFound = false;
            var waitForSelector = Page.WaitForSelectorAsync(".zombo").ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div class='notZombo'></div>");
            Assert.False(divFound);
            await Page.EvaluateExpressionAsync("document.querySelector('div').className = 'zombo'");
            Assert.True(await waitForSelector);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should return the element handle")]
        [PuppeteerTimeout]
        public async Task ShouldReturnTheElementHandle()
        {
            var waitForSelector = Page.WaitForSelectorAsync(".zombo");
            await Page.SetContentAsync("<div class='zombo'>anything</div>");
            Assert.AreEqual("anything", await Page.EvaluateFunctionAsync<string>("x => x.textContent", await waitForSelector));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should have correct stack trace for timeout")]
        [PuppeteerTimeout]
        public void ShouldHaveCorrectStackTraceForTimeout()
        {
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await Page.WaitForSelectorAsync(".zombo", new WaitForSelectorOptions { Timeout = 10 }));
            StringAssert.Contains("WaitForSelectorTests", exception.StackTrace);
        }
    }
}
