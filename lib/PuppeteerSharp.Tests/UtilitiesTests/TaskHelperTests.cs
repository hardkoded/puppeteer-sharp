using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class TaskHelperTests
    {
        [Test]
        [PuppeteerTimeout]
        public void ShouldExecuteActionOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1)));
        }

        [Test]
        [PuppeteerTimeout]
        public async Task ShouldNotExecuteActionOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1));
        }

        [Test]
        [PuppeteerTimeout]
        public void ShouldThrowOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(TimeSpan.FromTicks(1)));
        }

        [Test]
        [PuppeteerTimeout]
        public async Task ShouldNotThrowOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(TimeSpan.FromTicks(1));
        }

        [Test]
        [PuppeteerTimeout]
        public async Task ShouldNotExecuteActionOnTimeoutWhenCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(true);

            await tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Test]
        [PuppeteerTimeout]
        public void ShouldExecuteActionOnTimeoutWhenNotCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(false);
            Assert.ThrowsAsync<TimeoutException>(() => tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token));
        }

        [Test]
        [PuppeteerTimeout]
        public async Task ShouldNotExecuteAsyncActionOnCompletion()
        {
            var task = Task.CompletedTask;
            var token = new CancellationToken(false);

            await task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Test]
        [PuppeteerTimeout]
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
