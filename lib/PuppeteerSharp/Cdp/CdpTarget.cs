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
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpTarget : Target
{
    internal CdpTarget(
        TargetInfo targetInfo,
        CdpCDPSession session,
        CdpBrowserContext context,
        ITargetManager targetManager,
        Func<bool, Task<CDPSession>> sessionFactory,
        TaskQueue screenshotTaskQueue) : base(targetInfo, session, context, targetManager, sessionFactory)
    {
        ScreenshotTaskQueue = screenshotTaskQueue;
        Initialize();
    }

    /// <inheritdoc/>
    public override ITarget Opener => TargetInfo.OpenerId != null ?
        CdpBrowser.TargetManager.GetAvailableTargets().GetValueOrDefault(TargetInfo.OpenerId) : null;

    internal CdpBrowser CdpBrowser => Browser as CdpBrowser;

    internal TaskQueue ScreenshotTaskQueue { get; }

    /// <inheritdoc/>
    public override async Task<IPage> AsPageAsync()
    {
        if (Session == null)
        {
            var session = (CdpCDPSession)await CreateCDPSessionAsync().ConfigureAwait(false);
            return await CdpPage.CreateAsync(session, this, false, null, ScreenshotTaskQueue).ConfigureAwait(false);
        }

        return await CdpPage.CreateAsync((CdpCDPSession)Session, this, false, null, ScreenshotTaskQueue).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<ICDPSession> CreateCDPSessionAsync()
    {
        var session = await SessionFactory(false).ConfigureAwait(false);
        session.Target = this;
        return session;
    }

    internal void TargetInfoChanged(TargetInfo targetInfo)
    {
        TargetInfo = targetInfo;
        CheckIfInitialized();
    }

    /// <summary>
    /// Initializes the target.
    /// </summary>
    internal virtual void Initialize()
    {
        IsInitialized = true;
        InitializedTaskWrapper.TrySetResult(InitializationStatus.Success);
    }

    /// <summary>
    /// Check is the target is not initialized.
    /// </summary>
    protected internal virtual void CheckIfInitialized()
    {
        IsInitialized = true;
        InitializedTaskWrapper.TrySetResult(InitializationStatus.Success);
    }
}
