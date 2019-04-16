using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IWebSocketTransport
    {
        bool IsClosed { get; set; }
        Task SendAsync(string message);
        void StopReading();
        void Dispose();
    }

}
