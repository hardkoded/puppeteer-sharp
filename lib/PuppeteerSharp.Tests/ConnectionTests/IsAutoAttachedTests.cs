using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.ConnectionTests;

public class IsAutoAttachedTests
{
    private const string TargetId = "target-id";

    [
        Test,
        PuppeteerTest(
            "Connection.test",
            "Connection isAutoAttached",
            "should report the target as auto-attached again after a manual attach is rejected"
        )
    ]
    public async Task ShouldReportTheTargetAsAutoAttachedAgainAfterAManualAttachIsRejected()
    {
        using var transport = new MockTransport();
        using var connection = CreateConnection(transport);

        var attach = connection.CreateSessionAsync(new TargetInfo { TargetId = TargetId }, false);
        Assert.That(connection.IsAutoAttached(TargetId), Is.False);

        transport.RejectAttach(transport.Sent[0]);
        var exception = Assert.ThrowsAsync<MessageException>(async () => await attach);
        Assert.That(exception.Message, Does.Contain("No target with given id found"));

        Assert.That(connection.IsAutoAttached(TargetId), Is.True);
    }

    [
        Test,
        PuppeteerTest(
            "Connection.test",
            "Connection isAutoAttached",
            "should not let an emulated attach clear a pending manual attach"
        )
    ]
    public async Task ShouldNotLetAnEmulatedAttachClearAPendingManualAttach()
    {
        using var transport = new MockTransport();
        using var connection = CreateConnection(transport);

        var manual = connection.CreateSessionAsync(new TargetInfo { TargetId = TargetId }, false);
        var emulated = connection.CreateSessionAsync(new TargetInfo { TargetId = TargetId }, true);
        var manualCommand = transport.Sent[0];
        var emulatedCommand = transport.Sent[1];

        transport.AcceptAttach(emulatedCommand, "emulated-session");
        await emulated;
        Assert.That(connection.IsAutoAttached(TargetId), Is.False);

        transport.AcceptAttach(manualCommand, "manual-session");
        await manual;
        Assert.That(connection.IsAutoAttached(TargetId), Is.True);
    }

    private static Connection CreateConnection(MockTransport transport) =>
        Connection.CreateFromTransport(transport, new ConnectOptions());

    /// <summary>
    /// Records the commands a connection sends and lets a test reply to them in place of the browser.
    /// </summary>
    private sealed class MockTransport : IConnectionTransport
    {
        public event EventHandler<TransportClosedEventArgs> Closed;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public List<int> Sent { get; } = [];

        public bool IsClosed { get; private set; }

        private Dictionary<int, string> Targets { get; } = [];

        public void StopReading() { }

        public Task SendAsync(byte[] message)
        {
            using var doc = JsonDocument.Parse(message);
            var id = doc.RootElement.GetProperty("id").GetInt32();
            Targets[id] = doc.RootElement.GetProperty("params").GetProperty("targetId").GetString();
            Sent.Add(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Completes a Target.attachToTarget command the way Chrome does: the
        /// Target.attachedToTarget event first, then the command response.
        /// </summary>
        public void AcceptAttach(int id, string sessionId)
        {
            Receive(
                $$$"""{"method":"Target.attachedToTarget","params":{"sessionId":"{{{sessionId}}}","targetInfo":{"targetId":"{{{Targets[id]}}}","type":"page"},"waitingForDebugger":false}}"""
            );
            Receive($$$"""{"id":{{{id}}},"result":{"sessionId":"{{{sessionId}}}"}}""");
        }

        public void RejectAttach(int id) =>
            Receive(
                $$$"""{"id":{{{id}}},"error":{"code":-32602,"message":"No target with given id found"}}"""
            );

        public void Dispose()
        {
            IsClosed = true;
            Closed?.Invoke(this, new TransportClosedEventArgs("disposed"));
        }

        private void Receive(string message) =>
            MessageReceived?.Invoke(
                this,
                new MessageReceivedEventArgs(Encoding.UTF8.GetBytes(message))
            );
    }
}
