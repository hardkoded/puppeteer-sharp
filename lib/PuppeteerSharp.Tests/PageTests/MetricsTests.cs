using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class MetricsTests : PuppeteerPageBaseTest
    {
        public MetricsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldGetMetricsFromPage()
        {
            await Page.GoToAsync("about:blank");
            var metrics = await Page.MetricsAsync();
            CheckMetrics(metrics);
        }

        [Fact]
        public async Task MetricsEventFiredOnConsoleTimespan()
        {
            var metricsTaskWrapper = new TaskCompletionSource<MetricEventArgs>();
            Page.Metrics += (sender, e) => metricsTaskWrapper.SetResult(e);

            await Page.EvaluateExpressionAsync("console.timeStamp('test42')");
            var result = await metricsTaskWrapper.Task;

            Assert.Equal("test42", result.Title);
            CheckMetrics(result.Metrics);
        }

        private void CheckMetrics(Dictionary<string, decimal> metrics)
        {
            var metricsToCheck = Page.SupportedMetrics.ToList();

            foreach (var name in metrics.Keys)
            {
                Assert.Contains(name, metricsToCheck);
                Assert.True(metrics[name] >= 0);
                metricsToCheck.Remove(name);
            }
            Assert.True(metricsToCheck.Count == 0);
        }
    }
}