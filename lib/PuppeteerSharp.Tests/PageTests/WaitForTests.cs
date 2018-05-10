﻿using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class WaitForTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWaitForSelector()
        {
            var found = false;
            var waitFor = Page.WaitForSelectorAsync("div").ContinueWith(_ => found = true);
            await Page.GoToAsync(TestConstants.EmptyPage);

            Assert.False(found);

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await waitFor;
            Assert.True(found);
        }

        [Fact]
        public async Task ShouldTimeout()
        {
            var startTime = DateTime.Now;
            var timeout = 42;
            await Page.WaitForTimeoutAsync(timeout);
            Assert.True((DateTime.Now - startTime).TotalMilliseconds > timeout / 2);
        }

        [Fact]
        public async Task ShouldWaitForPredicate()
        {
            var watchdog = Page.WaitForFunctionAsync("() => window.innerWidth < 100");
            var viewPortTask = Page.SetViewportAsync(new ViewPortOptions { Width = 10, Height = 10 });
            await watchdog;
        }

        [Fact]
        public async Task ShouldWaitForPredicateWithArguments()
        {
            await Page.WaitForFunctionAsync("(arg1, arg2) => arg1 !== arg2", new WaitForFunctionOptions(), 1, 2);
        }
    }
}
