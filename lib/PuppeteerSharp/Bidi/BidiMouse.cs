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
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiMouse(BidiPage page) : Mouse
{
    public override Task DropAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0) => throw new NotImplementedException();

    public override Task ResetAsync() => throw new NotImplementedException();

    public override Task MoveAsync(decimal x, decimal y, MoveOptions options = null) => throw new NotImplementedException();

    public override Task UpAsync(ClickOptions options = null) => throw new NotImplementedException();

    public override Task WheelAsync(decimal deltaX, decimal deltaY) => throw new NotImplementedException();

    public override Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY) => throw new NotImplementedException();

    public override Task DragEnterAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task DragOverAsync(decimal x, decimal y, DragData data) => throw new NotImplementedException();

    public override Task DownAsync(ClickOptions options = null) => throw new NotImplementedException();

    public override async Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
    {
        var actions = new List<IPointerSourceAction>
        {
            new PointerMoveAction()
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            },
        };

        var pointerDownAction = new PointerDownAction(GetBidiButton(options?.Button ?? MouseButton.Left));
        var pointerUpAction = new PointerUpAction(GetBidiButton(options?.Button ?? MouseButton.Left));

        for (var i = 1; i < (options?.Count ?? 1); ++i)
        {
            actions.Add(pointerDownAction);
            actions.Add(pointerUpAction);
        }

        actions.Add(pointerDownAction);

        if (options?.Delay is > 0)
        {
            actions.Add(new PauseAction()
            {
                Duration = TimeSpan.FromMilliseconds(options.Delay),
            });
        }

        actions.Add(pointerUpAction);

        var finalSource = new PointerSourceActions();
        finalSource.Actions.AddRange(actions);

        await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([finalSource]).ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing) => throw new NotImplementedException();

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
}

