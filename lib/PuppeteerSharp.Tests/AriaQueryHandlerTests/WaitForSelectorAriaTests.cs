using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using SixLabors.ImageSharp;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForSelectorAriaTests : PuppeteerPageBaseTest
    {
        public WaitForSelectorAriaTests(ITestOutputHelper output) : base(output)
        {
        }

        const string addElement = @"(tag) => document.body.appendChild(document.createElement(tag))";

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should immediately resolve promise if node exists")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldImmediatelyResolvePromiseIfNodeExists()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should work for ElementHandle.waitForSelector")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkForElementHandleWaitForSelector()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(
                @"() => {
                    return (document.body.innerHTML = `<div><button>test</button></div>`);
                }");
            var element = Page.QuerySelectorAsync("div");
            await Page.WaitForSelectorAsync("aria/test");
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should persist query handler bindings across reloads")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldPersistQueryHandlerBindingsAcrossReloads()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.ReloadAsync();
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should persist query handler bindings across navigations")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldPersistQueryHandlerBindingsAcrossNavigations()
        {
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await Page.GoToAsync("data:text/html,");
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should work independently of `exposeFunction")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkIndependentlyOfExposeFunction()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.ExposeFunctionAsync("ariaQuerySelector", new Func<int, int, int>((a, b) => a + b));
            await Page.EvaluateFunctionAsync(addElement, "button");
            await Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            var result = await Page.EvaluateExpressionAsync<int>("globalThis.ariaQuerySelector(2,8)");
            Assert.Equal(10, result);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should work with removed MutationObserver")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithRemovedMutationObserver()
        {
            await Page.EvaluateFunctionAsync(@"() => delete window.MutationObserver");
            var handleTask = Page.WaitForSelectorAsync("aria/anything");
            await Task.WhenAll(
                handleTask,
                Page.SetContentAsync("<h1>anything</h1>"));
            Assert.NotNull(handleTask.Result);
            Assert.Equal("anything", await Page.EvaluateFunctionAsync("x => x.textContent", handleTask.Result));
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should resolve promise when node is added")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldResolvePromiseWhenNodeIsAdded()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var frame = Page.MainFrame;
            var watchdog = frame.WaitForSelectorAsync("aria/[role=\"heading\"]");
            await frame.EvaluateFunctionAsync(addElement, "br");
            await frame.EvaluateFunctionAsync(addElement, "h1");
            var elementHandle = await watchdog;
            var tagName = await (
              await elementHandle.GetPropertyAsync("tagName")
            ).JsonValueAsync();
            Assert.Equal("H1", tagName);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should work when node is added through innerHTML")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWhenNodeIsAddedThroughInnerHTML()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var watchdog = Page.WaitForSelectorAsync("aria/name");
            await Page.EvaluateFunctionAsync(addElement, "span");
            await Page.EvaluateFunctionAsync(@"() => {
                return (document.querySelector('span').innerHTML =
                  '<h3><div aria-label=""name""></div></h3>');
            }");
            await watchdog;
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "Page.waitForSelector is shortcut for main frame")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageWaitForSelectorIsShortcutForMainFrame()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var otherFrame = Page.FirstChildFrame();
            var watchdog = Page.WaitForSelectorAsync("aria/[role=\"button\"]");
            await otherFrame.EvaluateFunctionAsync(addElement, "button");
            await Page.EvaluateFunctionAsync(addElement, "button");
            var elementHandle = await watchdog;
            Assert.Same(elementHandle.ExecutionContext.Frame, Page.MainFrame);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "waitForSelector (aria)", "should run in specified frame")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldRunInSpecifiedFrame()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame2", TestConstants.EmptyPage);
            var frame1 = Page.Frames.First(frame => frame.Name == "frame1");
            var frame2 = Page.Frames.First(frame => frame.Name == "frame2");
            var waitForSelectorTask = frame2.WaitForSelectorAsync("aria/[role=\"button\"]");
            await frame1.EvaluateFunctionAsync(addElement, "button");
            await frame2.EvaluateFunctionAsync(addElement, "button");
            var elementHandle = await waitForSelectorTask;
            Assert.Same(elementHandle.ExecutionContext.Frame, frame2);
        }
    }
}
