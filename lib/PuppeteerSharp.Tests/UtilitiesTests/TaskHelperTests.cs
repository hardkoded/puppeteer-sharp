using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class TaskHelperTests
    {
        [Test]
        [Retry(2)]
        public void ShouldExecuteActionOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1)));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldNotExecuteActionOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1));
        }

        [Test]
        [Retry(2)]
        public void ShouldThrowOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(TimeSpan.FromTicks(1)));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldNotThrowOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(TimeSpan.FromTicks(1));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldNotExecuteActionOnTimeoutWhenCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(true);

            await tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Test]
        [Retry(2)]
        public void ShouldExecuteActionOnTimeoutWhenNotCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(false);
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token));
        }

        [Test]
        [Retry(2)]
        public async Task ShouldNotExecuteAsyncActionOnCompletion()
        {
            var task = Task.CompletedTask;
            var token = new CancellationToken(false);

            await task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Test]
        [Retry(2)]
        public async Task ShouldStopExecutionWhenTokenIsCanceled()
        {
            using var tokenSource = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<bool>();

            var task = tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromHours(42), tokenSource.Token);
            tokenSource.Cancel();

            await task;
        }
    }
}
