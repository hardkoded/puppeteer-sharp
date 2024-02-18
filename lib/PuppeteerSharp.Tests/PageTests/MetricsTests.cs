using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class MetricsTests : PuppeteerPageBaseTest
    {
        public MetricsTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.metrics", "should get metrics from a page")]
        public async Task ShouldGetMetricsFromPage()
        {
            await Page.GoToAsync("about:blank");
            var metrics = await Page.MetricsAsync();
            CheckMetrics(metrics);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.metrics", "metrics event fired on console.timeStamp")]
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
                Assert.Contains(name, metricsToCheck);
                Assert.True(metrics[name] >= 0);
                metricsToCheck.Remove(name);
            }
            Assert.True(metricsToCheck.Count == 0);
        }
    }
}
