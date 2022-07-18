using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TracingTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TracingTests : DevToolsContextBaseTest
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
                    if (System.IO.File.Exists(_file))
                    {
                        System.IO.File.Delete(_file);
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
        [PuppeteerFact]
        public async Task ShouldOutputATrace()
        {
            await DevToolsContext.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await DevToolsContext.Tracing.StopAsync();

            Assert.True(System.IO.File.Exists(_file));
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should run with custom categories if provided")]
        [PuppeteerFact]
        public async Task ShouldRunWithCustomCategoriesProvided()
        {
            await DevToolsContext.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file,
                Categories = new List<string>
                {
                    "disabled-by-default-v8.cpu_profiler.hires"
                }
            });

            await DevToolsContext.Tracing.StopAsync();

            using (var file = System.IO.File.OpenText(_file))
            using (var reader = new JsonTextReader(file))
            {
                var traceJson = JToken.ReadFrom(reader);
                Assert.Contains("disabled-by-default-v8.cpu_profiler.hires", traceJson["metadata"]["trace-config"].ToString());
            }
        }

        //[PuppeteerTest("tracing.spec.ts", "Tracing", "should throw if tracing on two pages")]
        //[PuppeteerFact]
        //public async Task ShouldThrowIfTracingOnTwoPages()
        //{
        //    await DevToolsContext.Tracing.StartAsync(new TracingOptions
        //    {
        //        Path = _file
        //    });
        //    var newPage = await Browser.NewPageAsync();
        //    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        //    {
        //        await DevToolsContext.Tracing.StartAsync(new TracingOptions
        //        {
        //            Path = _file
        //        });
        //    });

        //    await newPage.CloseAsync();
        //    await DevToolsContext.Tracing.StopAsync();
        //}

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should return a buffer")]
        [PuppeteerFact]
        public async Task ShouldReturnABuffer()
        {
            await DevToolsContext.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true,
                Path = _file
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await DevToolsContext.Tracing.StopAsync();
            var buf = System.IO.File.ReadAllText(_file);
            Assert.Equal(trace, buf);
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should work without options")]
        [PuppeteerFact]
        public async Task ShouldWorkWithoutOptions()
        {
            await DevToolsContext.Tracing.StartAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await DevToolsContext.Tracing.StopAsync();
            Assert.NotNull(trace);
        }

        [PuppeteerTest("tracing.spec.ts", "Tracing", "should support a buffer without a path")]
        [PuppeteerFact]
        public async Task ShouldSupportABufferWithoutAPath()
        {
            await DevToolsContext.Tracing.StartAsync(new TracingOptions
            {
                Screenshots = true
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var trace = await DevToolsContext.Tracing.StopAsync();
            Assert.Contains("screenshot", trace);
        }
    }
}
