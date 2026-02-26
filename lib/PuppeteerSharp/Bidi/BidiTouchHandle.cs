// * MIT License
//  *
//  * Copyright (c) Dario Kondratiuk
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
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiTouchHandle : ITouchHandle
{
    private readonly BidiTouchscreen _touchScreen;
    private readonly BidiPage _page;
    private readonly PointerSourceActions _touchSource;
    private readonly TaskQueue _actionsQueue;

    internal BidiTouchHandle(BidiTouchscreen touchScreen, BidiPage page, PointerSourceActions touchSource, TaskQueue actionsQueue)
    {
        _touchScreen = touchScreen;
        _page = page;
        _touchSource = touchSource;
        _actionsQueue = actionsQueue;
    }

    /// <inheritdoc />
    public Task MoveAsync(decimal x, decimal y)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });

            await _page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    /// <inheritdoc />
    public Task EndAsync()
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerUpAction(0));

            await _page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
            _touchScreen.RemoveHandle(this);
        });
    }
}

#endif
