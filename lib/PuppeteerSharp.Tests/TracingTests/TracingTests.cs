using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TracingTests
{
    public sealed class TracingTests : PuppeteerPageBaseTest, IAsyncDisposable
    {
        private readonly string _file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        public async ValueTask DisposeAsync()
        {
            var attempts = 0;
            const int maxAttempts = 5;

            while (true)
            {
                try
                {
                    attempts++;
                    if (File.Exists(_file))
                    {
                        File.Delete(_file);
                    }
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    if (attempts == maxAttempts)
                    {
                        break;
                    }

                    await Task.Delay(1000);
                }
            }
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should output a trace")]
        public async Task ShouldOutputATrace()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.Tracing.StopAsync();

            Assert.That(File.Exists(_file), Is.True);
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should run with custom categories if provided")]
        public async Task ShouldRunWithCustomCategoriesProvided()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file,
                Categories = new List<string>
                {
                    "disabled-by-default-v8.cpu_profiler.hires"
                }
            });

            await Page.Tracing.StopAsync();

            var jsonString = await File.ReadAllTextAsync(_file);

            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;
            var metadata = root.GetProperty("metadata");
            var traceConfig = metadata.GetProperty("trace-config");

            var traceConfigString = traceConfig.GetString();
            Assert.That(traceConfigString, Does.Contain("disabled-by-default-v8.cpu_profiler.hires"));
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should run with default categories")]
        public async Task ShouldRunWithDefaultCategories()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Path = _file,
            });

            await Page.Tracing.StopAsync();
            var jsonString = await File.ReadAllTextAsync(_file);

            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;
            var traceEvents = root.GetProperty("traceEvents");
            var traceConfigString = traceEvents.ToString();
            Assert.That(traceConfigString, Does.Contain("toplevel"));
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should throw if tracing on two pages")]
        public async Task ShouldThrowIfTracingOnTwoPages()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Path = _file
            });
            var newPage = await Browser.NewPageAsync();
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Page.Tracing.StartAsync(new TracingOptions
                {
                    Path = _file
                });
            });

            await newPage.CloseAsync();
            await Page.Tracing.StopAsync();
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should return a buffer")]
        public async Task ShouldReturnABuffer()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await Page.Tracing.StopAsync();
            var buf = File.ReadAllText(_file);
            Assert.That(buf, Is.EqualTo(trace));
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should work without options")]
        public async Task ShouldWorkWithoutOptions()
        {
            await Page.Tracing.StartAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await Page.Tracing.StopAsync();
            Assert.That(trace, Is.Not.Null);
        }

        [Test, PuppeteerTest("tracing.spec", "Tracing", "should support a buffer without a path")]
        public async Task ShouldSupportABufferWithoutAPath()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await Page.Tracing.StopAsync();
            Assert.That(trace, Does.Contain("screenshot"));
        }
    }
}
