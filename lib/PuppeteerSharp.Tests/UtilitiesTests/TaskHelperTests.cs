using System;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using Xunit;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class TaskHelperTests
    {
        [Fact]
        public async Task ShouldExecuteActionOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();

            Func<Task> act = () => tcs.Task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1));

            await Assert.ThrowsAnyAsync<TimeoutException>(act);
        }

        [Fact]
        public async Task ShouldNotExecuteActionOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(() => throw new TimeoutException(), TimeSpan.FromTicks(1));
        }

        [Fact]
        public async Task ShouldThrowOnTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();

            Func<Task> act = () => tcs.Task.WithTimeout(TimeSpan.FromTicks(1));

            await Assert.ThrowsAnyAsync<TimeoutException>(act);
        }

        [Fact]
        public async Task ShouldNotThrowOnCompletion()
        {
            var task = Task.FromResult(true);

            await task.WithTimeout(TimeSpan.FromTicks(1));
        }

        [Fact]
        public async Task ShouldNotExecuteActionOnTimeoutWhenCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(true);

            await tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Fact]
        public async Task ShouldExecuteActionOnTimeoutWhenNotCanceled()
        {
            var tcs = new TaskCompletionSource<bool>();
            var token = new CancellationToken(false);

            Func<Task> act = () => tcs.Task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);

            await Assert.ThrowsAnyAsync<TimeoutException>(act);
        }

        [Fact]
        public async Task ShouldNotExecuteAsyncActionOnCompletion()
        {
            var task = Task.CompletedTask;
            var token = new CancellationToken(false);

            await task.WithTimeout(() => Task.FromException(new TimeoutException()), TimeSpan.FromTicks(1), token);
        }

        [Fact]
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
