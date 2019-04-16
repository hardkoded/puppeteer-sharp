using Microsoft.Extensions.Logging;
using PuppeteerSharp.Abstractions.Transport;

namespace PuppeteerSharp.Abstractions
{
    interface IConnection
    {
        string Url { get; }
        int Delay { get; }
        IConnectionTransport Transport { get; }
        bool IsClosed { get; set; }
        string CloseReason { get; set; }
        ILoggerFactory LoggerFactory { get; }
        void Dispose();
    }
}
