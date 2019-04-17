using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Abstractions
{
    public interface IResponse
    {
        string Url { get; }
        Dictionary<string, string> Headers { get; }
        HttpStatusCode Status { get; }
        bool Ok { get; }
        IRequest Request { get; }
        bool FromCache { get; }
        SecurityDetails SecurityDetails { get; }
        bool FromServiceWorker { get; }
        string StatusText { get; }
        RemoteAddress RemoteAddress { get; }
        IFrame Frame { get; }
        ValueTask<byte[]> BufferAsync();
        Task<string> TextAsync();
        Task<JObject> JsonAsync();
        Task<T> JsonAsync<T>();
    }
}