using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IRequest
    {
        IResponse Response { get; set; }
        string Failure { get; set; }
        string RequestId { get; set; }
        string InterceptionId { get; set; }
        ResourceType ResourceType { get; set; }
        IFrame Frame { get; }
        bool IsNavigationRequest { get; }
        HttpMethod Method { get; set; }
        object PostData { get; set; }
        Dictionary<string, string> Headers { get; set; }
        string Url { get; set; }
        IRequest[] RedirectChain { get; }
        Task ContinueAsync(Payload overrides = null);
        Task RespondAsync(ResponseData response);
        Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed);
    }

}
