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
using WebDriverBiDi.BrowsingContext;

namespace PuppeteerSharp.Bidi.Core;

internal class BrowsingContext : IDisposable
{
    private readonly ConcurrentDictionary<string, BrowsingContext> _children = new();
    private readonly ConcurrentDictionary<string, Request> _requests = new();
    private string _reason;
    private Navigation _navigation;

    private BrowsingContext(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener)
    {
        UserContext = userContext;
        Parent = parent;
        Id = id;
        Url = url;
        OriginalOpener = originalOpener;
    }

    public event EventHandler<ClosedEventArgs> Closed;

    public event EventHandler DomContentLoaded;

    public event EventHandler Load;

    public event EventHandler<BidiBrowsingContextEventArgs> BrowsingContextCreated;

    public event EventHandler<RequestEventArgs> Request;

    public event EventHandler<BrowserContextNavigationEventArgs> Navigation;

    public UserContext UserContext { get; }

    public string Id { get; }

    public string Url { get; private set; }

    public Session Session => UserContext.Browser.Session;

    public IEnumerable<BrowsingContext> Children => _children.Values;

    internal string OriginalOpener { get; }

    internal BrowsingContext Top
    {
        get
        {
            var context = this;

            while (context.Parent != null)
            {
                context = context.Parent;
            }

            return context;
        }
    }

    private BrowsingContext Parent { get; }

    public static BrowsingContext From(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener)
    {
        var context = new BrowsingContext(userContext, parent, id, url, originalOpener);
        context.Initialize();
        return context;
    }

    public void Dispose()
    {
        _reason ??= "Browser was disconnected, probably because the session ended.";
        OnClosed(_reason);
        foreach (var context in _children.Values)
        {
            context.Dispose("Parent browsing context was disposed");
        }
    }

    public async Task CloseAsync(bool? promptUnload)
    {
        foreach (var context in _children.Values)
        {
            await context.CloseAsync(promptUnload).ConfigureAwait(false);
        }

        await Session.Driver.BrowsingContext.CloseAsync(new CloseCommandParameters(Id)
        {
            PromptUnload = promptUnload,
        }).ConfigureAwait(false);
    }

    internal async Task NavigateAsync(string url, ReadinessState wait)
    {
        await Session.Driver.BrowsingContext.NavigateAsync(new NavigateCommandParameters(Id, url)
        {
            Wait = wait,
        }).ConfigureAwait(false);
    }

    protected virtual void OnBrowsingContextCreated(BidiBrowsingContextEventArgs e) => BrowsingContextCreated?.Invoke(this, e);

    private void Initialize()
    {
        UserContext.Closed += (sender, args) => Dispose("User context was closed");

        Session.BrowsingContextContextCreated += (sender, args) =>
        {
            if (args.Parent != Id)
            {
                return;
            }

            var browsingContext = From(UserContext, this, args.UserContextId, args.Url, args.OriginalOpener);

            _children.TryAdd(args.UserContextId, browsingContext);

            browsingContext.Closed += (sender, args) =>
            {
                _children.TryRemove(browsingContext.Id, out _);
            };

            OnBrowsingContextCreated(new BidiBrowsingContextEventArgs(browsingContext));
        };

        Session.BrowsingContextContextDestroyed += (sender, args) =>
        {
            if (args.UserContextId != Id)
            {
                return;
            }

            Dispose("Browsing context already closed.");
        };

        Session.BrowsingContextDomContentLoaded += (sender, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            Url = args.Url;
            OnDomContentLoaded();
        };

        Session.BrowsingContextLoad += (sender, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            Url = args.Url;
            OnLoad();
        };

        Session.BrowsingcontextNavigationStarted += (sender, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            foreach (var entry in _requests)
            {
                if (entry.Value.IsDisposed)
                {
                    _requests.TryRemove(entry.Key, out _);
                }
            }

            if (_navigation is { IsDisposed: false })
            {
                return;
            }

            _navigation = Core.Navigation.From(this);

            _navigation.Fragment += UpdateUrlFromEvent;
            _navigation.Aborted += UpdateUrlFromEvent;
            _navigation.Failed += UpdateUrlFromEvent;

            OnNavigation(new BrowserContextNavigationEventArgs(_navigation));
        };

        Session.NetworkBeforeRequestSent += (sender, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            if (_requests.ContainsKey(args.Request.RequestId))
            {
                return;
            }

            var request = Core.Request.From(this, args);
            _requests.TryAdd(args.Request.RequestId, request);
            Request?.Invoke(this, new RequestEventArgs(request));
        };
    }

    private void OnNavigation(BrowserContextNavigationEventArgs args) => Navigation?.Invoke(this, args);

    private void UpdateUrlFromEvent(object sender, NavigationEventArgs e)
    {
        Url = e.Url;
    }

    private void OnLoad() => Load?.Invoke(this, EventArgs.Empty);

    private void OnDomContentLoaded() => DomContentLoaded?.Invoke(this, EventArgs.Empty);

    private void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }

    private void OnClosed(string reason) => Closed?.Invoke(this, new ClosedEventArgs(reason));
}
