using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Abstractions.Input;

namespace PuppeteerSharp.Abstractions
{
    public interface IFrame
    {
        List<IFrame> ChildFrames { get; }
        string Name { get; set; }
        string Url { get; set; }
        bool Detached { get; set; }
        IFrame ParentFrame { get; set; }
        Task<IResponse> GoToAsync(string url, NavigationOptions options);
        Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null);
        Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null);
        Task<JToken> EvaluateExpressionAsync(string script);
        Task<T> EvaluateExpressionAsync<T>(string script);
        Task<JToken> EvaluateFunctionAsync(string script, params object[] args);
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);
        Task<IJSHandle> EvaluateFunctionHandleAsync(string function, params object[] args);
        Task<IExecutionContext> GetExecutionContextAsync();
        Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null);
        Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null);
        Task WaitForTimeoutAsync(int milliseconds);
        Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args);
        Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options);
        Task<string[]> SelectAsync(string selector, params string[] values);
        Task<IElementHandle> QuerySelectorAsync(string selector);
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);
        Task<IElementHandle[]> XPathAsync(string expression);
        Task<IElementHandle> AddStyleTagAsync(AddTagOptions options);
        Task<IElementHandle> AddScriptTagAsync(AddTagOptions options);
        Task<string> GetContentAsync();
        Task SetContentAsync(string html, NavigationOptions options = null);
        Task<string> GetTitleAsync();
        Task ClickAsync(string selector, ClickOptions options = null);
        Task HoverAsync(string selector);
        Task FocusAsync(string selector);
        Task TypeAsync(string selector, string text, TypeOptions options = null);
    }
}