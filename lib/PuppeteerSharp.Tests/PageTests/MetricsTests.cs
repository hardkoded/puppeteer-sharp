using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class MetricsTests : PuppeteerPageBaseTest
    {
        public MetricsTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.metrics", "should get metrics from a page")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldGetMetricsFromPage()
        {
            await Page.GoToAsync("about:blank");
            var metrics = await Page.MetricsAsync();
            CheckMetrics(metrics);
        }

        [PuppeteerTest("page.spec.ts", "Page.metrics", "metrics event fired on console.timeStamp")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task MetricsEventFiredOnConsoleTimespan()
        {
            var metricsTaskWrapper = new TaskCompletionSource<MetricEventArgs>();
            Page.Metrics += (_, e) => metricsTaskWrapper.SetResult(e);

            await Page.EvaluateExpressionAsync("console.timeStamp('test42')");
            var result = await metricsTaskWrapper.Task;

            Assert.AreEqual("test42", result.Title);
            CheckMetrics(result.Metrics);
        }

        private void CheckMetrics(Dictionary<string, decimal> metrics)
        {
            var metricsToCheck = PuppeteerSharp.Page.SupportedMetrics.ToList();

            foreach (var name in metrics.Keys)
            {
                StringAssert.Contains(name, metricsToCheck);
                Assert.True(metrics[name] >= 0);
                metricsToCheck.Remove(name);
            }
            Assert.True(metricsToCheck.Count == 0);
        }
    }
}
