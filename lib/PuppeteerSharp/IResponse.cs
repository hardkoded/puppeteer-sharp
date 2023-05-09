using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="Response"/> class represents responses which are received by page.
    /// </summary>
    /// <seealso cref="Page.GoAsync(int, NavigationOptions)"/>
    /// <seealso cref="IPage.GoForwardAsync(NavigationOptions)"/>
    /// <seealso cref="IPage.ReloadAsync(int?, WaitUntilNavigation[])"/>
    /// <seealso cref="IPage.Response"/>
    /// <seealso cref="IPage.WaitForResponseAsync(Func{IResponse, bool}, WaitForOptions)"/>
    public interface IResponse
    {
        /// <summary>
        /// Contains the URL of the response.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// An object with HTTP headers associated with the response. All header names are lower-case.
        /// </summary>
        /// <value>The headers.</value>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Contains the status code of the response.
        /// </summary>
        /// <value>The status.</value>
        HttpStatusCode Status { get; }

        /// <summary>
        /// Contains a boolean stating whether the response was successful (status in the range 200-299) or not.
        /// </summary>
        /// <value><c>true</c> if ok; otherwise, <c>false</c>.</value>
        bool Ok { get; }

        /// <summary>
        /// A matching <see cref="Request"/> object.
        /// </summary>
        /// <value>The request.</value>
        IRequest Request { get; }

        /// <summary>
        /// True if the response was served from either the browser's disk cache or memory cache.
        /// </summary>
        bool FromCache { get; }

        /// <summary>
        /// Gets or sets the security details.
        /// </summary>
        /// <value>The security details.</value>
        SecurityDetails SecurityDetails { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Response"/> was served by a service worker.
        /// </summary>
        /// <value><c>true</c> if the <see cref="Response"/> was served by a service worker; otherwise, <c>false</c>.</value>
        bool FromServiceWorker { get; }

        /// <summary>
        /// Contains the status text of the response (e.g. usually an "OK" for a success).
        /// </summary>
        /// <value>The status text.</value>
        string StatusText { get; }

        /// <summary>
        /// Remove server address.
        /// </summary>
        RemoteAddress RemoteAddress { get; }

        /// <summary>
        /// A <see cref="Frame"/> that initiated this request. Or null if navigating to error pages.
        /// </summary>
        IFrame Frame { get; }

        /// <summary>
        /// Returns a Task which resolves to a buffer with response body.
        /// </summary>
        /// <returns>A Task which resolves to a buffer with response body.</returns>
        ValueTask<byte[]> BufferAsync();

        /// <summary>
        /// Returns a Task which resolves to a text representation of response body.
        /// </summary>
        /// <returns>A Task which resolves to a text representation of response body.</returns>
        Task<string> TextAsync();

        /// <summary>
        /// Returns a Task which resolves to a <see cref="JsonObject"/> representation of response body.
        /// </summary>
        /// <seealso cref="JsonAsync{T}"/>
        /// <returns>A Task which resolves to a <see cref="JsonObject"/> representation of response body.</returns>
        Task<JsonObject> JsonAsync();

        /// <summary>
        /// Returns a Task which resolves to a <typeparamref name="T"/> representation of response body.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <seealso cref="JsonAsync"/>
        /// <returns>A Task which resolves to a <typeparamref name="T"/> representation of response body.</returns>
        Task<T> JsonAsync<T>();
    }
}
