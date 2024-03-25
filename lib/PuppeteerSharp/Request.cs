using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class Request<TResponse>
        : IRequest
        where TResponse : IResponse
    {
        /// <inheritdoc/>
        public string Id { get; internal init; }

        /// <inheritdoc/>
        public string InterceptionId { get; internal init; }

        /// <inheritdoc/>
        public string FailureText { get; internal set; }

        /// <inheritdoc cref="Response"/>
        public virtual TResponse Response { get; internal set; }

        /// <inheritdoc/>
        IResponse IRequest.Response => Response;

        /// <inheritdoc/>
        public ResourceType ResourceType { get; internal init; }

        /// <inheritdoc/>
        public IFrame Frame { get; internal init; }

        /// <inheritdoc/>
        public bool IsNavigationRequest { get; internal init; }

        /// <inheritdoc/>
        public HttpMethod Method { get; internal init; }

        /// <inheritdoc/>
        public object PostData { get; internal init; }

        /// <inheritdoc/>
        public Dictionary<string, string> Headers { get; internal init; }

        /// <inheritdoc/>
        public string Url { get; internal init; }

        /// <inheritdoc/>
        public IRequest[] RedirectChain => RedirectChainList.ToArray();

        /// <inheritdoc />
        public Initiator Initiator { get; internal init; }

        /// <inheritdoc />
        public bool HasPostData { get; internal init; }

        internal List<IRequest> RedirectChainList { get; init; }

        internal abstract Payload ContinueRequestOverrides
        {
            get;
        }

        internal abstract ResponseData ResponseForRequest
        {
            get;
        }

        internal abstract RequestAbortErrorCode AbortErrorReason
        {
            get;
        }

        internal bool FromMemoryCache { get; set; }

        /// <inheritdoc/>
        public abstract Task ContinueAsync(Payload overrides = null, int? priority = null);

        /// <inheritdoc/>
        public abstract Task RespondAsync(ResponseData response, int? priority = null);

        /// <inheritdoc/>
        public abstract Task AbortAsync(
            RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed,
            int? priority = null);

        /// <inheritdoc />
        public abstract Task<string> FetchPostDataAsync();

        internal abstract Task FinalizeInterceptionsAsync();

        internal abstract void EnqueueInterceptionAction(Func<IRequest, Task> pendingHandler);
    }
}
