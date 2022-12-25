namespace PuppeteerSharp.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate for creation of <see cref="IConnectionTransport"/> instances.
    /// </summary>
    /// <param name="url">Chromium URL.</param>
    /// <param name="options">Connection options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{IConnectionTransport}"/> instance for the asynchronous socket create and connect operation.</returns>
    public delegate Task<IConnectionTransport> TransportFactory(Uri url, IConnectionOptions options, CancellationToken cancellationToken);
}
