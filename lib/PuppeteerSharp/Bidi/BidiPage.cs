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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Media;
using WebDriverBiDi.BrowsingContext;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiPage : Page
{
    private readonly CdpEmulationManager _cdpEmulationManager;
    private InternalNetworkConditions _emulatedNetworkConditions;

    internal BidiPage(BidiBrowserContext browserContext, BrowsingContext browsingContext) : base(browserContext.ScreenshotTaskQueue)
    {
        BrowserContext = browserContext;
        Browser = browserContext.Browser;
        BidiMainFrame = BidiFrame.From(this, null, browsingContext);
        _cdpEmulationManager = new CdpEmulationManager(BidiMainFrame.Client);
        Mouse = new BidiMouse(this);
    }

    /// <inheritdoc />
    public override IBrowserContext BrowserContext { get; }

    /// <inheritdoc />
    public override CDPSession Client { get; }

    /// <inheritdoc />
    public override Target Target { get; }

    /// <inheritdoc />
    public override IFrame[] Frames
    {
        get
        {
            var frames = new List<IFrame> { BidiMainFrame };
            for (var i = 0; i < frames.Count; i++)
            {
                frames.AddRange(frames[i].ChildFrames);
            }

            return [.. frames];
        }
    }

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
    public override async Task BringToFrontAsync()
        => await BidiMainFrame.BrowsingContext.ActivateAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public override Task EmulateVisionDeficiencyAsync(VisionDeficiency type) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateTimezoneAsync(string timezoneId) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null)
        => _cdpEmulationManager.EmulateIdleStateAsync(idleOverrides);

    /// <inheritdoc />
    public override Task EmulateCPUThrottlingAsync(decimal? factor = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task<IResponse> ReloadAsync(NavigationOptions options)
    {
        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = BidiMainFrame.BrowsingContext.ReloadAsync();

        try
        {
            await Task.WhenAll(waitForNavigationTask, navigationTask).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("no such history entry"))
            {
                return null;
            }

            throw new NavigationException(ex.Message, ex);
        }

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> GoBackAsync(NavigationOptions options = null) => GoAsync(-1, options);

    /// <inheritdoc />
    public override Task<IResponse> GoForwardAsync(NavigationOptions options = null) => GoAsync(1, options);

    /// <inheritdoc />
    public override Task SetBurstModeOffAsync() => throw new NotImplementedException();

    /// <inheritdoc />
    public override Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task SetGeolocationAsync(GeolocationOption options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var longitude = options.Longitude;
        var latitude = options.Latitude;
        var accuracy = options.Accuracy;

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentException($"Invalid longitude '{longitude}': precondition -180 <= LONGITUDE <= 180 failed.");
        }

        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentException($"Invalid latitude '{latitude}': precondition -90 <= LATITUDE <= 90 failed.");
        }

        if (accuracy < 0)
        {
            throw new ArgumentException($"Invalid accuracy '{accuracy}': precondition 0 <= ACCURACY failed.");
        }

        var coordinates = new WebDriverBiDi.Emulation.GeolocationCoordinates((double)longitude, (double)latitude)
        {
            Accuracy = (double?)accuracy,
        };

        var commandParameters = new WebDriverBiDi.Emulation.SetGeolocationOverrideCoordinatesCommandParameters
        {
            Coordinates = coordinates,
        };

        commandParameters.Contexts.Add(BidiMainFrame.BrowsingContext.Id);

        await BidiMainFrame.BrowsingContext.Session.Driver.Emulation.SetGeolocationOverrideAsync(commandParameters).ConfigureAwait(false);
    }

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
    public override async Task SetViewportAsync(ViewPortOptions viewport)
    {
        if (viewport == null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        if (!BidiBrowser.CdpSupported)
        {
            await BidiMainFrame.BrowsingContext.SetViewportAsync(
                new SetViewportOptions()
                {
                    Viewport = viewport?.Width > 0 && viewport?.Height > 0
                        ? new WebDriverBiDi.BrowsingContext.Viewport()
                        {
                            Width = (ulong)viewport.Width,
                            Height = (ulong)viewport.Height,
                        }
                        : null,
                    DevicePixelRatio = viewport?.DeviceScaleFactor,
                }).ConfigureAwait(false);
            Viewport = viewport;
            return;
        }

        var needsReload = await _cdpEmulationManager.EmulateViewportAsync(viewport).ConfigureAwait(false);
        Viewport = viewport;

        if (needsReload)
        {
            await ReloadAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task SetCookieAsync(params CookieParam[] cookies)
    {
        if (cookies == null)
        {
            throw new ArgumentNullException(nameof(cookies));
        }

        var pageUrl = Url;
        var pageUrlStartsWithHttp = pageUrl.StartsWith("http", StringComparison.Ordinal);

        foreach (var cookie in cookies)
        {
            var cookieUrl = cookie.Url ?? string.Empty;
            if (string.IsNullOrEmpty(cookieUrl) && pageUrlStartsWithHttp)
            {
                cookieUrl = pageUrl;
            }

            if (cookieUrl == "about:blank")
            {
                throw new PuppeteerException($"Blank page can not have cookie \"{cookie.Name}\"");
            }

            if (cookieUrl.StartsWith("data:", StringComparison.Ordinal))
            {
                throw new PuppeteerException($"Data URL page can not have cookie \"{cookie.Name}\"");
            }

            // TODO: Support Chrome cookie partition keys
            if (!string.IsNullOrEmpty(cookie.PartitionKey))
            {
                throw new PuppeteerException("BiDi only allows domain partition keys");
            }

            Uri normalizedUrl = null;
            if (Uri.TryCreate(cookieUrl, UriKind.Absolute, out var parsedUrl))
            {
                normalizedUrl = parsedUrl;
            }

            var domain = cookie.Domain ?? normalizedUrl?.Host;
            if (string.IsNullOrEmpty(domain))
            {
                throw new MessageException("At least one of the url and domain needs to be specified");
            }

            // Automatically set secure flag for HTTPS URLs if not explicitly provided
            var cookieToUse = cookie;
            if (!cookie.Secure.HasValue && normalizedUrl?.Scheme == "https")
            {
                // Create a copy to avoid mutating the input parameter
                cookieToUse = new CookieParam
                {
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Url = cookie.Url,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Secure = true,
                    HttpOnly = cookie.HttpOnly,
                    Expires = cookie.Expires,
                    SameSite = cookie.SameSite,
                    PartitionKey = cookie.PartitionKey,
                };
            }

            var bidiCookie = BidiCookieHelper.PuppeteerToBidiCookie(cookieToUse, domain);

            await BidiBrowser.Driver.Storage.SetCookieAsync(new WebDriverBiDi.Storage.SetCookieCommandParameters(bidiCookie)
            {
                Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
            }).ConfigureAwait(false);
        }
    }

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
    public override async Task DeleteCookieAsync(params CookieParam[] cookies)
    {
        await Task.WhenAll(cookies.Select(async cookie =>
        {
            var cookieUrl = cookie.Url ?? Url;
            Uri normalizedUrl = null;
            if (Uri.TryCreate(cookieUrl, UriKind.Absolute, out var parsedUrl))
            {
                normalizedUrl = parsedUrl;
            }

            var domain = cookie.Domain ?? normalizedUrl?.Host;
            if (string.IsNullOrEmpty(domain))
            {
                throw new MessageException("At least one of the url and domain needs to be specified");
            }

            var filter = new WebDriverBiDi.Storage.CookieFilter
            {
                Domain = domain,
                Name = cookie.Name,
            };

            if (!string.IsNullOrEmpty(cookie.Path))
            {
                filter.Path = cookie.Path;
            }

            await BidiBrowser.Driver.Storage.DeleteCookiesAsync(new WebDriverBiDi.Storage.DeleteCookiesCommandParameters
            {
                Filter = filter,
                Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
            }).ConfigureAwait(false);
        })).ConfigureAwait(false);
    }

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
    public override async Task SetOfflineModeAsync(bool value)
    {
        if (!BidiBrowser.CdpSupported)
        {
            throw new NotSupportedException();
        }

        _emulatedNetworkConditions ??= new InternalNetworkConditions
        {
            Offline = false,
            Upload = -1,
            Download = -1,
            Latency = 0,
        };

        _emulatedNetworkConditions.Offline = value;
        await ApplyNetworkConditionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions) => throw new NotImplementedException();

    /// <inheritdoc />
    public override async Task<CookieParam[]> GetCookiesAsync(params string[] urls)
    {
        if (urls == null)
        {
            throw new ArgumentNullException(nameof(urls));
        }

        var normalizedUrls = (urls.Length > 0 ? urls : [Url]).Select(url => new Uri(url)).ToArray();

        var result = await BidiBrowser.Driver.Storage.GetCookiesAsync(new WebDriverBiDi.Storage.GetCookiesCommandParameters
        {
            Partition = new WebDriverBiDi.Storage.BrowsingContextPartitionDescriptor(BidiMainFrame.BrowsingContext.Id),
        }).ConfigureAwait(false);

        return result.Cookies
            .Select(BidiCookieHelper.BidiToPuppeteerCookie)
            .Where(cookie => normalizedUrls.Any(url => BidiCookieHelper.TestUrlMatchCookie(cookie, url)))
            .ToArray();
    }

    internal static BidiPage From(BidiBrowserContext browserContext, BrowsingContext browsingContext)
    {
        var page = new BidiPage(browserContext, browsingContext);
        page.Initialize();
        return page;
    }

    internal new void OnPageError(PageErrorEventArgs e) => base.OnPageError(e);

    /// <inheritdoc />
    protected override Task<byte[]> PdfInternalAsync(string file, PdfOptions options) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override async Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options)
    {
        Debug.Assert(options != null, nameof(options) + " != null");

        if (options.OmitBackground)
        {
            throw new PuppeteerException("BiDi does not support 'omitBackground'.");
        }

        if (options.OptimizeForSpeed == true)
        {
            throw new PuppeteerException("BiDi does not support 'optimizeForSpeed'.");
        }

        if (options.FromSurface == true)
        {
            throw new PuppeteerException("BiDi does not support 'fromSurface'.");
        }

        if (options.Clip != null && options.Clip.Scale != 1)
        {
            throw new PuppeteerException("BiDi does not support 'scale' in 'clip'.");
        }

        BoundingBox box = null;
        if (options.Clip != null)
        {
            if (options.CaptureBeyondViewport)
            {
                box = options.Clip;
            }
            else
            {
                // The clip is always with respect to the document coordinates, so we
                // need to convert this to viewport coordinates when we aren't capturing
                // beyond the viewport.
                var points = await EvaluateFunctionAsync<decimal[]>(@"() => {
                    if (!window.visualViewport) {
                        throw new Error('window.visualViewport is not supported.');
                    }
                    return [
                        window.visualViewport.pageLeft,
                        window.visualViewport.pageTop,
                    ];
                }").ConfigureAwait(false);

                box = new Clip
                {
                    X = options.Clip.X - points[0],
                    Y = options.Clip.Y - points[1],
                    Width = options.Clip.Width,
                    Height = options.Clip.Height,
                };
            }
        }

        var fileType = options.Type switch
        {
            ScreenshotType.Jpeg => "jpg",
            ScreenshotType.Png => "png",
            ScreenshotType.Webp => "webp",
            _ => "png",
        };

        var parameters = new ScreenshotParameters()
        {
            Origin = options.CaptureBeyondViewport ? ScreenshotOrigin.Document : ScreenshotOrigin.Viewport,
            Format = new ImageFormat()
            {
                Type = $"image/{fileType}",
                Quality = options.Quality / 100,
            },
        };

        if (box != null)
        {
            parameters.Clip = new BoxClipRectangle
            {
                X = (double)box.X,
                Y = (double)box.Y,
                Width = (double)Math.Ceiling(box.Width),
                Height = (double)Math.Ceiling(box.Height),
            };
        }

        var data = await BidiMainFrame.BrowsingContext.CaptureScreenshotAsync(parameters).ConfigureAwait(false);

        return data;
    }

    /// <inheritdoc />
    protected override Task ExposeFunctionAsync(string name, Delegate puppeteerFunction) => throw new NotImplementedException();

    private async Task<IResponse> GoAsync(int delta, NavigationOptions options)
    {
        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = BidiMainFrame.BrowsingContext.TraverseHistoryAsync(delta);

        try
        {
            await Task.WhenAll(waitForNavigationTask, navigationTask).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("no such history entry"))
            {
                return null;
            }

            throw new NavigationException(ex.Message, ex);
        }

        return waitForNavigationTask.Result;
    }

    private async Task ApplyNetworkConditionsAsync()
    {
        if (_emulatedNetworkConditions == null)
        {
            return;
        }

        await BidiMainFrame.Client.SendAsync(
            "Network.emulateNetworkConditions",
            new Cdp.Messaging.NetworkEmulateNetworkConditionsRequest
            {
                Offline = _emulatedNetworkConditions.Offline,
                Latency = _emulatedNetworkConditions.Latency,
                UploadThroughput = _emulatedNetworkConditions.Upload,
                DownloadThroughput = _emulatedNetworkConditions.Download,
            }).ConfigureAwait(false);
    }

    private void Initialize()
    {
        BidiMainFrame.BrowsingContext.Closed += (_, _) =>
        {
            OnClose();
        };
    }
}
