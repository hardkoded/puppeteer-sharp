using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetJavaScriptEnabledTests : PuppeteerPageBaseTest
    {
        public SetJavaScriptEnabledTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.EvaluateExpressionAsync("something"));
            Assert.That(exception.Message, Does.Contain("something is not defined"));

            await Page.SetJavaScriptEnabledAsync(true);
            Assert.That(Page.IsJavaScriptEnabled, Is.True);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.That(await Page.EvaluateExpressionAsync<string>("something"), Is.EqualTo("forbidden"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "setInterval should pause")]
        public async Task SetIntervalShouldPause()
        {
            // Set up an interval that increments a counter every 0ms.
            await Page.EvaluateFunctionAsync(@"() => {
                return setInterval(() => {
                    return (globalThis.intervalCounter = (globalThis.intervalCounter ?? 0) + 1);
                }, 0);
            }");

            // Disable JavaScript execution on the page. This should pause timers.
            await Page.SetJavaScriptEnabledAsync(false);

            // Capture the current value of the counter after JS is disabled.
            var intervalCounter = await Page.EvaluateFunctionAsync<int>("() => globalThis.intervalCounter");

            // Wait for 100 ms. This gives the event loop a chance to run if not paused.
            await Task.Delay(100);

            // Verify that the counter has not changed.
            Assert.That(
                await Page.EvaluateFunctionAsync<int>("() => globalThis.intervalCounter"),
                Is.EqualTo(intervalCounter));

            // Re-enable JavaScript execution.
            await Page.SetJavaScriptEnabledAsync(true);

            // Wait for another task to give the interval a chance to fire.
            await Page.EvaluateFunctionAsync(@"() => {
                return new Promise(resolve => setTimeout(resolve, 100));
            }");

            // Verify that the counter increased.
            Assert.That(
                await Page.EvaluateFunctionAsync<int>("() => globalThis.intervalCounter"),
                Is.GreaterThan(intervalCounter));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "setTimeout should stop")]
        public async Task SetTimeoutShouldStop()
        {
            // Set up a recursive setTimeout chain.
            await Page.EvaluateFunctionAsync(@"() => {
                const task = () => {
                    globalThis.timeoutCounter = (globalThis.timeoutCounter ?? 0) + 1;
                    setTimeout(task, 0);
                };
                task();
            }");

            // Disable JavaScript, which should pause the timeout chain.
            await Page.SetJavaScriptEnabledAsync(false);

            // Capture the counter's value after the timeout chain is paused.
            var timeoutCounter = await Page.EvaluateFunctionAsync<int>("() => globalThis.timeoutCounter");

            // Wait for 100 ms.
            await Task.Delay(100);

            // Verify the counter has not changed.
            Assert.That(
                await Page.EvaluateFunctionAsync<int>("() => globalThis.timeoutCounter"),
                Is.EqualTo(timeoutCounter));

            // Re-enable JavaScript.
            await Page.SetJavaScriptEnabledAsync(true);

            // Wait for another task.
            await Page.EvaluateFunctionAsync(@"() => {
                return new Promise(resolve => setTimeout(resolve, 100));
            }");

            // Verify the counter still has not changed, confirming that setTimeout does not
            // resume upon re-enabling JavaScript.
            Assert.That(
                await Page.EvaluateFunctionAsync<int>("() => globalThis.timeoutCounter"),
                Is.EqualTo(timeoutCounter));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "then should not pause")]
        public async Task ThenShouldNotPause()
        {
            // Disable JavaScript execution on the page.
            await Page.SetJavaScriptEnabledAsync(false);

            // Assert the microtasks continue to work even when page scripts are disabled.
            Assert.That(
                await Page.EvaluateFunctionAsync<int>(@"() => {
                    return Promise.resolve().then(() => 42);
                }"),
                Is.EqualTo(42));
        }
    }
}
