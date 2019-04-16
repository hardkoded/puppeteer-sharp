using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Abstractions
{
    interface ICDPSession
    {
        TargetType TargetType { get; }
        string SessionId { get; }
        bool IsClosed { get; set; }
        string CloseReason { get; set; }
        ILoggerFactory LoggerFactory { get; }
        Task<T> SendAsync<T>(string method, object args = null);
        Task<JObject> SendAsync(string method, object args = null, bool waitForCallback = true);
        Task DetachAsync();
    }

}
