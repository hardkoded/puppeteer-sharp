using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class MetricsTests : DevToolsContextBaseTest
    {
        public MetricsTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.metrics", "should get metrics from a page")]
        [PuppeteerFact]
        public async Task ShouldGetMetricsFromPage()
        {
            await DevToolsContext.GoToAsync("about:blank");
            var metrics = await DevToolsContext.MetricsAsync();
            CheckMetrics(metrics);
        }

        [PuppeteerTest("page.spec.ts", "Page.metrics", "metrics event fired on console.timeStamp")]
        [PuppeteerFact]
        public async Task MetricsEventFiredOnConsoleTimespan()
        {
            var metricsTaskWrapper = new TaskCompletionSource<MetricEventArgs>();
            DevToolsContext.Metrics += (_, e) => metricsTaskWrapper.SetResult(e);

            await DevToolsContext.EvaluateExpressionAsync("console.timeStamp('test42')");
            var result = await metricsTaskWrapper.Task;

            Assert.Equal("test42", result.Title);
            CheckMetrics(result.Metrics);
        }

        private void CheckMetrics(Dictionary<string, decimal> metrics)
        {
            var metricsToCheck = DevToolsContext.SupportedMetrics.ToList();

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
