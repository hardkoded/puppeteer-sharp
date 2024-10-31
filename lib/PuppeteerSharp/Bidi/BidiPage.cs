// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiPage : Page
{
    internal BidiPage(BidiBrowserContext browserContext, BrowsingContext browsingContext) : base(browserContext.ScreenshotTaskQueue)
    {
        BrowserContext = browserContext;
        BidiMainFrame = BidiFrame.From(this, null, browsingContext);
    }

    /// <inheritdoc />
    public override IBrowserContext BrowserContext { get; }

    /// <inheritdoc />
    public override CDPSession Client { get; }

    /// <inheritdoc />
    public override Target Target { get; }

    /// <inheritdoc />
    public override IFrame[] Frames { get; }

    /// <inheritdoc />
    public override WebWorker[] Workers { get; }

    /// <inheritdoc />
    public override bool IsJavaScriptEnabled { get; }

    /// <inheritdoc/>
    public override IFrame MainFrame => BidiMainFrame;

    internal BidiBrowser BidiBrowser => (BidiBrowser)BrowserContext.Browser;

    internal BidiFrame BidiMainFrame { get; set; }

    internal ConcurrentDictionary<string, string> ExtraHttpHeaders { get; set; } = new();

    internal ConcurrentDictionary<string, string> UserAgentHeaders { get; set; } = new();

    /// <inheritdoc />
    protected override Browser Browser { get; }

    /// <inheritdoc />
    public override Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task AuthenticateAsync(Credentials credentials) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task BringToFrontAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateVisionDeficiencyAsync(VisionDeficiency type) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateTimezoneAsync(string timezoneId) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateCPUThrottlingAsync(decimal? factor = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> ReloadAsync(NavigationOptions options) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> GoBackAsync(NavigationOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> GoForwardAsync(NavigationOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetBurstModeOffAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetGeolocationAsync(GeolocationOption options) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetJavaScriptEnabledAsync(bool enabled) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetBypassCSPAsync(bool enabled) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetCacheEnabledAsync(bool enabled = true) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateMediaTypeAsync(MediaType type) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetViewportAsync(ViewPortOptions viewport) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetCookieAsync(params CookieParam[] cookies) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task CloseAsync(PageCloseOptions options = null)
    {
        try
        {
            await BidiMainFrame.BrowsingContext.CloseAsync(options?.RunBeforeUnload).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return;
        }
    }

    /// <inheritdoc />
    public override Task DeleteCookieAsync(params CookieParam[] cookies) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetDragInterceptionAsync(bool enabled) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<Dictionary<string, decimal>> MetricsAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task RemoveExposedFunctionAsync(string name) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetBypassServiceWorkerAsync(bool bypass) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetRequestInterceptionAsync(bool value) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task SetOfflineModeAsync(bool value) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<CookieParam[]> GetCookiesAsync(params string[] urls) => throw new NotImplementedException();

    internal static BidiPage From(BidiBrowserContext browserContext, BrowsingContext browsingContext)
    {
        var page = new BidiPage(browserContext, browsingContext);
        page.Initialize();
        return page;
    }

    /// <inheritdoc />
    protected override Task<byte[]> PdfInternalAsync(string file, PdfOptions options) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override Task ExposeFunctionAsync(string name, Delegate puppeteerFunction) => throw new NotImplementedException();

    private void Initialize()
    {
    }
}
