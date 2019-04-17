using System;

namespace PuppeteerSharp.Transport
{
    /// <summary>
    /// Connection transport abstraction.
    /// </summary>
    public interface IConnectionTransport : Abstractions.Transport.IConnectionTransport, IDisposable
    {
    }
}
