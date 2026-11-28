using System;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.ConnectionTests;

/// <summary>
/// An in-memory transport which lets a test play the role of the remote end.
/// </summary>
internal class TestConnectionTransport : IConnectionTransport
{
    public event EventHandler<TransportClosedEventArgs> Closed;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public bool IsClosed { get; private set; }

    public static BidiConnection CreateBidiConnection(IConnectionTransport transport) =>
#pragma warning disable CA2000 // Ownership is transferred to the driver
        BidiConnection.Create(TimeSpan.FromSeconds(30), new PuppeteerConnection(transport));
#pragma warning restore CA2000

    public virtual Task SendAsync(byte[] message) => Task.CompletedTask;

    public void StopReading() { }

    public void Dispose() { }

    public void Close()
    {
        if (IsClosed)
        {
            return;
        }

        IsClosed = true;
        Closed?.Invoke(this, new TransportClosedEventArgs("Test transport closed"));
    }

    public void EmitMessage(string message) =>
        MessageReceived?.Invoke(
            this,
            new MessageReceivedEventArgs(Encoding.UTF8.GetBytes(message))
        );
}
