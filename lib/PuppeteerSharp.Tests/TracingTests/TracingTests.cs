using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TracingTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TracingTests : PuppeteerPageBaseTest
    {
        private readonly string _file;

        public TracingTests(ITestOutputHelper output) : base(output)
        {
            _file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        public override async Task DisposeAsync()
        {
            await base.DisposeAsync();

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

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should output a trace")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldOutputATrace()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.Tracing.StopAsync();

            Assert.True(File.Exists(_file));
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should run with custom categories if provided")]
        [SkipBrowserFact(skipFirefox: true)]
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

            using (var file = File.OpenText(_file))
            using (var reader = new JsonTextReader(file))
            {
                var traceJson = JToken.ReadFrom(reader);
                Assert.Contains("disabled-by-default-v8.cpu_profiler.hires", traceJson["metadata"]["trace-config"].ToString());
            }
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should throw if tracing on two pages")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowIfTracingOnTwoPages()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Path = _file
            });
            var newPage = await Browser.NewPageAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Page.Tracing.StartAsync(new TracingOptions
                {
                    Path = _file
                });
            });

            await newPage.CloseAsync();
            await Page.Tracing.StopAsync();
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should return a buffer")]
        [SkipBrowserFact(skipFirefox: true)]
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
            Assert.Equal(trace, buf);
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should work without options")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithoutOptions()
        {
            await Page.Tracing.StartAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await Page.Tracing.StopAsync();
            Assert.NotNull(trace);
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should support a buffer without a path")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportABufferWithoutAPath()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await Page.Tracing.StopAsync();
            Assert.Contains("screenshot", trace);
        }
    }
}
