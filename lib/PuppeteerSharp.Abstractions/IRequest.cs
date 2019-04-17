using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    public interface IRequest
    {
        IResponse Response { get; }
        string Failure { get; }
        string RequestId { get; }
        string InterceptionId { get; }
        ResourceType ResourceType { get; }
        IFrame Frame { get; }
        bool IsNavigationRequest { get; }
        HttpMethod Method { get; }
        object PostData { get; }
        Dictionary<string, string> Headers { get; }
        string Url { get; }
        IRequest[] RedirectChain { get; }
        Task ContinueAsync(Payload overrides = null);
        Task RespondAsync(ResponseData response);
        Task AbortAsync();
    }
}