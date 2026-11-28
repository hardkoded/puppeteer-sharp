using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ConnectionTests;

public class BidiConnectionTests
{
    [
        Test,
        PuppeteerTest(
            "Connection.test",
            "WebDriver BiDi Connection",
            "emits closed once when transport closes or dispose is called"
        )
    ]
    public async Task ShouldEmitClosedOnceWhenTransportClosesOrDisposeIsCalled()
    {
        using var transport = new TestConnectionTransport();
        await using var connection = TestConnectionTransport.CreateBidiConnection(transport);
        await connection.StartAsync("ws://127.0.0.1");

        var closedCount = 0;
        var closed = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        connection.Closed += (_, _) =>
        {
            Interlocked.Increment(ref closedCount);
            closed.TrySetResult(true);
        };

        transport.Close();
        await closed.Task.WithTimeout(5_000);
        Assert.That(closedCount, Is.EqualTo(1));
        Assert.That(connection.IsClosed, Is.True);

        await connection.StopAsync();
        await connection.DisposeAsync();
        Assert.That(closedCount, Is.EqualTo(1));
    }
}
