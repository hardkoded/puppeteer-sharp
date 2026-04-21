using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WorkerTests
{
    public class PageWorkerTests : PuppeteerPageBaseTest
    {
        public PageWorkerTests() : base()
        {
        }

        private async Task<WebWorker> CreateWorkerAsync()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("() => new Worker('data:text/javascript,1')");
            return await workerCreatedTcs.Task;
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "Page.workers")]
        [Ignore("TODO: Fix me. Too flaky")]
        public async Task PageWorkers()
        {
            var workerCreatedTcs = new TaskCompletionSource<bool>();
            var workerDestroyedTcs = new TaskCompletionSource<bool>();

            Page.WorkerCreated += (_, _) => workerCreatedTcs.TrySetResult(true);
            Page.WorkerDestroyed += (_, _) => workerDestroyedTcs.TrySetResult(true);

            await Task.WhenAll(
                workerCreatedTcs.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html"));
            var worker = Page.Workers[0];
            Assert.That(worker.Url, Does.Contain("worker.js"));
            Assert.That(await worker.EvaluateExpressionAsync<string>("globalThis.workerFunction()"), Is.EqualTo("worker function result"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            await workerDestroyedTcs.Task.WithTimeout();
            Assert.That(Page.Workers, Is.Empty);
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should emit created and destroyed events")]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.That(await workerDestroyedTcs.Task, Is.SameAs(worker));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should report console logs")]
        public async Task ShouldReportConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");

            var log = await consoleTcs.Task;
            Assert.That(log.Text, Is.EqualTo("1"));
            Assert.That(log.Location, Is.EqualTo(new ConsoleMessageLocation
            {
                URL = "",
                LineNumber = 0,
                ColumnNumber = 8
            }));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should have JSHandles for console logs")]
        public async Task ShouldHaveJSHandlesForConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) =>
            {
                consoleTcs.TrySetResult(e.Message);
            };
            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1, 2, 3, this)`)");
            var log = await consoleTcs.Task;

            Assert.That(log.Text, Is.EqualTo("1 2 3 JSHandle@object"));
            Assert.That(log.Args.Count, Is.EqualTo(4));
            var json = await (await log.Args[3].GetPropertyAsync("origin")).JsonValueAsync<string>();
            Assert.That(json, Is.EqualTo("null"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should have an execution context")]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.That(await worker.EvaluateExpressionAsync<int>("1+1"), Is.EqualTo(2));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should report errors")]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>();
            Page.PageError += (_, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            Assert.That(errorLog, Does.Contain("this is my error"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should work with console logs")]
        public async Task ShouldWorkWithConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1,2,3,this)`)");
            var log = await consoleTcs.Task;

            Assert.That(log.Text, Does.Contain("1 2 3").Or.Contain("1 2 3"));
            Assert.That(log.Args, Has.Count.EqualTo(4));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "can be closed")]
        public async Task CanBeClosed()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            await workerCreatedTcs.Task;
            var worker = Page.Workers[0];
            var workerClosedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerClosedTcs.TrySetResult(e.Worker);

            Assert.That(worker.Url, Does.Contain("worker.js"));
            await worker.CloseAsync();
            Assert.That(await workerClosedTcs.Task, Is.SameAs(worker));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work")]
        public async Task ConsoleShouldWork()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.log('hello', 5, {foo: 'bar'})");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Text, Does.Contain("hello").And.Contain("5"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Log));
            Assert.That(message.Args, Has.Count.EqualTo(3));
            Assert.That(await message.Args[0].JsonValueAsync<string>(), Is.EqualTo("hello"));
            Assert.That(await message.Args[1].JsonValueAsync<int>(), Is.EqualTo(5));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for Error instances")]
        public async Task ConsoleShouldWorkForErrorInstances()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.log(new Error('test error'))");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Text, Does.Contain("test error").Or.EqualTo("JSHandle@error"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Log));
            Assert.That(message.Args, Has.Count.EqualTo(1));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should return the first line of the error message in text()")]
        public async Task ConsoleShouldReturnFirstLineOfErrorMessageInText()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.log(new Error('test error\\nsecond line'))");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Text, Does.Contain("test error").Or.EqualTo("JSHandle@error"));
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Log));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for console.trace")]
        public async Task ConsoleShouldWorkForConsoleTrace()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.trace('calling console.trace')");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Trace));
            Assert.That(message.Text, Is.EqualTo("calling console.trace"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for console.dir")]
        public async Task ConsoleShouldWorkForConsoleDir()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.dir('calling console.dir')");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Dir));
            Assert.That(message.Text, Is.EqualTo("calling console.dir"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for console.warn")]
        public async Task ConsoleShouldWorkForConsoleWarn()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.warn('calling console.warn')");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Warning));
            Assert.That(message.Text, Is.EqualTo("calling console.warn"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for console.error")]
        public async Task ConsoleShouldWorkForConsoleError()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.error('calling console.error')");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Error));
            Assert.That(message.Text, Is.EqualTo("calling console.error"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for console.log with promise")]
        public async Task ConsoleShouldWorkForConsoleLogWithPromise()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync("() => console.log(Promise.resolve('should not wait until resolved!'))");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Type, Is.EqualTo(ConsoleType.Log));
            Assert.That(message.Text, Does.Contain("promise").Or.Contain("Promise"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for different console API calls with timing functions")]
        public async Task ConsoleShouldWorkForTimingFunctions()
        {
            var worker = await CreateWorkerAsync();
            var messages = new List<ConsoleMessage>();
            worker.Console += (_, e) => messages.Add(e.Message);

            await worker.EvaluateFunctionAsync(@"() => {
                console.time('calling console.time');
                console.timeEnd('calling console.time');
            }");

            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0].Type, Is.EqualTo(ConsoleType.TimeEnd));
            Assert.That(messages[0].Text, Does.Contain("calling console.time"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should work for different console API calls with group functions")]
        public async Task ConsoleShouldWorkForGroupFunctions()
        {
            var worker = await CreateWorkerAsync();
            var messages = new List<ConsoleMessage>();
            worker.Console += (_, e) => messages.Add(e.Message);

            await worker.EvaluateFunctionAsync(@"() => {
                console.group('calling console.group');
                console.groupEnd();
            }");

            Assert.That(messages, Has.Count.EqualTo(2));
            Assert.That(messages[0].Type, Is.EqualTo(ConsoleType.StartGroup));
            Assert.That(messages[1].Type, Is.EqualTo(ConsoleType.EndGroup));
            Assert.That(messages[0].Text, Does.Contain("calling console.group"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers console", "should return remote objects")]
        public async Task ConsoleShouldReturnRemoteObjects()
        {
            var worker = await CreateWorkerAsync();
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            worker.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await worker.EvaluateFunctionAsync(@"() => {
                globalThis.test = 1;
                console.log(1, 2, 3, globalThis);
            }");

            var message = await consoleTcs.Task.WithTimeout();
            Assert.That(message.Text, Does.Contain("1 2 3"));
            Assert.That(message.Args, Has.Count.EqualTo(4));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should work with waitForNetworkIdle")]
        public async Task ShouldWorkWithWaitForNetworkIdle()
        {
            var workerCreatedTcs = new TaskCompletionSource<bool>();
            Page.WorkerCreated += (_, _) => workerCreatedTcs.TrySetResult(true);

            await Task.WhenAll(
                workerCreatedTcs.Task,
                Page.GoToAsync(
                    TestConstants.ServerUrl + "/worker/worker.html",
                    new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } }));

            await Page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions { Timeout = 3000 });
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should retrieve body for main worker requests")]
        public async Task ShouldRetrieveBodyForMainWorkerRequests()
        {
            IResponse testResponse = null;
            var workerUrl = TestConstants.ServerUrl + "/worker/worker.js";

            Page.Response += (_, e) =>
            {
                if (e.Response.Request.Url == workerUrl)
                {
                    testResponse = e.Response;
                }
            };

            var workerCreatedTcs = new TaskCompletionSource<bool>();
            Page.WorkerCreated += (_, _) => workerCreatedTcs.TrySetResult(true);

            await Task.WhenAll(
                workerCreatedTcs.Task,
                Page.GoToAsync(
                    TestConstants.ServerUrl + "/worker/worker.html",
                    new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } }));

            Assert.That(testResponse, Is.Not.Null);
            var text = await testResponse.TextAsync();
            Assert.That(text, Does.Contain("hello from the worker"));
        }
    }
}
