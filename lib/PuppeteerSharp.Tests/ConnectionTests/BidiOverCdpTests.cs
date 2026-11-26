using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PuppeteerSharp.Bidi;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;
using WebDriverBiDi;
using WebDriverBiDi.Session;

namespace PuppeteerSharp.Tests.ConnectionTests;

public class BidiOverCdpTests
{
    [
        Test,
        PuppeteerTest(
            "BidiOverCdp.test",
            "connectBidiOverCdp",
            "closes BidiServer and BiDi connection if CDP disconnects during startup"
        )
    ]
    public async Task ShouldCloseTheBidiConnectionIfCdpDisconnectsDuringStartup()
    {
        using var cdpTransport = new TestCdpTransport { CloseAfterMapperStarts = true };

        var transport = await CreateBidiOverCdpTransportAsync(cdpTransport);
        await using var connection = TestConnectionTransport.CreateBidiConnection(transport);
        await connection.StartAsync("bidi-over-cdp://local");

        Assert.That(transport.IsClosed, Is.True);
        Assert.ThrowsAsync<WebDriverBiDiConnectionException>(() =>
            connection.Session.StatusAsync(new StatusCommandParameters())
        );
    }

    [
        Test,
        PuppeteerTest(
            "BidiOverCdp.test",
            "connectBidiOverCdp",
            "delivers queued bidiResponse messages before closing on CDP disconnect"
        )
    ]
    public async Task ShouldDeliverQueuedBidiResponseMessagesBeforeClosingOnCdpDisconnect()
    {
        using var cdpTransport = new TestCdpTransport();
        var transport = await CreateBidiOverCdpTransportAsync(cdpTransport);
        await using var connection = TestConnectionTransport.CreateBidiConnection(transport);

        var events = new ConcurrentQueue<string>();
        var closed = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        connection.Log.OnEntryAdded.AddObserver(_ => events.Enqueue("log.entryAdded"));
        await connection.StartAsync("bidi-over-cdp://local");
        connection.Closed += (_, _) =>
        {
            events.Enqueue("closed");
            closed.TrySetResult(true);
        };

        // Emit a BiDi event and disconnect CDP in the same tick.
        cdpTransport.EmitBidiMessage(
            JsonSerializer.Serialize(
                new
                {
                    type = "event",
                    method = "log.entryAdded",
                    @params = new
                    {
                        type = "console",
                        level = "info",
                        source = new { realm = "realm" },
                        text = "text",
                        timestamp = 0,
                        method = "log",
                        args = Array.Empty<object>(),
                    },
                }
            )
        );
        cdpTransport.Close();

        await closed.Task.WithTimeout(5_000);
        Assert.That(events, Is.EqualTo(new[] { "log.entryAdded", "closed" }));
    }

    private static Task<BidiOverCdpTransport> CreateBidiOverCdpTransportAsync(
        TestCdpTransport cdpTransport
    ) =>
        BidiOverCdpTransport.CreateAsync(
            "ws://127.0.0.1",
            new ConnectOptions
            {
                TransportFactory = (_, _, _) => Task.FromResult<IConnectionTransport>(cdpTransport),
            },
            NullLoggerFactory.Instance
        );

    /// <summary>
    /// A CDP endpoint which answers the commands needed to start the BiDi mapper.
    /// </summary>
    private sealed class TestCdpTransport : TestConnectionTransport
    {
        private const string MapperSessionId = "mapper-session";
        private const string MapperTargetId = "mapper-target";

        public bool CloseAfterMapperStarts { get; init; }

        public void EmitBidiMessage(string message) =>
            Emit(
                new
                {
                    sessionId = MapperSessionId,
                    method = "Runtime.bindingCalled",
                    @params = new { name = "sendBidiResponse", payload = message },
                }
            );

        public override Task SendAsync(byte[] message)
        {
            using var request = JsonDocument.Parse(Encoding.UTF8.GetString(message));
            var root = request.RootElement;
            var id = root.GetProperty("id").GetInt32();
            var method = root.GetProperty("method").GetString();

            if (root.TryGetProperty("sessionId", out var sessionId))
            {
                var startsMapper =
                    method == "Runtime.evaluate"
                    && root.GetProperty("params")
                        .GetProperty("expression")
                        .GetString()!
                        .StartsWith("window.runMapperInstance", StringComparison.Ordinal);

                Emit(
                    new
                    {
                        id,
                        sessionId = sessionId.GetString(),
                        result = new { },
                    }
                );

                if (startsMapper && CloseAfterMapperStarts)
                {
                    Close();
                }

                return Task.CompletedTask;
            }

            if (method == "Target.attachToTarget")
            {
                Emit(
                    new
                    {
                        method = "Target.attachedToTarget",
                        @params = new
                        {
                            sessionId = MapperSessionId,
                            targetInfo = new { type = "page", targetId = MapperTargetId },
                        },
                    }
                );
                Emit(new { id, result = new { sessionId = MapperSessionId } });
            }
            else
            {
                Emit(new { id, result = new { targetId = MapperTargetId } });
            }

            return Task.CompletedTask;
        }

        private void Emit(object message) => EmitMessage(JsonSerializer.Serialize(message));
    }
}
