using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task ShouldWork()
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

    }
}
