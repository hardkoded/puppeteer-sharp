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

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiFrame : Frame
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiFrame> _frames = new();

    internal BidiFrame(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        ParentPage = parentPage;
        ParentFrame = parentFrame;
        BrowsingContext = browsingContext;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<IFrame> ChildFrames { get; }

    /// <inheritdoc/>
    public override string Url => BrowsingContext.Url;

    /// <inheritdoc />
    public override IPage Page => BidiPage;

    /// <inheritdoc />
    public override CDPSession Client { get; protected set; }

    internal BidiPage BidiPage
    {
        get
        {
            var frame = this;
            while (frame?.ParentPage == null)
            {
                frame = frame.ParentFrame as BidiFrame;
            }

            return frame.ParentPage;
        }
    }

    internal BrowsingContext BrowsingContext { get; }

    internal BidiPage ParentPage { get; }

    internal override Frame ParentFrame { get; }

    internal TimeoutSettings TimeoutSettings => BidiPage.TimeoutSettings;

    /// <inheritdoc />
    public override Task<IElementHandle> AddStyleTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IElementHandle> AddScriptTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task SetContentAsync(string html, NavigationOptions options = null) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override async Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        var waitForNavigationTask = WaitForNavigationAsync(options);
        var navigationTask = NavigateAsync(url);

        try
        {
            await Task.WhenAll(waitForNavigationTask, navigationTask).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw RewriteNavigationError(ex, url, options?.Timeout ?? TimeoutSettings.NavigationTimeout);
        }

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
    {
        // TODO: This logic is missing tons of things.
        var navigationTcs = new TaskCompletionSource<IResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        // TODO: Async void is not safe. Refactor code.
        BrowsingContext.Navigation += (sender, args) =>
        {
            args.Navigation.RequestCreated += async (o, eventArgs) =>
            {
                try
                {
                    var httpRequest = await BidiHttpRequest.Requests.GetItemAsync(args.Navigation.Request)
                        .ConfigureAwait(false);

                    if (httpRequest.Response != null)
                    {
                        navigationTcs.TrySetResult(httpRequest.Response);
                        return;
                    }

                    args.Navigation.Request.Success += (o, eventArgs) =>
                    {
                        navigationTcs.TrySetResult(httpRequest.Response);
                    };
                }
                catch (Exception ex)
                {
                    navigationTcs.TrySetException(ex);
                }
            };
        };

        return navigationTcs.Task;
    }

    internal static BidiFrame From(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        parentFrame = new BidiFrame(parentPage, parentFrame, browsingContext);
        parentFrame.Initialize();
        return parentFrame;
    }

    /// <inheritdoc />
    protected internal override DeviceRequestPromptManager GetDeviceRequestPromptManager() => throw new System.NotImplementedException();

    private PuppeteerException RewriteNavigationError(Exception ex, string url, int timeoutSettingsNavigationTimeout)
    {
        return ex is TimeoutException
            ? new NavigationException($"Navigation timeout of {timeoutSettingsNavigationTimeout} ms exceeded", url)
            : new PuppeteerException("Navigation failed: " + ex.Message, ex);
    }

    private async Task NavigateAsync(string url)
    {
        // Some implementations currently only report errors when the
        // readiness=interactive.
        //
        // Related: https://bugzilla.mozilla.org/show_bug.cgi?id=1846601
        try
        {
            await BrowsingContext.NavigateAsync(url, WebDriverBiDi.BrowsingContext.ReadinessState.Interactive).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("net::ERR_HTTP_RESPONSE_CODE_FAILURE"))
            {
                return;
            }

            throw new PuppeteerException("Failed to navigate to " + url, ex);
        }
    }

    private void Initialize()
    {
        foreach (var browsingContext in BrowsingContext.Children)
        {
            CreateFrameTarget(browsingContext);
        }

        BrowsingContext.Closed += (sender, args) =>
        {
            foreach (var session in BidiCdpSession.Sessions)
            {
                if (session.Frame == this)
                {
                    session.OnClose();
                }
            }

            ((BidiPage)Page).OnFrameDetached(new FrameEventArgs(this));
        };

        BrowsingContext.Request += (sender, args) =>
        {
            var httpRequest = BidiHttpRequest.From(args.Request, this);

            args.Request.Success += (o, eventArgs) =>
            {
                ((BidiPage)Page).OnRequestFinished(new RequestEventArgs(httpRequest));
            };

            args.Request.Error += (o, eventArgs) =>
            {
                ((BidiPage)Page).OnRequestFailed(new RequestEventArgs(httpRequest));
            };

            _ = httpRequest.FinalizeInterceptionsAsync();
        };
    }

    private void CreateFrameTarget(BrowsingContext browsingContext)
    {
        var frame = BidiFrame.From(null, this, browsingContext);
        _frames.TryAdd(browsingContext, frame);
        ((BidiPage)Page).OnFrameAttached(new FrameEventArgs(frame));

        browsingContext.Closed += (sender, args) =>
        {
            _frames.TryRemove(browsingContext, out var _);
        };
    }
}

