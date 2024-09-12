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

using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;

namespace PuppeteerSharp.Bidi;

/// <inheritdoc />
public class BidiFrame : Frame
{
    internal BidiFrame(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        Page = parentPage;
        ParentFrame = parentFrame;
        BrowsingContext = browsingContext;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<IFrame> ChildFrames { get; }

    /// <inheritdoc />
    public override IPage Page { get; }

    /// <inheritdoc />
    public override CDPSession Client { get; protected set; }

    internal BrowsingContext BrowsingContext { get; }

    internal override Frame ParentFrame { get; }

    /// <inheritdoc />
    public override Task<IElementHandle> AddStyleTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IElementHandle> AddScriptTagAsync(AddTagOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task SetContentAsync(string html, NavigationOptions options = null) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> GoToAsync(string url, NavigationOptions options) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public override Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null) => throw new System.NotImplementedException();

    internal static BidiFrame From(BidiPage parentPage, BidiFrame parentFrame, BrowsingContext browsingContext)
    {
        parentFrame = new BidiFrame(parentPage, parentFrame, browsingContext);
        parentFrame.Initialize();
        return parentFrame;
    }

    /// <inheritdoc />
    protected internal override DeviceRequestPromptManager GetDeviceRequestPromptManager() => throw new System.NotImplementedException();

    private void Initialize()
    {
        throw new System.NotImplementedException();
    }
}
