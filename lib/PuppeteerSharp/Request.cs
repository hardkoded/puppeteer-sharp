using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class Request<TResponse>
        : IRequest, IInterceptableRequest
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
        public string PostData { get; internal init; }

        /// <inheritdoc/>
        public virtual Dictionary<string, string> Headers { get; protected set; }

        /// <inheritdoc/>
        public string Url { get; internal init; }

        /// <inheritdoc/>
        public IRequest[] RedirectChain => RedirectChainList.ToArray();

        /// <inheritdoc />
        public Initiator Initiator { get; internal init; }

        /// <inheritdoc />
        public bool HasPostData { get; internal init; }

        /// <inheritdoc />
        public abstract bool IsInterceptResolutionHandled { get; protected set; }

        /// <inheritdoc />
        public abstract InterceptResolutionState InterceptResolutionState { get; }

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
        public abstract Task ContinueAsync(Payload payloadOverrides = null, int? priority = null);

        /// <inheritdoc/>
        public abstract Task RespondAsync(ResponseData response, int? priority = null);

        /// <inheritdoc/>
        public abstract Task AbortAsync(
            RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed,
            int? priority = null);

        /// <inheritdoc />
        public abstract Task<string> FetchPostDataAsync();

        /// <inheritdoc/>
        void IInterceptableRequest.EnqueueInterceptionAction(Func<IRequest, Task> pendingHandler)
            => EnqueueInterceptionActionCore(pendingHandler);

        internal abstract Task FinalizeInterceptionsAsync();

        /// <summary>
        /// Enqueues an interception action to be executed when the request is finalized.
        /// </summary>
        /// <param name="pendingHandler">The handler to execute.</param>
        internal abstract void EnqueueInterceptionActionCore(Func<IRequest, Task> pendingHandler);

        /// <summary>
        /// Verifies that request interception is enabled and the request has not been handled yet.
        /// </summary>
        /// <exception cref="PuppeteerException">Thrown when interception is not enabled or request is already handled.</exception>
        protected abstract void VerifyInterception();

        /// <summary>
        /// Determines whether this request can be intercepted.
        /// </summary>
        /// <returns><c>true</c> if the request can be intercepted; otherwise, <c>false</c>.</returns>
        protected abstract bool CanBeIntercepted();
    }
}
