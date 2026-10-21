using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameManagerConcurrencyTests
    {
        [Test]
        public async Task ShouldConsumeEachRecordedFrameExactlyOnceWhenHandledConcurrently()
        {
            const int actors = 8;
            const int rounds = 200;
            const int padding = 50000;

            var handleFrameTree = typeof(FrameManager).GetMethod("HandleFrameTreeAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            var received = typeof(FrameManager).GetField("_frameNavigatedReceived", BindingFlags.NonPublic | BindingFlags.Instance);
            var errors = new List<Exception>();
            var violations = 0;

            for (var round = 0; round < rounds; round++)
            {
                var manager = RuntimeHelpers.GetUninitializedObject(typeof(FrameManager));
                var ids = Enumerable.Range(0, actors).Select(i => "frame-" + i).ToArray();
                var entries = Enumerable.Repeat("padding", padding).Concat(ids).ToList();
                received.SetValue(manager, entries);

                using var start = new Barrier(actors);
                var workers = ids.Select(id => Task.Factory.StartNew(
                    () =>
                    {
                        var tree = new PageGetFrameTree { Frame = new FramePayload { Id = id } };
                        start.SignalAndWait();
                        try
                        {
                            ((Task)handleFrameTree.Invoke(manager, [null, tree])).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            lock (errors)
                            {
                                errors.Add(ex is TargetInvocationException { InnerException: not null } t ? t.InnerException : ex);
                            }
                        }
                    },
                    TaskCreationOptions.LongRunning));

                await Task.WhenAll(workers);

                if (entries.Count != padding || entries.Any(id => id != "padding"))
                {
                    violations++;
                }
            }

            Assert.That(errors, Is.Empty);
            Assert.That(violations, Is.Zero);
        }
    }
}
