using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TracingTests
{
    public class TracingUnitTests
    {
        [
            Test,
            PuppeteerTest(
                "Tracing.test.ts",
                "Tracing",
                "does not mutate caller categories when screenshots is enabled"
            )
        ]
        public async Task DoesNotMutateCallerCategoriesWhenScreenshotsIsEnabled()
        {
            var client = new MockCdpSession();
            var tracing = new Tracing(client);
            var categories = new List<string> { "-*", "devtools.timeline" };

            await tracing.StartAsync(
                new TracingOptions { Categories = categories, Screenshots = true }
            );

            Assert.That(categories, Is.EqualTo(new[] { "-*", "devtools.timeline" }));
            Assert.That(client.Commands, Has.Count.EqualTo(1));
            Assert.That(client.Commands[0].Method, Is.EqualTo("Tracing.start"));

            var request = (TracingStartRequest)client.Commands[0].Args;
            Assert.That(request.TransferMode, Is.EqualTo("ReturnAsStream"));
            Assert.That(request.TraceConfig.ExcludedCategories, Is.EqualTo(new[] { "*" }));
            Assert.That(
                request.TraceConfig.IncludedCategories,
                Is.EqualTo(new[] { "devtools.timeline", "disabled-by-default-devtools.screenshot" })
            );
            Assert.That(request.TraceConfig.TraceBufferSizeInKb, Is.Null);
        }

        [
            Test,
            PuppeteerTest(
                "Tracing.test.ts",
                "Tracing",
                "resets recording state when Tracing.start fails"
            )
        ]
        public async Task ResetsRecordingStateWhenTracingStartFails()
        {
            var client = new MockCdpSession();
            var shouldFail = true;
            client.OnSend = method =>
                method == "Tracing.start" && shouldFail
                    ? Task.FromException(new PuppeteerException("Tracing is already started"))
                    : Task.CompletedTask;
            var tracing = new Tracing(client);

            var exception = Assert.ThrowsAsync<PuppeteerException>(() => tracing.StartAsync());
            Assert.That(exception.Message, Does.Contain("Tracing is already started"));

            shouldFail = false;
            await tracing.StartAsync();
            Assert.That(client.Commands, Has.Count.EqualTo(2));
        }

        [
            Test,
            PuppeteerTest(
                "Tracing.test.ts",
                "Tracing",
                "cleans up listeners and resets recording state when Tracing.end fails"
            )
        ]
        public async Task CleansUpListenersAndResetsRecordingStateWhenTracingEndFails()
        {
            var client = new MockCdpSession();
            client.OnSend = method =>
                method == "Tracing.end"
                    ? Task.FromException(
                        new PuppeteerException(
                            "Protocol error (Tracing.end): Tracing is not started"
                        )
                    )
                    : Task.CompletedTask;
            var tracing = new Tracing(client);

            await tracing.StartAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(() => tracing.StopAsync());
            Assert.That(
                exception.Message,
                Does.Contain("Protocol error (Tracing.end): Tracing is not started")
            );
            Assert.That(client.MessageReceivedListenerCount, Is.Zero);
            Assert.That(client.DisconnectedListenerCount, Is.Zero);

            await tracing.StartAsync();
        }

        [
            Test,
            PuppeteerTest(
                "Tracing.test.ts",
                "Tracing",
                "rejects and cleans up listeners if session disconnects while stopping"
            )
        ]
        public async Task RejectsAndCleansUpListenersIfSessionDisconnectsWhileStopping()
        {
            var client = new MockCdpSession();
            var tracing = new Tracing(client);

            await tracing.StartAsync();
            var stopTask = tracing.StopAsync();
            client.EmitDisconnected();

            // A TimeoutException here means that StopAsync hung on session disconnect.
            Assert.ThrowsAsync<TargetClosedException>(() => stopTask.WithTimeout(1_000));
            Assert.That(client.MessageReceivedListenerCount, Is.Zero);
            Assert.That(client.DisconnectedListenerCount, Is.Zero);

            tracing.UpdateClient(new MockCdpSession());
            await tracing.StartAsync();
        }

        private sealed class MockCdpSession : CDPSession
        {
            public List<(string Method, object Args)> Commands { get; } = [];

            public Func<string, Task> OnSend { get; set; }

            public override bool Detached => false;

            public override ILoggerFactory LoggerFactory => NullLoggerFactory.Instance;

            public int MessageReceivedListenerCount => GetListenerCount("_messageReceived");

            public int DisconnectedListenerCount => GetListenerCount(nameof(Disconnected));

            internal override CDPSession ParentSession => this;

            public override async Task<JsonElement?> SendAsync(
                string method,
                object args = null,
                bool waitForCallback = true,
                CommandOptions options = null
            )
            {
                Commands.Add((method, args));

                if (OnSend != null)
                {
                    await OnSend(method);
                }

                return null;
            }

            public override Task DetachAsync() => Task.CompletedTask;

            public override void Close(string closeReason) { }

            public void EmitDisconnected() => OnDisconnected();

            // CDPSession doesn't expose its subscribers, so we read the event's backing field.
            private int GetListenerCount(string fieldName)
            {
                var field = typeof(CDPSession).GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.NonPublic
                );
                Assert.That(field, Is.Not.Null);

                return (field.GetValue(this) as Delegate)?.GetInvocationList().Length ?? 0;
            }
        }
    }
}
