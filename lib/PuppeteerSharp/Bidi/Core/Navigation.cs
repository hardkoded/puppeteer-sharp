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

#if !CDP_ONLY

using System;
using WebDriverBiDi.BrowsingContext;

namespace PuppeteerSharp.Bidi.Core;

internal class Navigation
{
    private readonly BrowsingContext _browsingContext;
    private Navigation _navigation;

    private Navigation(BrowsingContext browsingContext)
    {
        _browsingContext = browsingContext;
    }

    public event EventHandler<NavigationEventArgs> Failed;

    public event EventHandler<NavigationEventArgs> Fragment;

    public event EventHandler<NavigationEventArgs> Aborted;

    public event EventHandler<RequestEventArgs> RequestCreated;

    public string Id { get; private set; }

    public Request Request { get; private set; }

    public Session Session => _browsingContext.UserContext.Browser.Session;

    public bool IsDisposed { get; private set; }

    // We have some race conditions puppeteer doesn't have.
    // We might receive a fragment before the BidiFrame sets up the listener.
    public bool FragmentReceived { get; set; }

    public static Navigation From(BrowsingContext context)
    {
        var navigation = new Navigation(context);
        navigation.Initialize();
        return navigation;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }

    protected virtual void OnFailed(NavigationEventArgs e) => Failed?.Invoke(this, e);

    private void Initialize()
    {
        _browsingContext.Closed += (sender, args) =>
        {
            OnFailed(new NavigationEventArgs(_browsingContext.Url));
        };

        _browsingContext.Request += (sender, args) =>
        {
            if (args.Request.Navigation == null || !Matches(args.Request.Navigation))
            {
                return;
            }

            Request = args.Request;
            OnRequestCreated(Request);
            Request.Redirect += (sender, args) => Request = args.Request;
        };

        Session.BrowsingContextNavigationStarted += (sender, args) =>
        {
            if (args.BrowsingContextId != _browsingContext.Id || _navigation != null)
            {
                return;
            }

            _navigation = Navigation.From(_browsingContext);
        };

        Session.BrowsingContextDomContentLoaded += OnSessionPageLoaded;
        Session.BrowsingContextLoad += OnSessionPageLoaded;

        Session.BrowsingContextFragmentNavigated += (sender, args) => HandleNavigation(args, () => OnFragment(new NavigationEventArgs(args.Url)));
        Session.BrowsingContextNavigationFailed += (sender, args) => HandleNavigation(args, () => OnFailed(new NavigationEventArgs(args.Url)));
        Session.BrowsingContextNavigationAborted += (sender, args) => HandleNavigation(args, () => OnAborted(new NavigationEventArgs(args.Url)));
    }

    private void HandleNavigation(WebDriverBiDi.BrowsingContext.NavigationEventArgs args, Action action)
    {
        // Skip if this navigation is already disposed
        if (IsDisposed)
        {
            return;
        }

        if (args.BrowsingContextId != _browsingContext.Id || !Matches(args.NavigationId))
        {
            return;
        }

        action();
        Dispose();
    }

    private void OnFragment(NavigationEventArgs args)
    {
        FragmentReceived = true;
        Fragment?.Invoke(this, args);
    }

    private void OnAborted(NavigationEventArgs args) => Aborted?.Invoke(this, args);

    private void OnSessionPageLoaded(object sender, WebDriverBiDi.BrowsingContext.NavigationEventArgs e)
    {
        if (e.BrowsingContextId != _browsingContext.Id || e.NavigationId == null || !Matches(e.NavigationId))
        {
            return;
        }

        Dispose();
    }

    private void OnRequestCreated(Request request) => RequestCreated?.Invoke(this, new RequestEventArgs(request));

    private bool Matches(string navigation)
    {
        if (_navigation is { IsDisposed: false })
        {
            return false;
        }

        if (Id == null)
        {
            Id = navigation;
            return true;
        }

        return Id == navigation;
    }
}

#endif
