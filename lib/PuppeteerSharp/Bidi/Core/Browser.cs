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
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Browser;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Protocol;
using WebDriverBiDi.Script;
using CloseCommandParameters = WebDriverBiDi.Browser.CloseCommandParameters;

namespace PuppeteerSharp.Bidi.Core;

internal class Browser(Session session) : IDisposable
{
    private readonly ConcurrentDictionary<string, UserContext> _userContexts = new();
    private bool _disposed;

    public event EventHandler Closed;

    public event EventHandler Disconnected;

    public Session Session { get; } = session;

    public bool IsClosed { get; set; }

    public string Reason { get; set; }

    internal ICollection<UserContext> UserContexts => _userContexts.Values;

    public static async Task<Browser> From(Session session)
    {
        var browser = new Browser(session);
        await browser.InitializeAsync().ConfigureAwait(false);
        return browser;
    }

    public async Task CloseAsync()
    {
        try
        {
            await Session.Driver.Browser.CloseAsync(new CloseCommandParameters()).ConfigureAwait(false);
        }
        finally
        {
            Dispose("Browser already closed.", true);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Session?.Dispose();
        }

        _disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        Dispose();
    }

    private async Task InitializeAsync()
    {
        Session.Ended += OnSessionEnded;
        Session.Driver.OnEventReceived.AddObserver(OnEventReceived);

        await SyncUserContextsAsync().ConfigureAwait(false);
        await SyncBrowsingContextsAsync().ConfigureAwait(false);
    }

    private Task SyncBrowsingContextsAsync()
    {
        // In case contexts are created or destroyed during `getTree`, we use this
        // set to detect them.
        var contextIds = new ConcurrentSet<string>();

        Session.Driver.BrowsingContext.OnContextCreated.AddObserver(info =>
        {
            contextIds.Add(info.BrowsingContextId);
        });
        var tree = Session.Driver.BrowsingContext.GetTreeAsync(new GetTreeCommandParameters());
        var contexts = tree.Result.ContextTree;

        // Simulating events so contexts are created naturally.
        foreach (var info in contexts)
        {
            if (!contextIds.Contains(info.BrowsingContextId))
            {
                Session.OnBrowsingContextContextCreated(new BrowsingContextEventArgs(info));
            }

            foreach (var child in info.Children)
            {
                contexts.Add(child);
            }
        }

        return Task.CompletedTask;
    }

    private async Task SyncUserContextsAsync()
    {
        var result = await Session.Driver.Browser.GetUserContextsAsync(new GetUserContextsCommandParameters()).ConfigureAwait(false);


/* Unmerged change from project 'PuppeteerSharp(net8.0)'
Before:
/* Unmerged change from project 'PuppeteerSharp(net8.0)'
After:
/* Unmerged change from project 'PuppeteerSharp(net8.0)'
*/
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        Before:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        After:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        */
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        Before:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        After:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        */
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        Before:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        After:
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        */
        /* Unmerged change from project 'PuppeteerSharp(net8.0)'
        Before:
                foreach (var context in result.UserContexts) {
        After:
                foreach (var context in result.UserContexts)
                {
        */
        foreach (var context in result.UserContexts)
        {
            CreateUserContext(context.UserContextId);
        }
    }

    private UserContext CreateUserContext(string id)
    {
        var userContext = UserContext.Create(this, id);
        _userContexts.TryAdd(userContext.Id, userContext);
        userContext.Closed += (sender, args) => _userContexts.TryRemove(userContext.Id, out _);

        return userContext;
    }

    private void OnEventReceived(EventReceivedEventArgs e)
    {
        switch (e.EventName)
        {
            case "script.realmCreated":
                var realInfo = e.EventData as RealmInfo;

                if (realInfo.Type != RealmType.SharedWorker)
                {
                    return;
                }

                break;
            default:
                return;
        }
    }

    private void OnSessionEnded(object sender, SessionEndArgs e)
    {
        Dispose(e.Reason);
    }

    private void Dispose(string reason = null, bool closed = false)
    {
        Reason = reason;
        IsClosed = closed;
        Dispose(true);
    }
}
