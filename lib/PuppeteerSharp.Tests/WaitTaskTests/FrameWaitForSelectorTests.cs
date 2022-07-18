using System.Linq;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.WaitTaskTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForSelectorTests : DevToolsContextBaseTest
    {
        const string AddElement = "tag => document.body.appendChild(document.createElement(tag))";

        public FrameWaitForSelectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should immediately resolve promise if node exists")]
        [PuppeteerFact]
        public async Task ShouldImmediatelyResolveTaskIfNodeExists()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var frame = DevToolsContext.MainFrame;
            await frame.WaitForSelectorAsync("*");
            await frame.EvaluateFunctionAsync(AddElement, "div");
            await frame.WaitForSelectorAsync("div");
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should work with removed MutationObserver")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRemovedMutationObserver()
        {
            await DevToolsContext.EvaluateExpressionAsync("delete window.MutationObserver");
            var waitForSelector = DevToolsContext.WaitForSelectorAsync(".zombo");

            await Task.WhenAll(
                waitForSelector,
                DevToolsContext.SetContentAsync("<div class='zombo'>anything</div>"));

            Assert.Equal("anything", await DevToolsContext.EvaluateFunctionAsync<string>("x => x.textContent", await waitForSelector));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should resolve promise when node is added")]
        [PuppeteerFact]
        public async Task ShouldResolveTaskWhenNodeIsAdded()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var frame = DevToolsContext.MainFrame;
            var watchdog = frame.WaitForSelectorAsync("div");
            await frame.EvaluateFunctionAsync(AddElement, "br");
            await frame.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await watchdog;
            var property = await eHandle.GetPropertyAsync("tagName");
            var tagName = await property.JsonValueAsync<string>();
            Assert.Equal("DIV", tagName);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should work when node is added through innerHTML")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenNodeIsAddedThroughInnerHTML()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var watchdog = DevToolsContext.WaitForSelectorAsync("h3 div");
            await DevToolsContext.EvaluateFunctionAsync(AddElement, "span");
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('span').innerHTML = '<h3><div></div></h3>'");
            await watchdog;
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "Page.waitForSelector is shortcut for main frame")]
        [PuppeteerFact]
        public async Task PageWaitForSelectorAsyncIsShortcutForMainFrame()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            var otherFrame = DevToolsContext.FirstChildFrame();
            var watchdog = DevToolsContext.WaitForSelectorAsync("div");
            await otherFrame.EvaluateFunctionAsync(AddElement, "div");
            await DevToolsContext.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await watchdog;
            Assert.Equal(DevToolsContext.MainFrame, eHandle.ExecutionContext.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should run in specified frame")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame2", TestConstants.EmptyPage);
            var frame1 = DevToolsContext.FirstChildFrame();
            var frame2 = DevToolsContext.Frames.ElementAt(2);
            var waitForSelectorPromise = frame2.WaitForSelectorAsync("div");
            await frame1.EvaluateFunctionAsync(AddElement, "div");
            await frame2.EvaluateFunctionAsync(AddElement, "div");
            var eHandle = await waitForSelectorPromise;
            Assert.Equal(frame2, eHandle.ExecutionContext.Frame);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should throw when frame is detached")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            var frame = DevToolsContext.FirstChildFrame();
            var waitTask = frame.WaitForSelectorAsync(".box").ContinueWith(task => task?.Exception?.InnerException);
            await FrameUtils.DetachFrameAsync(DevToolsContext, "frame1");
            var waitException = await waitTask;
            Assert.NotNull(waitException);
            Assert.Contains("waitForFunction failed: frame got detached.", waitException.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should survive cross-process navigation")]
        [PuppeteerFact]
        public async Task ShouldSurviveCrossProcessNavigation()
        {
            var boxFound = false;
            var waitForSelector = DevToolsContext.WaitForSelectorAsync(".box").ContinueWith(_ => boxFound = true);
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.False(boxFound);
            await DevToolsContext.ReloadAsync();
            Assert.False(boxFound);
            await DevToolsContext.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/grid.html");
            await waitForSelector;
            Assert.True(boxFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for visible")]
        [PuppeteerFact]
        public async Task ShouldWaitForVisible()
        {
            var divFound = false;
            var waitForSelector = DevToolsContext.WaitForSelectorAsync("div", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divFound = true);
            await DevToolsContext.SetContentAsync("<div style='display: none; visibility: hidden;'>1</div>");
            Assert.False(divFound);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('display')");
            Assert.False(divFound);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('visibility')");
            Assert.True(await waitForSelector);
            Assert.True(divFound);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should wait for visible recursively")]
        [PuppeteerFact]
        public async Task ShouldWaitForVisibleRecursively()
        {
            var divVisible = false;
            var waitForSelector = DevToolsContext.WaitForSelectorAsync("div#inner", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divVisible = true);
            await DevToolsContext.SetContentAsync("<div style='display: none; visibility: hidden;'><div id='inner'>hi</div></div>");
            Assert.False(divVisible);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('display')");
            Assert.False(divVisible);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').style.removeProperty('visibility')");
            Assert.True(await waitForSelector);
            Assert.True(divVisible);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "hidden should wait for visibility: hidden")]
        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "hidden should wait for display: none")]
        [Theory]
        [InlineData("visibility", "hidden")]
        [InlineData("display", "none")]
        public async Task HiddenShouldWaitForVisibility(string propertyName, string propertyValue)
        {
            var divHidden = false;
            await DevToolsContext.SetContentAsync("<div style='display: block;'></div>");
            var waitForSelector = DevToolsContext.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await DevToolsContext.WaitForSelectorAsync("div"); // do a round trip
            Assert.False(divHidden);
            await DevToolsContext.EvaluateExpressionAsync($"document.querySelector('div').style.setProperty('{propertyName}', '{propertyValue}')");
            Assert.True(await waitForSelector);
            Assert.True(divHidden);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "hidden should wait for removal")]
        [PuppeteerFact]
        public async Task HiddenShouldWaitForRemoval()
        {
            await DevToolsContext.SetContentAsync("<div></div>");
            var divRemoved = false;
            var waitForSelector = DevToolsContext.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divRemoved = true);
            await DevToolsContext.WaitForSelectorAsync("div"); // do a round trip
            Assert.False(divRemoved);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').remove()");
            Assert.True(await waitForSelector);
            Assert.True(divRemoved);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should return null if waiting to hide non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullIfWaitingToHideNonExistingElement()
        {
            var handle = await DevToolsContext.WaitForSelectorAsync("non-existing", new WaitForSelectorOptions { Hidden = true });
            Assert.Null(handle);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await DevToolsContext.WaitForSelectorAsync("div", new WaitForSelectorOptions { Timeout = 10 }));

            Assert.Contains("waiting for selector 'div' failed: timeout", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should have an error message specifically for awaiting an element to be hidden")]
        [PuppeteerFact]
        public async Task ShouldHaveAnErrorMessageSpecificallyForAwaitingAnElementToBeHidden()
        {
            await DevToolsContext.SetContentAsync("<div></div>");
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await DevToolsContext.WaitForSelectorAsync("div", new WaitForSelectorOptions { Hidden = true, Timeout = 10 }));

            Assert.Contains("waiting for selector 'div' to be hidden failed: timeout", exception.Message);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should respond to node attribute mutation")]
        [PuppeteerFact]
        public async Task ShouldRespondToNodeAttributeMutation()
        {
            var divFound = false;
            var waitForSelector = DevToolsContext.WaitForSelectorAsync(".zombo").ContinueWith(_ => divFound = true);
            await DevToolsContext.SetContentAsync("<div class='notZombo'></div>");
            Assert.False(divFound);
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('div').className = 'zombo'");
            Assert.True(await waitForSelector);
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should return the element handle")]
        [PuppeteerFact]
        public async Task ShouldReturnTheElementHandle()
        {
            var waitForSelector = DevToolsContext.WaitForSelectorAsync(".zombo");
            await DevToolsContext.SetContentAsync("<div class='zombo'>anything</div>");
            Assert.Equal("anything", await DevToolsContext.EvaluateFunctionAsync<string>("x => x.textContent", await waitForSelector));
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForSelector", "should have correct stack trace for timeout")]
        [PuppeteerFact]
        public async Task ShouldHaveCorrectStackTraceForTimeout()
        {
            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(async ()
                => await DevToolsContext.WaitForSelectorAsync(".zombo", new WaitForSelectorOptions { Timeout = 10 }));
            Assert.Contains("WaitForSelectorTests", exception.StackTrace);
        }
    }
}
