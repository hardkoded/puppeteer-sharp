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
        private readonly List<MouseState> _transactions = new();

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

            var fromX = _x;
            var fromY = _y;
            _x = x;
            _y = y;
            var steps = options.Steps;

            for (var i = 1; i <= steps; i++)
            {
                await _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseMoved,
                    Button = _button,
                    X = fromX + ((_x - fromX) * ((decimal)i / steps)),
                    Y = fromY + ((_y - fromY) * ((decimal)i / steps)),
                    Modifiers = _keyboard.Modifiers,
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

            _button = options.Button;

            return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MousePressed,
                Button = _button,
                X = _x,
                Y = _y,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.ClickCount,
            });
        }

        /// <inheritdoc/>
        public Task UpAsync(ClickOptions options = null)
        {
            options ??= new ClickOptions();

            _button = MouseButton.None;

            return _client.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MouseReleased,
                Button = options.Button,
                X = _x,
                Y = _y,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.ClickCount,
            });
        }

        /// <inheritdoc/>
        public Task WheelAsync(decimal deltaX, decimal deltaY)
            => _client.SendAsync(
                "Input.dispatchMouseEvent",
                new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseWheel,
                    DeltaX = deltaX,
                    DeltaY = deltaY,
                    X = _x,
                    Y = _y,
                    Modifiers = _keyboard.Modifiers,
                    PointerType = PointerType.Mouse,
                });

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

        private MouseTransaction CreateTrasaction()
        {
            var transaction = new MouseState();
            _transactions.Add(transaction);

            return new MouseTransaction()
            {
                Update = updates =>
                {
                    if (updates.Position.HasValue)
                    {
                        transaction.Position = updates.Position.Value;
                    }

                    if (updates.Button.HasValue)
                    {
                        transaction.Button = updates.Button.Value;
                    }
                },
                Commit = () =>
                {
                    _mouseState.Position = transaction.Position ?? _mouseState.Position;
                    _mouseState.Button = transaction.Button ?? _mouseState.Button;
                    _transactions.Remove(transaction);
                },
                Rollback = () => _transactions.Remove(transaction),
            };
        }

        private async Task WithTransactionAsync(Func<Action<MouseState>, Task> action)
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

        private Task ResetAsync()
        {
            var actions = new List<Func<Task>>();

            foreach (var flagAndButton of [
            [MouseButtonFlag.Left, MouseButton.Left],
            [MouseButtonFlag.Middle, MouseButton.Middle],
            [MouseButtonFlag.Right, MouseButton.Right],
            [MouseButtonFlag.Forward, MouseButton.Forward],
            [MouseButtonFlag.Back, MouseButton.Back],
            ] as const) {
            if (this.#state.buttons & flag) {
                actions.push(this.up({button: button}));
            }
            }
            if (this.#state.position.x !== 0 || this.#state.position.y !== 0) {
            actions.push(this.move(0, 0));
            }
            await Promise.all(actions);
        }
    }
}
