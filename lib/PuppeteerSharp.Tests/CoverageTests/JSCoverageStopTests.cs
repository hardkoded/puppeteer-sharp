using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.CoverageTests;

public class JSCoverageStopTests
{
    private const string SessionId = "session-id";

    [
        Test,
        PuppeteerTest(
            "coverage.spec",
            "Coverage specs JSCoverage",
            "should wait for pending script sources when stopping"
        )
    ]
    public async Task ShouldWaitForPendingScriptSourcesWhenStopping()
    {
        using var transport = new MockTransport();
        using var connection = Connection.CreateFromTransport(transport, new ConnectOptions());
        transport.Attach();
        var coverage = new JSCoverage(connection.GetSession(SessionId));
        await coverage.StartAsync(new CoverageStartOptions());
        transport.ParseScript();

        var stopTask = coverage.StopAsync();

        // The script source has not arrived, so stopping has to wait for it.
        Assert.That(await Task.WhenAny(stopTask, Task.Delay(500)), Is.Not.SameAs(stopTask));

        transport.ReplyWithScriptSource();
        var entries = await stopTask;

        Assert.That(entries, Has.Exactly(1).Items);
        Assert.That(entries[0].Text, Is.EqualTo("abc"));
    }

    /// <summary>
    /// Replies to commands in place of the browser, holding back the script source until asked.
    /// </summary>
    private sealed class MockTransport : IConnectionTransport
    {
        private int _scriptSourceCommandId;

        public event EventHandler<TransportClosedEventArgs> Closed;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public bool IsClosed { get; private set; }

        public void StopReading() { }

        public Task SendAsync(byte[] message)
        {
            using var doc = JsonDocument.Parse(message);
            var id = doc.RootElement.GetProperty("id").GetInt32();

            switch (doc.RootElement.GetProperty("method").GetString())
            {
                case "Debugger.getScriptSource":
                    _scriptSourceCommandId = id;
                    break;
                case "Profiler.takePreciseCoverage":
                    Reply(
                        id,
                        """{"result":[{"scriptId":"1","url":"a.js","functions":[{"functionName":"","isBlockCoverage":false,"ranges":[{"startOffset":0,"endOffset":3,"count":1}]}]}]}"""
                    );
                    break;
                default:
                    Reply(id, "{}");
                    break;
            }

            return Task.CompletedTask;
        }

        public void Attach() =>
            Receive(
                $$$"""{"method":"Target.attachedToTarget","params":{"sessionId":"{{{SessionId}}}","targetInfo":{"targetId":"target-id","type":"page"},"waitingForDebugger":false}}"""
            );

        public void ParseScript() =>
            Receive(
                $$$"""{"sessionId":"{{{SessionId}}}","method":"Debugger.scriptParsed","params":{"scriptId":"1","url":"a.js"}}"""
            );

        public void ReplyWithScriptSource() =>
            Reply(_scriptSourceCommandId, """{"scriptSource":"abc"}""");

        public void Dispose()
        {
            IsClosed = true;
            Closed?.Invoke(this, new TransportClosedEventArgs("disposed"));
        }

        private void Reply(int id, string result) =>
            Receive($$$"""{"sessionId":"{{{SessionId}}}","id":{{{id}}},"result":{{{result}}}}""");

        private void Receive(string message) =>
            MessageReceived?.Invoke(
                this,
                new MessageReceivedEventArgs(Encoding.UTF8.GetBytes(message))
            );
    }
}
