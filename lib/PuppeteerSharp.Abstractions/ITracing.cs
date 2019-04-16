using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface ITracing
    {
        Task StartAsync(TracingOptions options);
        Task<string> StopAsync();
    }

}
