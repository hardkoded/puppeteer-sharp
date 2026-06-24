using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WorkerTests
{
    public class WorkerWaitForFunctionTests : PuppeteerPageBaseTest
    {
        public WorkerWaitForFunctionTests() : base()
        {
        }

        private async Task<WebWorker> CreateWorkerAsync(string workerScript = "1")
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync($"() => new Worker('data:text/javascript,{workerScript}')");
            return await workerCreatedTcs.Task;
        }

        [Test, PuppeteerTest("worker.spec", "Workers waitForFunction", "should wait for a condition")]
        public async Task ShouldWaitForACondition()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync(@"() => new Worker(`data:text/javascript,
                setTimeout(() => {
                    self.foo = true;
                }, 500);
            `)");
            var worker = await workerCreatedTcs.Task;

            await worker.WaitForFunctionAsync("() => self.foo === true").WithTimeout();
        }

        [Test, PuppeteerTest("worker.spec", "Workers waitForFunction", "should timeout if condition is not met")]
        public async Task ShouldTimeoutIfConditionIsNotMet()
        {
            var worker = await CreateWorkerAsync();

            Exception error = null;
            try
            {
                await worker.WaitForFunctionAsync(
                    "() => false",
                    new WaitForFunctionOptions { Timeout = 50 });
            }
            catch (Exception ex)
            {
                error = ex;
            }

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Does.Contain("Waiting failed"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers waitForFunction", "should return a JSHandle to a string and parse it")]
        public async Task ShouldReturnJSHandleToStringAndParseIt()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync(@"() => new Worker(`data:text/javascript,
                setTimeout(() => {
                    self.status = 'ready';
                }, 500);
            `)");
            var worker = await workerCreatedTcs.Task;

            await using var handle = await worker.WaitForFunctionAsync(
                "() => self.status === 'ready' ? 'Operation Success' : false").WithTimeout();

            var result = await handle.JsonValueAsync<string>();
            Assert.That(result, Is.EqualTo("Operation Success"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers waitForFunction", "should work with JSHandle as an argument")]
        public async Task ShouldWorkWithJSHandleAsArgument()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync(@"() => new Worker(`data:text/javascript,
                self.targetValue = 42;
            `)");
            var worker = await workerCreatedTcs.Task;

            // Wait briefly to let the worker initialize
            await Task.Delay(200);

            await using var argHandle = await worker.EvaluateExpressionHandleAsync("42");

            await worker.WaitForFunctionAsync(
                "(expected) => self.targetValue === expected",
                null,
                argHandle).WithTimeout();
        }
    }
}
