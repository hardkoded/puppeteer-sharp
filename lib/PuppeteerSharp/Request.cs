using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    public abstract class Request<TResponse> : IRequest, IInterceptableRequest
        where TResponse : IResponse
    {
        private readonly Dictionary<InterceptResolutionAction, int> _payloadPriorities = [];
        private readonly List<Func<IRequest, Task>> _interceptHandlers = [];
        private Payload _continueRequestOverrides = new();
        private ResponseData _responseForRequest;
        private RequestAbortErrorCode _abortErrorReason;

        /// <inheritdoc/>
        public string Id { get; internal init; }

        /// <inheritdoc/>
        public string InterceptionId { get; internal set; }

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
        public IPage Page => Frame?.Page;

        /// <inheritdoc/>
        public virtual WebWorker Worker { get; internal init; }

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

        internal Payload ContinueRequestOverrides => _continueRequestOverrides;

        internal ResponseData ResponseForRequest => _responseForRequest;

        internal RequestAbortErrorCode AbortErrorReason => _abortErrorReason;

        internal bool FromMemoryCache { get; set; }

        /// <summary>
        /// Gets the resolution state built from the calls made with a priority.
        /// </summary>
        protected InterceptResolutionState PendingResolutionState { get; private set; } =
            new(InterceptResolutionAction.None);

        /// <inheritdoc/>
        public async Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
        {
            VerifyInterception();

            if (!CanBeIntercepted())
            {
                return;
            }

            HttpUtils.ValidateHeaders(payloadOverrides?.Headers);

            if (priority is null)
            {
                await ContinueInternalAsync(payloadOverrides).ConfigureAwait(false);
                return;
            }

            if (ClaimInterceptionPayload(InterceptResolutionAction.Continue, priority.Value))
            {
                _continueRequestOverrides = payloadOverrides;
            }

            if (
                PendingResolutionState.Priority is null
                || priority > PendingResolutionState.Priority
            )
            {
                PendingResolutionState = new InterceptResolutionState(
                    InterceptResolutionAction.Continue,
                    priority
                );
                return;
            }

            if (priority == PendingResolutionState.Priority)
            {
                if (
                    PendingResolutionState.Action == InterceptResolutionAction.Abort
                    || PendingResolutionState.Action == InterceptResolutionAction.Respond
                )
                {
                    return;
                }

                PendingResolutionState.Action = InterceptResolutionAction.Continue;
            }
        }

        /// <inheritdoc/>
        public async Task RespondAsync(ResponseData response, int? priority = null)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            VerifyInterception();

            if (!CanBeIntercepted())
            {
                return;
            }

            if (priority is null)
            {
                await RespondInternalAsync(response).ConfigureAwait(false);
                return;
            }

            if (ClaimInterceptionPayload(InterceptResolutionAction.Respond, priority.Value))
            {
                _responseForRequest = response;
            }

            if (
                PendingResolutionState.Priority is null
                || priority > PendingResolutionState.Priority
            )
            {
                PendingResolutionState = new InterceptResolutionState(
                    InterceptResolutionAction.Respond,
                    priority
                );
                return;
            }

            if (priority == PendingResolutionState.Priority)
            {
                if (PendingResolutionState.Action == InterceptResolutionAction.Abort)
                {
                    return;
                }

                PendingResolutionState.Action = InterceptResolutionAction.Respond;
            }
        }

        /// <inheritdoc/>
        public async Task AbortAsync(
            RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed,
            int? priority = null
        )
        {
            VerifyInterception();

            if (!CanBeIntercepted())
            {
                return;
            }

            if (priority is null)
            {
                await AbortInternalAsync(errorCode).ConfigureAwait(false);
                return;
            }

            if (ClaimInterceptionPayload(InterceptResolutionAction.Abort, priority.Value))
            {
                _abortErrorReason = errorCode;
            }

            if (
                PendingResolutionState.Priority is null
                || priority >= PendingResolutionState.Priority
            )
            {
                PendingResolutionState = new InterceptResolutionState(
                    InterceptResolutionAction.Abort,
                    priority
                );
            }
        }

        /// <inheritdoc />
        public abstract Task<string> FetchPostDataAsync();

        /// <inheritdoc />
        public abstract Task<byte[]> ContentAsync();

        /// <inheritdoc/>
        void IInterceptableRequest.EnqueueInterceptionAction(Func<IRequest, Task> pendingHandler) =>
            _interceptHandlers.Add(pendingHandler);

        /// <summary>
        /// Awaits pending interception handlers and then decides how to fulfill the request interception.
        /// </summary>
        /// <returns>A task that completes when the interception has been fulfilled.</returns>
        internal virtual async Task FinalizeInterceptionsAsync()
        {
            foreach (var handler in _interceptHandlers)
            {
                await handler(this).ConfigureAwait(false);
            }

            _interceptHandlers.Clear();

            switch (InterceptResolutionState.Action)
            {
                case InterceptResolutionAction.Abort:
                    await AbortInternalAsync(AbortErrorReason).ConfigureAwait(false);
                    return;
                case InterceptResolutionAction.Respond:
                    if (ResponseForRequest is null)
                    {
                        throw new PuppeteerException("Response is missing for the interception");
                    }

                    await RespondInternalAsync(ResponseForRequest).ConfigureAwait(false);
                    return;
                case InterceptResolutionAction.Continue:
                    await ContinueInternalAsync(ContinueRequestOverrides).ConfigureAwait(false);
                    return;
            }
        }

        /// <summary>
        /// Aborts the request in the browser. Equivalent to upstream's <c>_abort</c>.
        /// </summary>
        /// <param name="errorCode">The abort error code.</param>
        /// <returns>A task that completes when the request has been aborted.</returns>
        internal abstract Task AbortInternalAsync(RequestAbortErrorCode errorCode);

        /// <summary>
        /// Responds to the request in the browser. Equivalent to upstream's <c>_respond</c>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A task that completes when the request has been fulfilled.</returns>
        internal abstract Task RespondInternalAsync(ResponseData response);

        /// <summary>
        /// Continues the request in the browser. Equivalent to upstream's <c>_continue</c>.
        /// </summary>
        /// <param name="overrides">The overrides.</param>
        /// <returns>A task that completes when the request has been continued.</returns>
        internal abstract Task ContinueInternalAsync(Payload overrides = null);

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

        /// <summary>
        /// In Cooperative Intercept Mode, an action takes its payload (the overrides of Continue,
        /// the response of Respond or the error reason of Abort) from the call with the highest priority,
        /// or from the latest of the calls that share that priority.
        /// </summary>
        /// <param name="action">The action whose payload is being provided.</param>
        /// <param name="priority">The priority of the call.</param>
        /// <returns><c>false</c> if a call with a higher priority has already provided the payload of the action.</returns>
        protected bool ClaimInterceptionPayload(InterceptResolutionAction action, int priority)
        {
            if (_payloadPriorities.TryGetValue(action, out var current) && priority < current)
            {
                return false;
            }

            _payloadPriorities[action] = priority;
            return true;
        }
    }
}
