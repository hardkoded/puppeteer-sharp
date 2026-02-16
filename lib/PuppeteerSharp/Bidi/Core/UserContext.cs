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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDriverBiDi.Browser;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Permissions;

namespace PuppeteerSharp.Bidi.Core;

internal class UserContext : IDisposable
{
    public const string DEFAULT = "default";

    private readonly ConcurrentDictionary<string, BrowsingContext> _browsingContexts = new();
    private string _reason;

    private UserContext(Browser browser, string id)
    {
        Browser = browser;
        Id = id;
    }

    public event EventHandler<ClosedEventArgs> Closed;

    public event EventHandler<BidiBrowsingContextEventArgs> BrowsingContextCreated;

    public IEnumerable<BrowsingContext> BrowsingContexts => _browsingContexts.Values;

    public Browser Browser { get; }

    public Session Session => Browser.Session;

    public bool IsDisposed { get; set; }

    public string Id { get; }

    public static UserContext Create(Browser browser, string id)
    {
        var context = new UserContext(browser, id);
        context.Initialize();
        return context;
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        _reason ??= "User context already closed, probably because the browser disconnected/closed.";
        OnClosed();
        GC.SuppressFinalize(this);
    }

    public async Task<BrowsingContext> CreateBrowserContextAsync(CreateType contextType, bool? background = null)
    {
        var createParams = new CreateCommandParameters(contextType)
        {
            UserContextId = Id,
            IsCreatedInBackground = background,
        };
        var result = await Session.Driver.BrowsingContext.CreateAsync(createParams).ConfigureAwait(false);

        if (_browsingContexts.TryGetValue(result.BrowsingContextId, out var browsingContext))
        {
            return browsingContext;
        }

        // Some browsers (e.g. Firefox) may not deliver the browsingContext.contextCreated
        // event before the browsingContext.create command response, or may not deliver it
        // at all. Create the context directly from the command result as a fallback.
        browsingContext = BrowsingContext.From(
            this,
            null,
            result.BrowsingContextId,
            "about:blank",
            null);

        if (_browsingContexts.TryAdd(browsingContext.Id, browsingContext))
        {
            browsingContext.Closed += (sender, args) => _browsingContexts.TryRemove(browsingContext.Id, out _);
            OnBrowsingContext(browsingContext);
        }
        else
        {
            // The event handler added it between our check and now; use that one.
            _browsingContexts.TryGetValue(result.BrowsingContextId, out browsingContext);
        }

        return browsingContext;
    }

    public async Task RemoveAsync()
    {
        try
        {
            await Session.Driver.Browser.RemoveUserContextAsync(new RemoveUserContextCommandParameters(Id)).ConfigureAwait(false);
        }
        finally
        {
            Dispose("User context already closed.");
        }
    }

    public async Task SetPermissionsAsync(
        string origin,
        string permissionName,
        PermissionState state)
    {
        await Session.Driver.Permissions.SetPermissionAsync(
            new SetPermissionCommandParameters(permissionName, state, origin)
            {
                UserContextId = Id,
            }).ConfigureAwait(false);
    }

    private void Initialize()
    {
        Browser.Closed += (sender, args) => Dispose("User context was closed");
        Browser.Disconnected += (sender, args) => Dispose("User context was disconnected");

        Browser.Session.BrowsingContextContextCreated += OnBrowsingContextContextCreated;
    }

    private void OnBrowsingContextContextCreated(object sender, BrowsingContextEventArgs info)
    {
        if (info.Parent != null)
        {
            return;
        }

        if (info.UserContextId != Id)
        {
            return;
        }

        var browsingContext = BrowsingContext.From(
            this,
            null,
            info.BrowsingContextId,
            info.Url,
            info.OriginalOpener,
            info.ClientWindowId);

        _browsingContexts.TryAdd(browsingContext.Id, browsingContext);

        browsingContext.Closed += (sender, args) => _browsingContexts.TryRemove(browsingContext.Id, out _);

        OnBrowsingContext(browsingContext);
    }

    private void OnBrowsingContext(BrowsingContext browsingContext) => BrowsingContextCreated?.Invoke(this, new BidiBrowsingContextEventArgs(browsingContext));

    private void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }

    private void OnClosed() => Closed?.Invoke(this, new ClosedEventArgs(_reason));
}

#endif
