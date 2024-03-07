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

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should immediately resolve promise if node exists")]
        public async Task ShouldImmediatelyResolvePromiseIfNodeExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should work for ElementHandle.waitForSelector")]
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

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should persist query handler bindings across reloads")]
        public async Task ShouldPersistQueryHandlerBindingsAcrossReloads()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.ReloadAsync();
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should persist query handler bindings across navigations")]
        public async Task ShouldPersistQueryHandlerBindingsAcrossNavigations()
        {
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should work independently of `exposeFunction")]
        public async Task ShouldWorkIndependentlyOfExposeFunction()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.ExposeFunctionAsync("ariaQuerySelector", new Func<int, int, int>((a, b) => a + b));
            await Page.EvaluateFunctionAsync(AddElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            var result = await Page.EvaluateExpressionAsync<int>("globalThis.ariaQuerySelector(2,8)");
            Assert.AreEqual(10, result);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should work with removed MutationObserver")]
        public async Task ShouldWorkWithRemovedMutationObserver()
        {
            await Page.EvaluateFunctionAsync(@"() => delete window.MutationObserver");
            var handleTask = Page.WaitForSelectorAsync("aria/anything");
            await Task.WhenAll(
                handleTask,
                Page.SetContentAsync("<h1>anything</h1>"));
            Assert.NotNull(handleTask.Result);
            Assert.AreEqual("anything", await Page.EvaluateFunctionAsync<string>("x => x.textContent", handleTask.Result));
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should resolve promise when node is added")]
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
            ).JsonValueAsync();
            Assert.AreEqual("H1", tagName);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should work when node is added through innerHTML")]
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

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "Page.waitForSelector is shortcut for main frame")]
        public async Task PageWaitForSelectorIsShortcutForMainFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var otherFrame = Page.FirstChildFrame();
            var watchdog = Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await otherFrame.EvaluateFunctionAsync(AddElement, "button");
            await Page.EvaluateFunctionAsync(AddElement, "button");
            var elementHandle = await watchdog;
            Assert.AreSame(elementHandle.Frame, Page.MainFrame);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "waitForSelector (aria)", "should run in specified frame")]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.Frames.First(frame => frame.Name == "frame1");
            var frame2 = Page.Frames.First(frame => frame.Name == "frame2");
            var waitForSelectorTask = frame2.WaitForSelectorAsync("aria/[role=\"button\"]");
            await frame1.EvaluateFunctionAsync(AddElement, "button");
            await frame2.EvaluateFunctionAsync(AddElement, "button");
            var elementHandle = await waitForSelectorTask;
            Assert.AreSame(elementHandle.Frame, frame2);
        }
    }
}
