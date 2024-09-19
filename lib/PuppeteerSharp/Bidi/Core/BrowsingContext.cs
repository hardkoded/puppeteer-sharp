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

namespace PuppeteerSharp.Bidi.Core;

internal class BrowsingContext : IDisposable
{
    private readonly UserContext _userContext;
    private readonly BrowsingContext _parent;
    private readonly string _id;
    private readonly string _url;
    private readonly string _originalOpener;
    private string _reason;

    private BrowsingContext(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener)
    {
        _userContext = userContext;
        _parent = parent;
        _id = id;
        _url = url;
        _originalOpener = originalOpener;
    }

    public static BrowsingContext From(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener)
    {
        var context = new BrowsingContext(userContext, parent, id, url, originalOpener);
        context.Initialize();
        return context;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void Initialize()
    {
        _userContext.Closed += (sender, args) => Dispose("User context was closed");
        _userContext.Disconnected += (sender, args) => Dispose("User context was disconnected");
    }

    private void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }
}
