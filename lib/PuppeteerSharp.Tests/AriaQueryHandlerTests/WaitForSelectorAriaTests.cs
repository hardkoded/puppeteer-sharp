using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class WaitForSelectorAriaTests : PuppeteerPageBaseTest
    {
        private const string AddElement = @"(tag) => document.body.appendChild(document.createElement(tag))";

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should immediately resolve promise if node exists")]
        public async Task ShouldImmediatelyResolvePromiseIfNodeExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should work for ElementHandle.waitForSelector")]
        public async Task ShouldWorkForElementHandleWaitForSelector()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(
                @"() => {
                    return (document.body.innerHTML = `<div><button>test</button></div>`);
                }");
            var element = await Page.QuerySelectorAsync("div");
            await element.WaitForSelectorAsync("aria/test");
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should persist query handler bindings across reloads")]
        public async Task ShouldPersistQueryHandlerBindingsAcrossReloads()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.ReloadAsync();
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should persist query handler bindings across navigations")]
        public async Task ShouldPersistQueryHandlerBindingsAcrossNavigations()
        {
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should work independently of `exposeFunction")]
        public async Task ShouldWorkIndependentlyOfExposeFunction()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.ExposeFunctionAsync("ariaQuerySelector", new Func<int, int, int>((a, b) => a + b));
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            var result = await Page.EvaluateExpressionAsync<int>("globalThis.ariaQuerySelector(2,8)");
            Assert.That(result, Is.EqualTo(10));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should work with removed MutationObserver")]
        public async Task ShouldWorkWithRemovedMutationObserver()
        {
            await Page.EvaluateFunctionAsync(@"() => delete window.MutationObserver");
            var handleTask = Page.WaitForSelectorAsync("aria/anything");
            await Task.WhenAll(
                handleTask,
                Page.SetContentAsync("<h1>anything</h1>"));
            Assert.That(handleTask.Result, Is.Not.Null);
            Assert.That(await Page.EvaluateFunctionAsync<string>("x => x.textContent", handleTask.Result), Is.EqualTo("anything"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should resolve promise when node is added")]
        public async Task ShouldResolvePromiseWhenNodeIsAdded()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var watchdog = frame.WaitForSelectorAsync("aria/[role=\"heading\"]");
            await frame.EvaluateFunctionAsync(AddElement, "br");
            await frame.EvaluateFunctionAsync(AddElement, "h1");
            var elementHandle = await watchdog;
            var tagName = await (
              await elementHandle.GetPropertyAsync("tagName")
            ).JsonValueAsync<string>();
            Assert.That(tagName, Is.EqualTo("H1"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should work when node is added through innerHTML")]
        public async Task ShouldWorkWhenNodeIsAddedThroughInnerHtml()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var watchdog = Page.WaitForSelectorAsync("aria/name");
            await Page.EvaluateFunctionAsync(AddElement, "span");
            await Page.EvaluateFunctionAsync(@"() => {
                return (document.querySelector('span').innerHTML =
                  '<h3><div aria-label=""name""></div></h3>');
            }");
            await watchdog;
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "Page.waitForSelector is shortcut for main frame")]
        public async Task PageWaitForSelectorIsShortcutForMainFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var otherFrame = await Page.FirstChildFrameAsync();
            var watchdog = Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await otherFrame.EvaluateFunctionAsync(AddElement, "button");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            var elementHandle = await watchdog;
            Assert.That(Page.MainFrame, Is.SameAs(elementHandle.Frame));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should run in specified frame")]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.ChildFrames().ElementAt(0);
            var frame2 = Page.ChildFrames().ElementAt(1);
            var waitForSelectorTask = frame2.WaitForSelectorAsync("aria/[role=\"button\"]");
            await frame1.EvaluateFunctionAsync(AddElement, "button");
            await frame2.EvaluateFunctionAsync(AddElement, "button");
            var elementHandle = await waitForSelectorTask;
            Assert.That(frame2, Is.SameAs(elementHandle.Frame));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should throw when frame is detached")]
        public async Task ShouldThrowWhenFrameIsDetached()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = await Page.FirstChildFrameAsync();
            var waitTask = frame.WaitForSelectorAsync("aria/does-not-exist");
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var waitException = Assert.ThrowsAsync<WaitTaskTimeoutException>(() => waitTask);

            Assert.That(waitException, Is.Not.Null);
            Assert.That(waitException.Message, Does.Contain("Waiting for selector `does-not-exist` failed"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should survive cross-process navigation")]
        public async Task ShouldSurviveCrossProcessNavigation()
        {
            var imgFound = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/[role=\"image\"]").ContinueWith(_ => imgFound = true);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(imgFound, Is.False);
            await Page.ReloadAsync();
            Assert.That(imgFound, Is.False);
            await Page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/grid.html");
            await waitForSelector;
            Assert.That(imgFound, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should wait for visible")]
        public async Task ShouldWaitForVisible()
        {
            var divFound = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/name", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div aria-label='name' style='display: none; visibility: hidden;'>1</div>");
            Assert.That(divFound, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.removeProperty('display')");
            Assert.That(divFound, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.removeProperty('visibility')");
            Assert.That(await waitForSelector, Is.True);
            Assert.That(divFound, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should wait for visible recursively")]
        public async Task ShouldWaitForVisibleRecursively()
        {
            var divVisible = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/inner", new WaitForSelectorOptions { Visible = true })
                .ContinueWith(_ => divVisible = true);
            await Page.SetContentAsync("<div style='display: none; visibility: hidden;'><div aria-label='inner'>hi</div></div>");
            Assert.That(divVisible, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.removeProperty('display')");
            Assert.That(divVisible, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.removeProperty('visibility')");
            Assert.That(await waitForSelector, Is.True);
            Assert.That(divVisible, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "hidden should wait for visibility: hidden")]
        public async Task HiddenShouldWaitForVisibilityHidden()
        {
            await Page.SetContentAsync("<div role='button' style='display: block;'>text</div>");
            var divHidden = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/[role=\"button\"]", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            Assert.That(divHidden, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.setProperty('visibility', 'hidden')");
            Assert.That(await waitForSelector, Is.True);
            Assert.That(divHidden, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "hidden should wait for display: none")]
        public async Task HiddenShouldWaitForDisplayNone()
        {
            await Page.SetContentAsync("<div role='main' style='display: block;'>text</div>");
            var divHidden = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/[role=\"main\"]", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divHidden = true);
            await Page.WaitForSelectorAsync("aria/[role=\"main\"]");
            Assert.That(divHidden, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').style.setProperty('display', 'none')");
            Assert.That(await waitForSelector, Is.True);
            Assert.That(divHidden, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "hidden should wait for removal")]
        public async Task HiddenShouldWaitForRemoval()
        {
            await Page.SetContentAsync("<div role='main'>text</div>");
            var divRemoved = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/[role=\"main\"]", new WaitForSelectorOptions { Hidden = true })
                .ContinueWith(_ => divRemoved = true);
            await Page.WaitForSelectorAsync("aria/[role=\"main\"]");
            Assert.That(divRemoved, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').remove()");
            Assert.That(await waitForSelector, Is.True);
            Assert.That(divRemoved, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should return null if waiting to hide non-existing element")]
        public async Task ShouldReturnNullIfWaitingToHideNonExistingElement()
        {
            var handle = await Page.WaitForSelectorAsync("aria/non-existing", new WaitForSelectorOptions { Hidden = true });
            Assert.That(handle, Is.Null);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should respect timeout")]
        public async Task ShouldRespectTimeout()
        {
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(
                () => Page.WaitForSelectorAsync("aria/[role=\"button\"]", new WaitForSelectorOptions { Timeout = 10 }));

            Assert.That(exception.Message, Does.Contain("Waiting for selector `[role=\"button\"]` failed"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should have an error message specifically for awaiting an element to be hidden")]
        public async Task ShouldHaveAnErrorMessageSpecificallyForAwaitingAnElementToBeHidden()
        {
            await Page.SetContentAsync("<div role='main'>text</div>");
            var exception = Assert.ThrowsAsync<WaitTaskTimeoutException>(
                () => Page.WaitForSelectorAsync("aria/[role=\"main\"]", new WaitForSelectorOptions { Hidden = true, Timeout = 10 }));

            Assert.That(exception.Message, Does.Contain("Waiting for selector `[role=\"main\"]` failed"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should respond to node attribute mutation")]
        public async Task ShouldRespondToNodeAttributeMutation()
        {
            var divFound = false;
            var waitForSelector = Page.WaitForSelectorAsync("aria/zombo").ContinueWith(_ => divFound = true);
            await Page.SetContentAsync("<div aria-label='notZombo'></div>");
            Assert.That(divFound, Is.False);
            await Page.EvaluateFunctionAsync("() => document.querySelector('div').setAttribute('aria-label', 'zombo')");
            Assert.That(await waitForSelector, Is.True);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should return the element handle")]
        public async Task ShouldReturnTheElementHandle()
        {
            var waitForSelector = Page.WaitForSelectorAsync("aria/zombo");
            await Page.SetContentAsync("<div aria-label='zombo'>anything</div>");
            var handle = await waitForSelector;
            Assert.That(await Page.EvaluateFunctionAsync<string>("x => x?.textContent", handle), Is.EqualTo("anything"));
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "AriaQueryHandler waitForSelector (aria)", "should have correct stack trace for timeout")]
        public async Task ShouldHaveCorrectStackTraceForTimeout()
        {
            WaitTaskTimeoutException error = null;
            try
            {
                await Page.WaitForSelectorAsync("aria/zombo", new WaitForSelectorOptions { Timeout = 10 });
            }
            catch (WaitTaskTimeoutException ex)
            {
                error = ex;
            }

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Does.Contain("Waiting for selector `zombo` failed"));
        }
    }
}
