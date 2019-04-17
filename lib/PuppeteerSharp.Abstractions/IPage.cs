using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Abstractions.Input;
using PuppeteerSharp.Abstractions.Media;
using PuppeteerSharp.Abstractions.Mobile;

namespace PuppeteerSharp.Abstractions
{
    public interface IPage
    {
        int DefaultNavigationTimeout { get; set; }
        int DefaultTimeout { get; set; }
        IFrame MainFrame { get; }
        IFrame[] Frames { get; }
        IWorker[] Workers { get; }
        string Url { get; }
        ITarget Target { get; }
        IKeyboard Keyboard { get; }
        ITouchscreen Touchscreen { get; }
        IMouse Mouse { get; }
        ViewPortOptions Viewport { get; }
        IBrowser Browser { get; }
        IBrowserContext BrowserContext { get; }
        bool IsClosed { get; }
        Task SetGeolocationAsync(GeolocationOption options);
        Task<Dictionary<string, decimal>> MetricsAsync();
        Task TapAsync(string selector);
        Task<IElementHandle> QuerySelectorAsync(string selector);
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);
        Task<IJSHandle> QuerySelectorAllHandleAsync(string selector);
        Task<IElementHandle[]> XPathAsync(string expression);
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);
        Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args);
        Task EvaluateOnNewDocumentAsync(string pageFunction, params object[] args);
        Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle);
        Task SetRequestInterceptionAsync(bool value);
        Task SetOfflineModeAsync(bool value);
        Task<CookieParam[]> GetCookiesAsync(params string[] urls);
        Task SetCookieAsync(params CookieParam[] cookies);
        Task DeleteCookieAsync(params CookieParam[] cookies);
        Task<IElementHandle> AddScriptTagAsync(AddTagOptions options);
        Task<IElementHandle> AddScriptTagAsync(string url);
        Task<IElementHandle> AddStyleTagAsync(AddTagOptions options);
        Task<IElementHandle> AddStyleTagAsync(string url);
        Task ExposeFunctionAsync(string name, Action puppeteerFunction);
        Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction);
        Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction);
        Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction);
        Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction);
        Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction);
        Task<string> GetContentAsync();
        Task SetContentAsync(string html, NavigationOptions options = null);
        Task<IResponse> GoToAsync(string url, NavigationOptions options);
        Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null);
        Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil);
        Task PdfAsync(string file);
        Task PdfAsync(string file, PdfOptions options);
        Task<Stream> PdfStreamAsync();
        Task<Stream> PdfStreamAsync(PdfOptions options);
        Task<byte[]> PdfDataAsync();
        Task<byte[]> PdfDataAsync(PdfOptions options);
        Task SetJavaScriptEnabledAsync(bool enabled);
        Task SetBypassCSPAsync(bool enabled);
        Task EmulateMediaAsync(MediaType media);
        Task SetViewportAsync(ViewPortOptions viewport);
        Task EmulateAsync(DeviceDescriptor options);
        Task ScreenshotAsync(string file);
        Task ScreenshotAsync(string file, ScreenshotOptions options);
        Task<Stream> ScreenshotStreamAsync();
        Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options);
        Task<string> ScreenshotBase64Async();
        Task<string> ScreenshotBase64Async(ScreenshotOptions options);
        Task<byte[]> ScreenshotDataAsync();
        Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options);
        Task<string> GetTitleAsync();
        Task CloseAsync(PageCloseOptions options = null);
        Task SetCacheEnabledAsync(bool enabled = true);
        Task ClickAsync(string selector, ClickOptions options = null);
        Task HoverAsync(string selector);
        Task FocusAsync(string selector);
        Task TypeAsync(string selector, string text, TypeOptions options = null);
        Task<JToken> EvaluateExpressionAsync(string script);
        Task<T> EvaluateExpressionAsync<T>(string script);
        Task<JToken> EvaluateFunctionAsync(string script, params object[] args);
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);
        Task SetUserAgentAsync(string userAgent);
        Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers);
        Task AuthenticateAsync(Credentials credentials);
        Task<IResponse> ReloadAsync(NavigationOptions options);
        Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null);
        Task<string[]> SelectAsync(string selector, params string[] values);
        Task WaitForTimeoutAsync(int milliseconds);
        Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args);
        Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args);
        Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null);
        Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null);
        Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null);
        Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null);
        Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null);
        Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null);
        Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null);
        Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null);
        Task<IResponse> GoBackAsync(NavigationOptions options = null);
        Task<IResponse> GoForwardAsync(NavigationOptions options = null);
        Task SetBurstModeOffAsync();
        void Dispose();
    }
}