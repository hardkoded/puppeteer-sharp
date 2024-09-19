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
using WebDriverBiDi.BrowsingContext;

namespace PuppeteerSharp.Bidi.Core;

internal class UserContext : IDisposable
{
    private readonly Browser _browser;
    private readonly string _id;
    private readonly ConcurrentDictionary<string, BrowsingContext> _browsingContexts = new();
    private string _reason;

    private UserContext(Browser browser, string id)
    {
        _browser = browser;
        _id = id;
    }

    public event EventHandler Closed;

    public event EventHandler Disconnected;

    public static IEnumerable<BrowsingContext> BrowsingContexts { get; set; }

    public static UserContext Create(Browser browser, string id)
    {
        var context = new UserContext(browser, id);
        context.Initialize();
        return context;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void Initialize()
    {
        _browser.Closed += (sender, args) => Dispose("User context was closed");
        _browser.Disconnected += (sender, args) => Dispose("User context was disconnected");

        _browser.Session.Driver.BrowsingContext.OnContextCreated.AddObserver(OnContextCreated);
    }

    private void OnContextCreated(BrowsingContextEventArgs info)
    {
        if (info.Parent == null)
        {
            return;
        }

        if (info.UserContextId != _id)
        {
            return;
        }

        var browsingContext = BrowsingContext.From(
            this,
            null,
            info.BrowsingContextId,
            info.Url,
            info.OriginalOpener);

        _browsingContexts.set(browsingContext.id, browsingContext);

        const browsingContextEmitter = this.#disposables.use(
            new EventEmitter(browsingContext)
        );
        browsingContextEmitter.on('closed', () => {
            browsingContextEmitter.removeAllListeners();

            this.#browsingContexts.delete(browsingContext.id);
        });

        this.emit('browsingcontext', {browsingContext});
    }

    private void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }
}
