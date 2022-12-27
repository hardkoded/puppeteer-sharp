using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Whenever the page sends a request, the following events are emitted by puppeteer's page:
    /// <see cref="IPage.Request"/> emitted when the request is issued by the page.
    /// <see cref="IPage.Response"/> emitted when/if the response is received for the request.
    /// <see cref="IPage.RequestFinished"/> emitted when the response body is downloaded and the request is complete.
    ///
    /// If request fails at some point, then instead of <see cref="IPage.RequestFinished"/> event (and possibly instead of <see cref="IPage.Response"/> event), the <see cref="IPage.RequestFailed"/> event is emitted.
    ///
    /// If request gets a 'redirect' response, the request is successfully finished with the <see cref="IPage.RequestFinished"/> event, and a new request is issued to a redirected url.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Responsed attached to the request.
        /// </summary>
        /// <value>The response.</value>
        public IResponse Response { get; }

        /// <summary>
        /// Gets the failure.
        /// </summary>
        /// <value>The failure.</value>
        public string Failure { get; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        string RequestId { get; }

        /// <summary>
        /// Gets the interception identifier.
        /// </summary>
        /// <value>The interception identifier.</value>
        string InterceptionId { get; }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        /// <value>The type of the resource.</value>
        ResourceType ResourceType { get; }

        /// <summary>
        /// Gets the frame.
        /// </summary>
        /// <value>The frame.</value>
        IFrame Frame { get; }

        /// <summary>
        /// Gets whether this request is driving frame's navigation.
        /// </summary>
        bool IsNavigationRequest { get; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        /// <value>HTTP method.</value>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets the post data.
        /// </summary>
        /// <value>The post data.</value>
        object PostData { get; }

        /// <summary>
        /// Gets the HTTP headers.
        /// </summary>
        /// <value>HTTP headers.</value>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; }

        /// <summary>
        /// A redirectChain is a chain of requests initiated to fetch a resource.
        /// If there are no redirects and the request was successful, the chain will be empty.
        /// If a server responds with at least a single redirect, then the chain will contain all the requests that were redirected.
        /// redirectChain is shared between all the requests of the same chain.
        /// </summary>
        /// <example>
        /// For example, if the website http://example.com has a single redirect to https://example.com, then the chain will contain one request:
        /// <code>
        /// var response = await page.GoToAsync("http://example.com");
        /// var chain = response.Request.RedirectChain;
        /// Console.WriteLine(chain.Length); // 1
        /// Console.WriteLine(chain[0].Url); // 'http://example.com'
        /// </code>
        /// If the website https://google.com has no redirects, then the chain will be empty:
        /// <code>
        /// var response = await page.GoToAsync("https://google.com");
        /// var chain = response.Request.RedirectChain;
        /// Console.WriteLine(chain.Length); // 0
        /// </code>
        /// </example>
        /// <value>The redirect chain.</value>
        IRequest[] RedirectChain { get; }

        /// <summary>
        /// Continues request with optional request overrides. To use this, request interception should be enabled with <see cref="IPage.SetRequestInterceptionAsync(bool)"/>. Exception is immediately thrown if the request interception is not enabled.
        /// If the URL is set it won't perform a redirect. The request will be silently forwarded to the new url. For example, the address bar will show the original url.
        /// </summary>
        /// <param name="payloadOverrides">Optional request overwrites.</param>
        /// <returns>Task.</returns>
        Task ContinueAsync(Payload payloadOverrides = null);

        /// <summary>
        /// Fulfills request with given response. To use this, request interception should be enabled with <see cref="IPage.SetRequestInterceptionAsync(bool)"/>. Exception is thrown if request interception is not enabled.
        /// </summary>
        /// <param name="response">Response that will fulfill this request.</param>
        /// <returns>Task.</returns>
        Task RespondAsync(ResponseData response);

        /// <summary>
        /// Aborts request. To use this, request interception should be enabled with <see cref="IPage.SetRequestInterceptionAsync(bool)"/>.
        /// Exception is immediately thrown if the request interception is not enabled.
        /// </summary>
        /// <param name="errorCode">Optional error code. Defaults to <see cref="RequestAbortErrorCode.Failed"/>.</param>
        /// <returns>Task.</returns>
        Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed);
    }
}
