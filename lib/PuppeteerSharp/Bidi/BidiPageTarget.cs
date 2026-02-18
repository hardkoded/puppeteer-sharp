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
using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

internal class BidiPageTarget(BidiPage page) : Target
{
    public override string Url => page.Url;

    public override TargetType Type => TargetType.Page;

    public override ITarget Opener => throw new InvalidOperationException();

    internal override Browser Browser => (Browser)page.BrowserContext.Browser;

    internal override BrowserContext BrowserContext => (BrowserContext)page.BrowserContext;

    public override Task<IPage> PageAsync() => Task.FromResult<IPage>(page);

    public override Task<IPage> AsPageAsync()
#pragma warning disable CA2000
        => Task.FromResult(BidiPage.From((BidiBrowserContext)page.BrowserContext, page.BidiMainFrame.BrowsingContext) as IPage);
#pragma warning restore CA2000

    public override Task<ICDPSession> CreateCDPSessionAsync()
        => page.CreateCDPSessionAsync();
}

#endif
