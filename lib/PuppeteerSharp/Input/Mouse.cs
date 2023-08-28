using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public class Mouse : IMouse
    {
        private readonly CDPSession _client;
        private readonly Keyboard _keyboard;
        private readonly MouseState _mouseState = new();
        private readonly List<MouseTransaction.TransactionData> _transactions = new();

        /// <inheritdoc cref="Mouse"/>
        public Mouse(CDPSession client, Keyboard keyboard)
        {
            _client = client;
            _keyboard = keyboard;
        }

        /// <inheritdoc/>
        public async Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
        {
            options ??= new MoveOptions();

            var fromX = GetState().Position.X;
            var fromY = GetState().Position.Y;
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
        public async Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
        {
            options ??= new ClickOptions();

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
        }

        /// <inheritdoc/>
        public Task DownAsync(ClickOptions options = null)
        {
            options ??= new ClickOptions();

            if (GetState().Buttons.HasFlag(options.Button))
            {
                throw new PuppeteerException($"{options.Button} is already pressed");
            }

            return WithTransactionAsync((updateState) =>
            {
                updateState(new MouseTransaction.TransactionData
                {
                    Buttons = GetState().Buttons | options.Button,
                });

                var state = GetState();
                return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MousePressed,
                    Modifiers = _keyboard.Modifiers,
                    ClickCount = options.ClickCount,
                    Buttons = (int)state.Buttons,
                    Button = options.Button,
                    X = state.Position.X,
                    Y = state.Position.Y,
                });
            });
        }

        /// <inheritdoc/>
        public Task UpAsync(ClickOptions options = null)
        {
            options ??= new ClickOptions();

            if (!GetState().Buttons.HasFlag(options.Button))
            {
                throw new PuppeteerException($"{options.Button} is not pressed");
            }

            return WithTransactionAsync((updateState) =>
            {
                updateState(new MouseTransaction.TransactionData
                {
                    Buttons = GetState().Buttons & ~options.Button,
                });

                var state = GetState();
                return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseReleased,
                    Modifiers = _keyboard.Modifiers,
                    ClickCount = options.ClickCount,
                    Buttons = (int)state.Buttons,
                    Button = options.Button,
                    X = state.Position.X,
                    Y = state.Position.Y,
                });
            });
        }

        /// <inheritdoc/>
        public Task WheelAsync(decimal deltaX, decimal deltaY)
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
        public async Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
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
        }

        /// <inheritdoc/>
        public Task DragEnterAsync(decimal x, decimal y, DragData data)
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
        public Task DragOverAsync(decimal x, decimal y, DragData data)
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
        public Task DropAsync(decimal x, decimal y, DragData data)
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
        public async Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0)
        {
            var data = await DragAsync(startX, startY, endX, endY).ConfigureAwait(false);
            await DragEnterAsync(endX, endY, data).ConfigureAwait(false);
            await DragOverAsync(endX, endY, data).ConfigureAwait(false);

            if (delay > 0)
            {
                await Task.Delay(delay).ConfigureAwait(false);
            }

            await DropAsync(endX, endY, data).ConfigureAwait(false);
            await UpAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task ResetAsync()
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
        }

        private MouseTransaction CreateTrasaction()
        {
            var transaction = new MouseTransaction.TransactionData();
            _transactions.Add(transaction);

            return new MouseTransaction()
            {
                Update = updates =>
                {
                    if (updates.Position.HasValue)
                    {
                        transaction.Position = updates.Position.Value;
                    }

                    if (updates.Buttons.HasValue)
                    {
                        transaction.Buttons = updates.Buttons.Value;
                    }
                },
                Commit = () =>
                {
                    _mouseState.Position = transaction.Position ?? _mouseState.Position;
                    _mouseState.Buttons = transaction.Buttons ?? _mouseState.Buttons;
                    _transactions.Remove(transaction);
                },
                Rollback = () => _transactions.Remove(transaction),
            };
        }

        private async Task WithTransactionAsync(Func<Action<MouseTransaction.TransactionData>, Task> action)
        {
            var transaction = CreateTrasaction();
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
            var state = new MouseTransaction.TransactionData();

            foreach (var transaction in _transactions)
            {
                if (transaction.Position.HasValue)
                {
                    state.Position = transaction.Position.Value;
                }

                if (transaction.Buttons.HasValue)
                {
                    state.Buttons = transaction.Buttons.Value;
                }
            }

            if (!state.Position.HasValue)
            {
                state.Position = _mouseState.Position;
            }

            if (!state.Buttons.HasValue)
            {
                state.Buttons = _mouseState.Buttons;
            }

            return new MouseState
            {
                Position = state.Position.Value,
                Buttons = state.Buttons.Value,
            };
        }
    }
}
