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
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpMouse : Mouse
{
    private readonly Keyboard _keyboard;
    private readonly MouseState _mouseState = new();
    private readonly TaskQueue _actionsQueue = new();
    private readonly TaskQueue _multipleActionsQueue = new();
    private MouseTransaction.TransactionData _inFlightTransaction = null;
    private CDPSession _client;

    /// <inheritdoc cref="Mouse"/>
    internal CdpMouse(CDPSession client, Keyboard keyboard)
    {
        _client = client;
        _keyboard = keyboard;
    }

    /// <inheritdoc/>
    public override async Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
    {
        options ??= new MoveOptions();

        var position = GetState().Position;
        var fromX = position.X;
        var fromY = position.Y;
        var steps = options.Steps;

        for (var i = 1; i <= steps; i++)
        {
            await WithTransactionAsync(async (updateState) =>
            {
                updateState(new MouseTransaction.TransactionData
                {
                    Position = new Point
                    {
                        X = fromX + ((x - fromX) * ((decimal)i / steps)),
                        Y = fromY + ((y - fromY) * ((decimal)i / steps)),
                    },
                });

                var state = GetState();

                await _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseMoved,
                    Modifiers = _keyboard.Modifiers,
                    Buttons = (int)state.Buttons,
                    Button = GetButtonFromPressedButtons(state.Buttons),
                    X = state.Position.X,
                    Y = state.Position.Y,
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
    {
        options ??= new ClickOptions();

        return _multipleActionsQueue.Enqueue(async () =>
        {
            if (options.Delay > 0)
            {
                await Task.WhenAll(
                    MoveAsync(x, y),
                    DownAsync(options)).ConfigureAwait(false);

                await Task.Delay(options.Delay).ConfigureAwait(false);
                await UpAsync(options).ConfigureAwait(false);
            }
            else
            {
                await Task.WhenAll(
                MoveAsync(x, y),
                DownAsync(options),
                UpAsync(options)).ConfigureAwait(false);
            }
        });
    }

    /// <inheritdoc/>
    public override Task DownAsync(ClickOptions options = null)
    {
        return WithTransactionAsync((updateState) =>
        {
            options ??= new ClickOptions();

            if (GetState().Buttons.HasFlag(options.Button))
            {
                throw new PuppeteerException($"{options.Button} is already pressed");
            }

            updateState(new MouseTransaction.TransactionData
            {
                Buttons = GetState().Buttons | options.Button,
            });

            var state = GetState();
            return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MousePressed,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.Count,
                Buttons = (int)state.Buttons,
                Button = options.Button,
                X = state.Position.X,
                Y = state.Position.Y,
            });
        });
    }

    /// <inheritdoc/>
    public override Task UpAsync(ClickOptions options = null)
    {
        return WithTransactionAsync((updateState) =>
        {
            options ??= new ClickOptions();

            if (!GetState().Buttons.HasFlag(options.Button))
            {
                throw new PuppeteerException($"{options.Button} is not pressed");
            }

            updateState(new MouseTransaction.TransactionData
            {
                Buttons = GetState().Buttons & ~options.Button,
            });

            var state = GetState();
            return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MouseReleased,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.Count,
                Buttons = (int)state.Buttons,
                Button = options.Button,
                X = state.Position.X,
                Y = state.Position.Y,
            });
        });
    }

    /// <inheritdoc/>
    public override Task WheelAsync(decimal deltaX, decimal deltaY)
    {
        var state = GetState();

        return _client.SendAsync(
            "Input.dispatchMouseEvent",
            new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MouseWheel,
                DeltaX = deltaX,
                DeltaY = deltaY,
                X = state.Position.X,
                Y = state.Position.Y,
                Modifiers = _keyboard.Modifiers,
                PointerType = PointerType.Mouse,
            });
    }

    /// <inheritdoc/>
    public override Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
    {
        return _multipleActionsQueue.Enqueue(async () =>
        {
            var result = new TaskCompletionSource<DragData>();

            void DragIntercepted(object sender, MessageEventArgs e)
            {
                if (e.MessageID == "Input.dragIntercepted")
                {
                    result.TrySetResult(e.MessageData.SelectToken("data").ToObject<DragData>());
                    _client.MessageReceived -= DragIntercepted;
                }
            }

            _client.MessageReceived += DragIntercepted;
            await MoveAsync(startX, startY).ConfigureAwait(false);
            await DownAsync().ConfigureAwait(false);
            await MoveAsync(endX, endY).ConfigureAwait(false);

            return await result.Task.ConfigureAwait(false);
        });
    }

    /// <inheritdoc/>
    public override Task DragEnterAsync(decimal x, decimal y, DragData data)
        => _client.SendAsync(
            "Input.dispatchDragEvent",
            new InputDispatchDragEventRequest
            {
                Type = DragEventType.DragEnter,
                X = x,
                Y = y,
                Modifiers = _keyboard.Modifiers,
                Data = data,
            });

    /// <inheritdoc/>
    public override Task DragOverAsync(decimal x, decimal y, DragData data)
        => _client.SendAsync(
            "Input.dispatchDragEvent",
            new InputDispatchDragEventRequest
            {
                Type = DragEventType.DragOver,
                X = x,
                Y = y,
                Modifiers = _keyboard.Modifiers,
                Data = data,
            });

    /// <inheritdoc/>
    public override Task DropAsync(decimal x, decimal y, DragData data)
        => _client.SendAsync(
            "Input.dispatchDragEvent",
            new InputDispatchDragEventRequest
            {
                Type = DragEventType.Drop,
                X = x,
                Y = y,
                Modifiers = _keyboard.Modifiers,
                Data = data,
            });

    /// <inheritdoc/>
    public override async Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0)
    {
        // DragAsync is already using _multipleActionsQueue
        var data = await DragAsync(startX, startY, endX, endY).ConfigureAwait(false);
        await _multipleActionsQueue.Enqueue(async () =>
        {
            await DragEnterAsync(endX, endY, data).ConfigureAwait(false);
            await DragOverAsync(endX, endY, data).ConfigureAwait(false);

            if (delay > 0)
            {
                await Task.Delay(delay).ConfigureAwait(false);
            }

            await DropAsync(endX, endY, data).ConfigureAwait(false);
            await UpAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task ResetAsync()
    {
        return _multipleActionsQueue.Enqueue(() =>
        {
            var actions = new List<Task>();
            var state = GetState();

            foreach (var button in new[]
            {
                    MouseButton.Left,
                    MouseButton.Middle,
                    MouseButton.Right,
                    MouseButton.Back,
                    MouseButton.Forward,
            })
            {
                if (state.Buttons.HasFlag(button))
                {
                    actions.Add(UpAsync(new()
                    {
                        Button = button,
                    }));
                }
            }

            if (state.Position.X != 0 || state.Position.Y != 0)
            {
                actions.Add(MoveAsync(0, 0));
            }

            return Task.WhenAll(actions);
        });
    }

    internal void UpdateClient(CDPSession newSession) => _client = newSession;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _actionsQueue.Dispose();
            _multipleActionsQueue.Dispose();
        }
    }

    private MouseTransaction CreateTransaction()
    {
        _inFlightTransaction = new MouseTransaction.TransactionData();

        return new MouseTransaction()
        {
            Update = updates =>
            {
                if (updates.Position.HasValue)
                {
                    _inFlightTransaction.Position = updates.Position.Value;
                }

                if (updates.Buttons.HasValue)
                {
                    _inFlightTransaction.Buttons = updates.Buttons.Value;
                }
            },
            Commit = () =>
            {
                _mouseState.Position = _inFlightTransaction.Position ?? _mouseState.Position;
                _mouseState.Buttons = _inFlightTransaction.Buttons ?? _mouseState.Buttons;
                _inFlightTransaction = null;
            },
            Rollback = () => _inFlightTransaction = null,
        };
    }

    private Task WithTransactionAsync(Func<Action<MouseTransaction.TransactionData>, Task> action)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            var transaction = CreateTransaction();
            try
            {
                await action(transaction.Update).ConfigureAwait(false);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new PuppeteerException("Failed to perform mouse action", ex);
            }
        });
    }

    private MouseButton GetButtonFromPressedButtons(MouseButton buttons)
    {
        if (buttons.HasFlag(MouseButton.Left))
        {
            return MouseButton.Left;
        }

        if (buttons.HasFlag(MouseButton.Right))
        {
            return MouseButton.Right;
        }

        if (buttons.HasFlag(MouseButton.Middle))
        {
            return MouseButton.Middle;
        }

        if (buttons.HasFlag(MouseButton.Back))
        {
            return MouseButton.Back;
        }

        if (buttons.HasFlag(MouseButton.Forward))
        {
            return MouseButton.Forward;
        }

        return MouseButton.None;
    }

    private MouseState GetState()
    {
        var state = new MouseTransaction.TransactionData()
        {
            Position = _mouseState.Position,
            Buttons = _mouseState.Buttons,
        };

        if (_inFlightTransaction != null)
        {
            if (_inFlightTransaction.Position.HasValue)
            {
                state.Position = _inFlightTransaction.Position.Value;
            }

            if (_inFlightTransaction.Buttons.HasValue)
            {
                state.Buttons = _inFlightTransaction.Buttons.Value;
            }
        }

        return new MouseState
        {
            Position = state.Position.Value,
            Buttons = state.Buttons.Value,
        };
    }
}
