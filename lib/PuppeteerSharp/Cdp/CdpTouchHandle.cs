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

using System;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Cdp;

internal class CdpTouchHandle : ITouchHandle
{
    private readonly CdpTouchscreen _touchScreen;
    private readonly TouchPoint _touchPoint;
    private readonly Keyboard _keyboard;
    private CDPSession _client;
    private bool _started;

    internal CdpTouchHandle(CDPSession client, CdpTouchscreen touchScreen, Keyboard keyboard, TouchPoint touchPoint)
    {
        _client = client;
        _touchScreen = touchScreen;
        _keyboard = keyboard;
        _touchPoint = touchPoint;
    }

    /// <inheritdoc />
    public Task MoveAsync(decimal x, decimal y)
    {
        _touchPoint.X = Math.Round(x, MidpointRounding.AwayFromZero);
        _touchPoint.Y = Math.Round(y, MidpointRounding.AwayFromZero);

        return _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchMove",
                TouchPoints = [_touchPoint],
                Modifiers = _keyboard.Modifiers,
            });
    }

    /// <inheritdoc />
    public async Task EndAsync()
    {
        await _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchEnd",
                TouchPoints = [_touchPoint],
                Modifiers = _keyboard.Modifiers,
            }).ConfigureAwait(false);

        _touchScreen.RemoveHandle(this);
    }

    internal async Task StartAsync()
    {
        if (_started)
        {
            throw new PuppeteerException("Touch has already started");
        }

        await _client.SendAsync(
            "Input.dispatchTouchEvent",
            new InputDispatchTouchEventRequest
            {
                Type = "touchStart",
                TouchPoints = [_touchPoint],
                Modifiers = _keyboard.Modifiers,
            }).ConfigureAwait(false);

        _started = true;
    }

    internal void UpdateClient(CDPSession newSession) => _client = newSession;
}
