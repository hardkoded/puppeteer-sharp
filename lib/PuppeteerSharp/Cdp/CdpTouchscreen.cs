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
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpTouchscreen : Touchscreen
{
    private readonly Keyboard _keyboard;
    private CDPSession _client;

    /// <inheritdoc cref="Touchscreen"/>
    internal CdpTouchscreen(CDPSession client, Keyboard keyboard)
    {
        _client = client;
        _keyboard = keyboard;
    }

    /// <inheritdoc />
    public override Task TouchStartAsync(decimal x, decimal y)
    {
        var touchPoints = new[]
        {
                new TouchPoint
                {
                    X = Math.Round(x, MidpointRounding.AwayFromZero),
                    Y = Math.Round(y, MidpointRounding.AwayFromZero),
                    RadiusX = 0.5m,
                    RadiusY = 0.5m,
                    Force = 0.5m,
                },
        };

        return _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = touchPoints,
                Modifiers = _keyboard.Modifiers,
            });
    }

    /// <inheritdoc />
    public override Task TouchMoveAsync(decimal x, decimal y)
    {
        var touchPoints = new[]
        {
                new TouchPoint
                {
                    X = Math.Round(x, MidpointRounding.AwayFromZero),
                    Y = Math.Round(y, MidpointRounding.AwayFromZero),
                    RadiusX = 0.5m,
                    RadiusY = 0.5m,
                    Force = 0.5m,
                },
        };

        return _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = touchPoints,
                Modifiers = _keyboard.Modifiers,
            });
    }

    /// <inheritdoc />
    public override Task TouchEndAsync()
    {
        return _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchEnd",
                TouchPoints = [],
                Modifiers = _keyboard.Modifiers,
            });
    }

    internal void UpdateClient(CDPSession newSession) => _client = newSession;
}
