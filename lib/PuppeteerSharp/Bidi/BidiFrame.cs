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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiFrame : Frame
{
    private readonly ConcurrentDictionary<BrowsingContext, BidiFrame> _frames = new();
    private readonly Realms _realms;

    private readonly ILoggerFactory _loggerFactory;

    internal BidiFrame(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext, ILoggerFactory loggerFactory)
    {
        Client = new BidiCdpSession(this, loggerFactory);
        _loggerFactory = loggerFactory;
        ParentPage = parentPage;
        ParentFrame = parentFrame;
        BrowsingContext = browsingContext;
        Id = browsingContext.Id;
        _realms = new Realms(
#pragma warning disable CA2000
            BidiFrameRealm.From(browsingContext.DefaultRealm, this),
            BidiFrameRealm.From(browsingContext.CreateWindowRealm($"__puppeteer_internal_{new Random().Next(0, 10000)}"), this));
#pragma warning restore CA2000
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<IFrame> ChildFrames
    {
        get
        {
            return BrowsingContext.Children.Select(child =>
            {
                _frames.TryGetValue(child, out var frame);

                return frame;
            }).Where(frame => frame != null).ToList();
        }
    }

    /// <inheritdoc/>
    public override string Url => BrowsingContext.Url;

    /// <inheritdoc />
    public override IPage Page => BidiPage;

    /// <inheritdoc />
    public override ICDPSession Client { get; protected set; }

    /// <inheritdoc />
    internal override Realm MainRealm => _realms.Default;

    internal override Realm IsolatedRealm => _realms.Internal;

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
        catch (NavigationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw RewriteNavigationError(ex, url, options?.Timeout ?? TimeoutSettings.NavigationTimeout);
        }

        return waitForNavigationTask.Result;
    }

    /// <inheritdoc />
    public override async Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        async Task<Navigation> WaitForEventNavigationAsync()
        {
            // TODO: This logic is missing tons of things.
            var navigationTcs = new TaskCompletionSource<Navigation>(TaskCreationOptions.RunContinuationsAsynchronously);

            // TODO: Async void is not safe. Refactor code.
            BrowsingContext.Navigation += (sender, args) => navigationTcs.TrySetResult(args.Navigation);

            await navigationTcs.Task.ConfigureAwait(false);

            var waitForLoadTask = WaitForLoadAsync(options);

            Task<bool> waitForFragmentTask;

            if (navigationTcs.Task.Result.FragmentReceived)
            {
                waitForFragmentTask = Task.FromResult(true);
            }
            else
            {
                var waitForFragmentTcs = new TaskCompletionSource<bool>();
                navigationTcs.Task.Result.Fragment += (sender, args) => waitForFragmentTcs.TrySetResult(true);
                waitForFragmentTask = waitForFragmentTcs.Task;
            }

            // TODO: Add frame detached event.
            // TODO: Add failed and aborted events.
            await Task.WhenAny(waitForLoadTask, waitForFragmentTask).WithTimeout(timeout).ConfigureAwait(false);

            return navigationTcs.Task.Result;
        }

        var waitForEventNavigationTask = WaitForEventNavigationAsync();
        var waitForNetworkIdleTask = WaitForNetworkIdleAsync(options);

        var waitForResponse = new Func<Task<IResponse>>(async () =>
        {
            await Task.WhenAll(waitForEventNavigationTask, waitForNetworkIdleTask).ConfigureAwait(false);
            var navigation = waitForEventNavigationTask.Result;
            var request = navigation.Request;

            if (navigation.Request == null)
            {
                return null;
            }

            var lastRequest = request.LastRedirect ?? request;
            BidiHttpRequest.Requests.TryGetValue(lastRequest, out var httpRequest);

            return httpRequest.Response;
        });

        var waitForResponseTask = waitForResponse();

        // TODO: Listen to frame detached event.
        await Task.WhenAny(waitForResponseTask).WithTimeout(timeout).ConfigureAwait(false);

        return waitForResponseTask.Result;
    }

    /// <inheritdoc />
    protected override DeviceRequestPromptManager GetDeviceRequestPromptManager() => throw new System.NotImplementedException();

    internal static BidiFrame From(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext, ILoggerFactory loggerFactory)
    {
        parentFrame = new BidiFrame(parentPage, parentFrame, browsingContext, loggerFactory);
        parentFrame.Initialize();
        return parentFrame;
    }

    private PuppeteerException RewriteNavigationError(Exception ex, string url, int timeoutSettingsNavigationTimeout)
    {
        return ex is TimeoutException
            ? new NavigationException($"Navigation timeout of {timeoutSettingsNavigationTimeout} ms exceeded", url, ex)
            : new PuppeteerException("Navigation failed: " + ex.Message, ex);
    }

    private Task WaitForLoadAsync(NavigationOptions options)
    {
        var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
        var timeout = options?.Timeout ?? TimeoutSettings.NavigationTimeout;

        List<Task> tasks = new();

        if (waitUntil.Contains(WaitUntilNavigation.Load))
        {
            var loadTcs = new TaskCompletionSource<bool>();
            BrowsingContext.Load += (sender, args) => loadTcs.TrySetResult(true);
            tasks.Add(loadTcs.Task);
        }

        if (waitUntil.Contains(WaitUntilNavigation.DOMContentLoaded))
        {
            var domContentLoadedTcs = new TaskCompletionSource<bool>();
            BrowsingContext.DomContentLoaded += (sender, args) => domContentLoadedTcs.TrySetResult(true);
            tasks.Add(domContentLoadedTcs.Task);
        }

        // TODO: Check frame detached event.
        return Task.WhenAll(tasks).WithTimeout(timeout);
    }

    private Task WaitForNetworkIdleAsync(NavigationOptions options)
    {
        // TODO: Complete this method.
        return Task.CompletedTask;
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

            throw new NavigationException($"Failed to navigate to {url}. {ex.Message}", url, ex);
        }
    }

    private void Initialize()
    {
        foreach (var browsingContext in BrowsingContext.Children)
        {
            CreateFrameTarget(browsingContext, _loggerFactory);
        }

        BrowsingContext.BrowsingContextCreated += (sender, args) =>
        {
            CreateFrameTarget(args.BrowsingContext, _loggerFactory);
        };

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

        BrowsingContext.Navigation += (sender, args) =>
        {
            if (args.Navigation.FragmentReceived)
            {
                ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
            }

            args.Navigation.Fragment += (o, eventArgs) =>
            {
                ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
            };
        };

        BrowsingContext.Load += (sender, args) =>
        {
            ((Page)Page).OnLoad();
        };

        BrowsingContext.DomContentLoaded += (sender, args) =>
        {
            ((Page)Page).OnDOMContentLoaded();
            ((Page)Page).OnFrameNavigated(new FrameNavigatedEventArgs(this, NavigationType.Navigation));
        };
    }

    private void CreateFrameTarget(BrowsingContext browsingContext, ILoggerFactory loggerFactory)
    {
        var frame = From(null, this, browsingContext, loggerFactory);
        _frames.TryAdd(browsingContext, frame);
        ((BidiPage)Page).OnFrameAttached(new FrameEventArgs(frame));

        browsingContext.Closed += (sender, args) =>
        {
            _frames.TryRemove(browsingContext, out var _);
        };
    }

    private class Realms(BidiFrameRealm defaultRealm, BidiFrameRealm internalRealm)
    {
        public BidiFrameRealm Default { get; } = defaultRealm;

        public BidiFrameRealm Internal { get; } = internalRealm;
    }
}

