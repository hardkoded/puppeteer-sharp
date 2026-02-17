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
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiTouchscreen(BidiPage page) : Touchscreen, IDisposable
{
    private readonly PointerSourceActions _touchSource = new()
    {
        Parameters = new PointerParameters
        {
            PointerType = WebDriverBiDi.Input.PointerType.Touch,
        },
    };

    private readonly TaskQueue _actionsQueue = new();

    public override Task TouchStartAsync(decimal x, decimal y)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });
            _touchSource.Actions.Add(new PointerDownAction(0));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    public override Task TouchMoveAsync(decimal x, decimal y)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    public override Task TouchEndAsync()
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerUpAction(0));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    public void Dispose()
    {
        _actionsQueue.Dispose();
        GC.SuppressFinalize(this);
    }
}

#endif
