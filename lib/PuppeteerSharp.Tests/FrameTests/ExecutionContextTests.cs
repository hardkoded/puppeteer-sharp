﻿using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ExecutionContextTests : PuppeteerPageBaseTest
    {
        public ExecutionContextTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, Page.Frames.Length);

            var context1 = await Page.Frames[0].GetExecutionContextAsync();
            var context2 = await Page.Frames[1].GetExecutionContextAsync();
            Assert.NotNull(context1);
            Assert.NotNull(context2);
            Assert.NotEqual(context1, context2);
            Assert.Equal(Page.Frames[0], context1.Frame);
            Assert.Equal(Page.Frames[1], context2.Frame);

            await Task.WhenAll(
                context1.EvaluateExpressionAsync("window.a = 1"),
                context2.EvaluateExpressionAsync("window.a = 2")
            );

            var a1 = context1.EvaluateExpressionAsync<int>("window.a");
            var a2 = context2.EvaluateExpressionAsync<int>("window.a");

            await Task.WhenAll(a1, a2);

            Assert.Equal(1, a1.Result);
            Assert.Equal(2, a2.Result);
        }
    }
}
