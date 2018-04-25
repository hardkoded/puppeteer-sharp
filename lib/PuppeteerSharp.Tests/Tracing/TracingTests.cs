using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace PuppeteerSharp.Tests.Tracing
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TracingTests : PuppeteerPageBaseTest
    {
        private string _file;

        public TracingTests()
        {
            _file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        public override void Dispose()
        {
            base.Dispose();

            int attempts = 0;
            const int attemptTimes = 5;

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
                    if (attempts == attemptTimes)
                    {
                        break;
                    }

                    Task.Delay(1000).GetAwaiter().GetResult();
                }
            }
        }

        [Fact]
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


        [Fact]
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
                dynamic traceJson = JToken.ReadFrom(reader);
                Assert.Contains("disabled-by-default-v8.cpu_profiler.hires", traceJson.metadata["trace-config"]);
            }
        }

        [Fact]
        public async Task ShouldThrowIfTracingOnTwoPages()
        {
            await Page.Tracing.StartAsync(new TracingOptions
            {
                Path = _file,
            });
            var page = await Browser.NewPageAsync();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await Page.Tracing.StartAsync(new TracingOptions
                {
                    Path = _file,
                });
            });

            await page.CloseAsync();
            await page.Tracing.StopAsync();
        }
    }
}
