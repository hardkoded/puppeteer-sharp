using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class ScreenshotMutexTests
    {
        [Test]
        public async Task ShouldLockAndRelease()
        {
            var mutex = new ScreenshotMutex();
            var guard = await mutex.AcquireAsync();
            Assert.That(guard, Is.Not.Null);
            guard.Dispose();
        }

        [Test]
        public async Task ShouldWorkSequentially()
        {
            var mutex = new ScreenshotMutex();
            var results = new System.Collections.Generic.List<int>();
            var first = await mutex.AcquireAsync();
            var secondTask = mutex.AcquireAsync();

            _ = Task.Delay(10).ContinueWith(_ =>
            {
                results.Add(1);
                first.Dispose();
            });

            var second = await secondTask;
            results.Add(2);
            second.Dispose();

            Assert.That(results, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public async Task ShouldCallOnReleaseWhenDisposed()
        {
            var mutex = new ScreenshotMutex();
            var onReleaseCalled = false;
            var guard = await mutex.AcquireAsync(() => onReleaseCalled = true);
            guard.Dispose();
            Assert.That(onReleaseCalled, Is.True);
        }

        [Test]
        public async Task ShouldCallOnReleaseWhenDisposedForQueuedAcquirers()
        {
            var mutex = new ScreenshotMutex();
            var first = await mutex.AcquireAsync();

            var onReleaseCalled = false;
            var secondTask = mutex.AcquireAsync(() => onReleaseCalled = true);

            first.Dispose();
            var second = await secondTask;

            second.Dispose();
            Assert.That(onReleaseCalled, Is.True);
        }
    }
}
