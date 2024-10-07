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
using WebDriverBiDi.BrowsingContext;

namespace PuppeteerSharp.Bidi.Core;

internal class Navigation : IDisposable
{
    private readonly BrowsingContext _browsingContext;
    private Navigation _navigation;
    private string _id;
    private Request _request;

    private Navigation(BrowsingContext browsingContext)
    {
        _browsingContext = browsingContext;
    }

    public event EventHandler<NavigationEventArgs> Failed;

    public event EventHandler<NavigationEventArgs> Fragment;

    public event EventHandler<NavigationEventArgs> Aborted;

    public event EventHandler<RequestEventArgs> Request;

    public Session Session => _browsingContext.UserContext.Browser.Session;

    public bool IsDisposed { get; private set; }

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

            _request = args.Request;
            OnRequest(_request);
            _request.Redirect += (sender, args) => _request = args.Request;
        };

        Session.BrowsingcontextNavigationStarted += (sender, args) =>
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
        if (args.BrowsingContextId != _browsingContext.Id || !Matches(args.NavigationId))
        {
            return;
        }

        action();
        Dispose();
    }

    private void OnFragment(NavigationEventArgs args) => Fragment?.Invoke(this, args);

    private void OnAborted(NavigationEventArgs args) => Aborted?.Invoke(this, args);

    private void OnSessionPageLoaded(object sender, WebDriverBiDi.BrowsingContext.NavigationEventArgs e)
    {
        if (e.BrowsingContextId != _browsingContext.Id || e.NavigationId == null || !Matches(e.NavigationId))
        {
            return;
        }

        Dispose();
    }

    private void OnRequest(Request request) => Request?.Invoke(this, new RequestEventArgs(request));

    private bool Matches(string navigation)
    {
        if (_navigation is { IsDisposed: false })
        {
            return false;
        }

        if (_id == null)
        {
            _id = navigation;
            return true;
        }

        return _id == navigation;
    }

    protected virtual void OnFailed(NavigationEventArgs e) => Failed?.Invoke(this, e);
}
