﻿using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WaitForResponseTests : PuppeteerPageBaseTest
    {
        public WaitForResponseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [Fact]
        public async Task ShouldWorkWithPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(response => response.Url == TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }

        [Fact]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(request => false, new WaitForOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Fact]
        public async Task ShouldRespectDefaultTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(request => false));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitForOptions
            {
                Timeout = 0
            });

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }, 50)")
            );
            Assert.Equal(TestConstants.ServerUrl + "/digits/2.png", task.Result.Url);
        }
    }
}