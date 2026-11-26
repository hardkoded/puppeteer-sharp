using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.CdpTests;

public class TargetTests
{
    private const string SessionId = "session-id";
    private const string TargetId = "target-id";
    private const int TestTimeout = 10_000;

    [
        Test,
        PuppeteerTest(
            "Target.test",
            "WorkerTarget close",
            "should detach from a service worker after worker() failed"
        )
    ]
    public async Task ShouldDetachFromAServiceWorkerAfterWorkerFailed()
    {
        using var fixture = new ServiceWorkerTargetFixture();
        var exception = Assert.CatchAsync(() => fixture.Target.WorkerAsync());
        Assert.That(exception.Message, Does.Contain("Attach failed"));

        var closed = fixture.Target.CloseAsync();
        fixture.Target.CloseTaskWrapper.TrySetResult(true);
        await closed.WithTimeout(TestTimeout);

        Assert.That(fixture.Transport.ClosedTargetIds, Is.EqualTo(new[] { TargetId }));
        Assert.That(fixture.Transport.DetachedSessionIds, Is.EqualTo(new[] { SessionId }));
    }

    [
        Test,
        PuppeteerTest(
            "Target.test",
            "WorkerTarget close",
            "should not throw if Chrome detached a session while closing"
        )
    ]
    public async Task ShouldNotThrowIfChromeDetachedASessionWhileClosing()
    {
        using var fixture = new ServiceWorkerTargetFixture();

        // Chrome sends `Target.detachedFromTarget` before the error response.
        fixture.Transport.OnDetach = id =>
        {
            fixture.Transport.EmitDetachedFromTarget(SessionId);
            fixture.Transport.Reject(id, "No session with given id");
        };

        var closed = fixture.Target.CloseAsync();
        fixture.Target.CloseTaskWrapper.TrySetResult(true);
        await closed.WithTimeout(TestTimeout);

        Assert.That(fixture.Transport.DetachedSessionIds, Has.Count.EqualTo(1));
    }

    [
        Test,
        PuppeteerTest(
            "Target.test",
            "WorkerTarget close",
            "should throw if detaching a session fails"
        )
    ]
    public void ShouldThrowIfDetachingASessionFails()
    {
        using var fixture = new ServiceWorkerTargetFixture();
        fixture.Transport.OnDetach = id => fixture.Transport.Reject(id, "Detach failed");

        var exception = Assert.ThrowsAsync<MessageException>(() =>
            fixture.Target.CloseAsync().WithTimeout(TestTimeout)
        );

        Assert.That(exception.Message, Does.Contain("Detach failed"));
    }

    [
        Test,
        PuppeteerTest(
            "Target.test",
            "WorkerTarget close",
            "should reject worker() after the sessions to detach were collected"
        )
    ]
    public async Task ShouldRejectWorkerAfterTheSessionsToDetachWereCollected()
    {
        using var fixture = new ServiceWorkerTargetFixture();
        Task<WebWorker> worker = null;

        // `CloseAsync()` would not detach a session that `WorkerAsync()` attached now.
        fixture.Transport.OnDetach = id =>
        {
            worker = fixture.Target.WorkerAsync();
            fixture.Transport.Accept(id);
        };

        var closed = fixture.Target.CloseAsync();
        fixture.Target.CloseTaskWrapper.TrySetResult(true);
        await closed.WithTimeout(TestTimeout);

        Assert.ThrowsAsync<TargetClosedException>(() => worker);
    }

    /// <summary>
    /// A service worker target with one session that the target manager keeps for it. The session
    /// is attached through the connection, so the connection is its parent.
    /// </summary>
    private sealed class ServiceWorkerTargetFixture : IDisposable
    {
        private readonly Connection _connection;
        private readonly TaskQueue _screenshotTaskQueue = new();

        public ServiceWorkerTargetFixture()
        {
            Transport = new MockTransport();
            _connection = Connection.CreateFromTransport(Transport, new ConnectOptions());

            // The session is created before the browser, so that the browser's own target
            // manager does not handle it.
            Transport.EmitAttachedToTarget(SessionId, TargetId);
            var session = _connection.GetSession(SessionId);

            // There is no browser process to close, so disposing the connection is enough.
#pragma warning disable CA2000 // Dispose objects before losing scope
            var browser = new CdpBrowser(SupportedBrowser.Chrome, _connection, [], null, null);
#pragma warning restore CA2000 // Dispose objects before losing scope
            var targetManager = Substitute.For<ITargetManager>();
            targetManager.GetAttachedSessions(TargetId).Returns([session]);

            Target = new CdpWorkerTarget(
                new TargetInfo { TargetId = TargetId, Type = TargetType.ServiceWorker },
                null,
                (BrowserContext)browser.DefaultContext,
                targetManager,
                _ => Task.FromException<CDPSession>(new PuppeteerException("Attach failed")),
                _screenshotTaskQueue
            );
        }

        public CdpWorkerTarget Target { get; }

        public MockTransport Transport { get; }

        public void Dispose()
        {
            _connection.Dispose();
            Transport.Dispose();
            _screenshotTaskQueue.Dispose();
        }
    }

    /// <summary>
    /// Records the commands a connection sends and replies to them in place of the browser.
    /// </summary>
    private sealed class MockTransport : IConnectionTransport
    {
        public MockTransport() => OnDetach = Accept;

        public event EventHandler<TransportClosedEventArgs> Closed;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public List<string> ClosedTargetIds { get; } = [];

        public List<string> DetachedSessionIds { get; } = [];

        public Action<int> OnDetach { get; set; }

        public bool IsClosed { get; private set; }

        public void StopReading() { }

        public Task SendAsync(byte[] message)
        {
            using var doc = JsonDocument.Parse(message);
            var id = doc.RootElement.GetProperty("id").GetInt32();
            var method = doc.RootElement.GetProperty("method").GetString();
            var parameters = doc.RootElement.GetProperty("params");

            if (method == "Target.closeTarget")
            {
                ClosedTargetIds.Add(parameters.GetProperty("targetId").GetString());
                Receive($$$"""{"id":{{{id}}},"result":{"success":true}}""");
            }
            else if (method == "Target.detachFromTarget")
            {
                DetachedSessionIds.Add(parameters.GetProperty("sessionId").GetString());
                OnDetach(id);
            }

            return Task.CompletedTask;
        }

        public void Accept(int id) => Receive($$$"""{"id":{{{id}}},"result":{}}""");

        public void Reject(int id, string error) =>
            Receive($$$"""{"id":{{{id}}},"error":{"code":-32602,"message":"{{{error}}}"}}""");

        public void EmitAttachedToTarget(string sessionId, string targetId) =>
            Receive(
                $$$"""{"method":"Target.attachedToTarget","params":{"sessionId":"{{{sessionId}}}","targetInfo":{"targetId":"{{{targetId}}}","type":"service_worker"},"waitingForDebugger":false}}"""
            );

        public void EmitDetachedFromTarget(string sessionId) =>
            Receive(
                $$$"""{"method":"Target.detachedFromTarget","params":{"sessionId":"{{{sessionId}}}"}}"""
            );

        public void Dispose()
        {
            if (IsClosed)
            {
                return;
            }

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
