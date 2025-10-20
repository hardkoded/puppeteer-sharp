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

        [Test, PuppeteerTest("page.spec", "Page Page.metrics", "should get metrics from a page")]
        public async Task ShouldGetMetricsFromPage()
        {
            await Page.GoToAsync("about:blank");
            var metrics = await Page.MetricsAsync();
            CheckMetrics(metrics);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.metrics", "metrics event fired on console.timeStamp")]
        public async Task MetricsEventFiredOnConsoleTimespan()
        {
            var metricsTaskWrapper = new TaskCompletionSource<MetricEventArgs>();
            Page.Metrics += (_, e) => metricsTaskWrapper.SetResult(e);

            await Page.EvaluateExpressionAsync("console.timeStamp('test42')");
            var result = await metricsTaskWrapper.Task;

            Assert.That(result.Title, Is.EqualTo("test42"));
            CheckMetrics(result.Metrics);
        }

        private void CheckMetrics(Dictionary<string, decimal> metrics)
        {
            var metricsToCheck = PuppeteerSharp.Page.SupportedMetrics.ToList();

            foreach (var name in metrics.Keys)
            {
                Assert.That(metricsToCheck, Does.Contain(name));
                Assert.That(metrics[name], Is.GreaterThanOrEqualTo(0));
                metricsToCheck.Remove(name);
            }
            Assert.That(metricsToCheck, Is.Empty);
        }
    }
}
