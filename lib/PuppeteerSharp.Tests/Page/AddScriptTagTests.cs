﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AddScriptTagTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddScriptTagAsync(new AddScriptTagOptions()));
            Assert.Equal("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddScriptTagOptions { Url = "/injectedfile.js" });
            Assert.NotNull(scriptHandle.AsElement());
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Fact]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.AddScriptTagAsync(new AddScriptTagOptions { Url = "/nonexistfile.js" }));
            Assert.Equal("Loading script from /nonexistfile.js failed", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddScriptTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("assets", "injectedfile.js"))
            });
            Assert.NotNull(scriptHandle.AsElement());
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Fact]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddScriptTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("assets", "injectedfile.js"))
            });
            var result = await Page.EvaluateExpressionAsync<string>("__injectedError.stack");
            Assert.Contains(Path.Combine("assets", "injectedfile.js"), result);
        }

        [Fact]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddScriptTagOptions { Content = "window.__injected = 35;" });
            Assert.NotNull(scriptHandle.AsElement());
            Assert.Equal(35, await Page.EvaluateExpressionAsync<int>("__injected"));
        }
    }
}
