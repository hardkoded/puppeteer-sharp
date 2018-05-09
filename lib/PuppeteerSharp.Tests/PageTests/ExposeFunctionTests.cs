﻿using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ExposeFunctionTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [Fact]
        public async Task ShouldSurviveNavigation()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => a * b);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateExpressionAsync<int>("compute(9, 4)");
            Assert.Equal(36, result);
        }

        [Fact]
        public async Task ShouldAwaitReturnedValueTask()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            var result = await Page.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task ShouldWorkOnFrames()
        {
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var frame = Page.Frames[1];
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task ShouldWorkOnFramesBeforeNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            await Page.ExposeFunctionAsync("compute", (int a, int b) => Task.FromResult(a * b));

            var frame = Page.Frames[1];
            var result = await frame.EvaluateExpressionAsync<int>("compute(3, 5)");
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task ShouldAwaitReturnedTask()
        {
            bool called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
                return Task.CompletedTask;
            });
            await Page.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }

        [Fact]
        public async Task ShouldWorkWithAction()
        {
            bool called = false;
            await Page.ExposeFunctionAsync("changeFlag", () =>
            {
                called = true;
            });
            await Page.EvaluateExpressionAsync("changeFlag()");
            Assert.True(called);
        }
    }
}
