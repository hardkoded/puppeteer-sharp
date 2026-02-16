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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiMouse(BidiPage page) : Mouse
{
    private readonly PointerSourceActions _mouseSource = new();
    private readonly WheelSourceActions _wheelSource = new();
    private readonly TaskQueue _actionsQueue = new();
    private readonly HashSet<MouseButton> _pressedButtons = [];
    private Point _lastMovePoint = new() { X = 0, Y = 0 };

    public override Task DropAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0) => throw new NotImplementedException();

    public override Task ResetAsync()
    {
        return _actionsQueue.Enqueue(async () =>
        {
            // BiDi uses releaseActions() which releases all buttons/keys automatically
            // The browser handles the release order (typically in reverse order of press)
            _lastMovePoint = new Point { X = 0, Y = 0 };
            _pressedButtons.Clear();
            await page.BidiMainFrame.BrowsingContext.ReleaseActionsAsync().ConfigureAwait(false);
        });
    }

    public override Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            var from = _lastMovePoint;
            var to = new Point
            {
                X = Math.Round(x),
                Y = Math.Round(y),
            };

            var steps = options?.Steps ?? 0;

            for (var i = 0; i < steps; ++i)
            {
                _mouseSource.Actions.Add(new PointerMoveAction
                {
                    X = (long)(from.X + ((to.X - from.X) * (i / (decimal)steps))),
                    Y = (long)(from.Y + ((to.Y - from.Y) * (i / (decimal)steps))),
                });
            }

            _mouseSource.Actions.Add(new PointerMoveAction
            {
                X = (long)to.X,
                Y = (long)to.Y,
            });

            _lastMovePoint = to;

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_mouseSource]).ConfigureAwait(false);
            _mouseSource.Actions.Clear();
        });
    }

    public override Task DownAsync(ClickOptions options = null)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            var button = options?.Button ?? MouseButton.Left;
            _mouseSource.Actions.Add(new PointerDownAction(GetBidiButton(button)));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_mouseSource]).ConfigureAwait(false);
            _mouseSource.Actions.Clear();
            _pressedButtons.Add(button);
        });
    }

    public override Task UpAsync(ClickOptions options = null)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            var button = options?.Button ?? MouseButton.Left;
            _mouseSource.Actions.Add(new PointerUpAction(GetBidiButton(button)));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_mouseSource]).ConfigureAwait(false);
            _mouseSource.Actions.Clear();
            _pressedButtons.Remove(button);
        });
    }

    public override Task WheelAsync(decimal deltaX, decimal deltaY)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _wheelSource.Actions.Add(new WheelScrollAction
            {
                X = (ulong)_lastMovePoint.X,
                Y = (ulong)_lastMovePoint.Y,
                DeltaX = (long)deltaX,
                DeltaY = (long)deltaY,
            });

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_wheelSource]).ConfigureAwait(false);
            _wheelSource.Actions.Clear();
        });
    }

    public override Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY) => throw new NotImplementedException();

    public override Task DragEnterAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task DragOverAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _mouseSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });

            var pointerDownAction = new PointerDownAction(GetBidiButton(options?.Button ?? MouseButton.Left));
            var pointerUpAction = new PointerUpAction(GetBidiButton(options?.Button ?? MouseButton.Left));

            for (var i = 1; i < (options?.Count ?? 1); ++i)
            {
                _mouseSource.Actions.Add(pointerDownAction);
                _mouseSource.Actions.Add(pointerUpAction);
            }

            _mouseSource.Actions.Add(pointerDownAction);

            if (options?.Delay is > 0)
            {
                _mouseSource.Actions.Add(new PauseAction
                {
                    Duration = TimeSpan.FromMilliseconds(options.Delay),
                });
            }

            _mouseSource.Actions.Add(pointerUpAction);

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_mouseSource]).ConfigureAwait(false);
            _mouseSource.Actions.Clear();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _actionsQueue.Dispose();
        }
    }

    private long GetBidiButton(MouseButton optionsButton)
    {
        return optionsButton switch
        {
            MouseButton.Left => 0,
            MouseButton.Middle => 1,
            MouseButton.Right => 2,
            MouseButton.Back => 3,
            MouseButton.Forward => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(optionsButton), $"Unsupported mouse button: {optionsButton}"),
        };
    }

    private struct Point
    {
        public decimal X { get; init; }

        public decimal Y { get; init; }
    }
}

#endif
