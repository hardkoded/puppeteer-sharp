using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class TaskQueueTests
    {
        [Test]
        public void ShouldDisposeSemaphoreWhenDisposing()
        {
            var taskQueue = new TaskQueue();
            taskQueue.Dispose();

            var semaphore = GetSemaphore(taskQueue);
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = semaphore.AvailableWaitHandle;
            });
        }

        [Test]
        public void ShouldNotThrowWhenDisposingMultipleTimes()
        {
            var taskQueue = new TaskQueue();
            taskQueue.Dispose();

            // Can safely be disposed a second time
            taskQueue.Dispose();
        }

        [Test]
        public async Task ShouldDisposeSemaphoreWhenDisposingAsync()
        {
            var taskQueue = new TaskQueue();
            await taskQueue.DisposeAsync().ConfigureAwait(false);

            var semaphore = GetSemaphore(taskQueue);
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = semaphore.AvailableWaitHandle;
            });
        }

        [Test]
        public async Task ShouldNotThrowWhenDisposingMultipleTimesAsync()
        {
            var taskQueue = new TaskQueue();
            await taskQueue.DisposeAsync().ConfigureAwait(false);

            // Can safely be disposed a second time
            await taskQueue.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CanDisposeWhileSemaphoreIsHeld()
        {
            var taskQueue = new TaskQueue();

            await taskQueue.Enqueue(() =>
            {
                taskQueue.Dispose();
                return Task.CompletedTask;
            });

            var semaphore = GetSemaphore(taskQueue);
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = semaphore.AvailableWaitHandle;
            });

            taskQueue.Dispose();
        }

        [Test]
        public async Task CanDisposeWhileSemaphoreIsHeldAsync()
        {
            var taskQueue = new TaskQueue();

            await taskQueue.Enqueue(async () =>
            {
                await taskQueue.DisposeAsync();
            });

            var semaphore = GetSemaphore(taskQueue);
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = semaphore.AvailableWaitHandle;
            });

            await taskQueue.DisposeAsync();
        }

        private static SemaphoreSlim GetSemaphore(TaskQueue queue) =>
            (SemaphoreSlim)typeof(TaskQueue).GetField("_semaphore", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(queue);
    }
}
