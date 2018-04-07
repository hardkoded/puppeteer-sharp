using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class MetricsTests : PuppeteerPageBaseTest
    {
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
            Page.MetricsReceived += (sender, e) => metricsTaskWrapper.SetResult(e);

            await Page.EvaluateExpressionAsync("console.timeStamp('test42')");
            var result = await metricsTaskWrapper.Task;

            Assert.Equal("test42", result.Title);
            CheckMetrics(result.Metrics);
        }

        private void CheckMetrics(Dictionary<string, decimal> metrics)
        {
            var metricsToCheck = new List<string>{
                "Timestamp",
                "Documents",
                "Frames",
                "JSEventListeners",
                "Nodes",
                "LayoutCount",
                "RecalcStyleCount",
                "LayoutDuration",
                "RecalcStyleDuration",
                "ScriptDuration",
                "TaskDuration",
                "JSHeapUsedSize",
                "JSHeapTotalSize",
            };

            foreach (var name in metrics.Keys)
            {
                Assert.Contains(name, metricsToCheck);
                Assert.True(metrics[name] > 0);
                metricsToCheck.Remove(name);
            }
            Assert.True(metricsToCheck.Count == 0);
        }
    }
}