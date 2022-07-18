using System.Threading.Tasks;

using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom.Input
{
    /// <summary>
    /// Provides methods to interact with the mouse
    /// </summary>
    public class Mouse
    {
        private readonly DevToolsConnection _connection;
        private readonly Keyboard _keyboard;

        private decimal _x = 0;
        private decimal _y = 0;
        private MouseButton _button = MouseButton.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mouse"/> class.
        /// </summary>
        /// <param name="connection">The connection</param>
        /// <param name="keyboard">The keyboard</param>
        public Mouse(DevToolsConnection connection, Keyboard keyboard)
        {
            _connection = connection;
            _keyboard = keyboard;
        }

        /// <summary>
        /// Dispatches a <c>mousemove</c> event.
        /// </summary>
        /// <param name="x">The destination mouse X coordinate.</param>
        /// <param name="y">The destination mouse Y coordinate.</param>
        /// <param name="options">Options to apply to the move operation.</param>
        /// <returns>Task</returns>
        public async Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
        {
            options = options ?? new MoveOptions();

            var fromX = _x;
            var fromY = _y;
            _x = x;
            _y = y;
            var steps = options.Steps;

            for (var i = 1; i <= steps; i++)
            {
                await _connection.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseMoved,
                    Button = _button,
                    X = fromX + ((_x - fromX) * ((decimal)i / steps)),
                    Y = fromY + ((_y - fromY) * ((decimal)i / steps)),
                    Modifiers = _keyboard.Modifiers
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Shortcut for <see cref="MoveAsync(decimal, decimal, MoveOptions)"/>, <see cref="DownAsync(ClickOptions)"/> and <see cref="UpAsync(ClickOptions)"/>
        /// </summary>
        /// <param name="x">The target mouse X location to click.</param>
        /// <param name="y">The target mouse Y location to click.</param>
        /// <param name="options">Options to apply to the click operation.</param>
        /// <returns>Task</returns>
        public async Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

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

        /// <summary>
        /// Dispatches a <c>mousedown</c> event.
        /// </summary>
        /// <param name="options">Options to apply to the mouse down operation.</param>
        /// <returns>Task</returns>
        public Task DownAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = options.Button;

            return _connection.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MousePressed,
                Button = _button,
                X = _x,
                Y = _y,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.ClickCount
            });
        }

        /// <summary>
        /// Dispatches a <c>mouseup</c> event.
        /// </summary>
        /// <param name="options">Options to apply to the mouse up operation.</param>
        /// <returns>Task</returns>
        public Task UpAsync(ClickOptions options = null)
        {
            options = options ?? new ClickOptions();

            _button = MouseButton.None;

            return _connection.SendAsync("Input.dispatchMouseEvent", new InputDispatchMouseEventRequest
            {
                Type = MouseEventType.MouseReleased,
                Button = options.Button,
                X = _x,
                Y = _y,
                Modifiers = _keyboard.Modifiers,
                ClickCount = options.ClickCount
            });
        }

        /// <summary>
        /// Dispatches a <c>wheel</c> event.
        /// </summary>
        /// <param name="deltaX">Delta X.</param>
        /// <param name="deltaY">Delta Y.</param>
        /// <returns>Task</returns>
        public Task WheelAsync(decimal deltaX, decimal deltaY)
            => _connection.SendAsync(
                "Input.dispatchMouseEvent",
                new InputDispatchMouseEventRequest
                {
                    Type = MouseEventType.MouseWheel,
                    DeltaX = deltaX,
                    DeltaY = deltaY,
                    X = _x,
                    Y = _y,
                    Modifiers = _keyboard.Modifiers,
                    PointerType = PointerType.Mouse
                });

        /// <summary>
        /// Dispatches a `drag` event.
        /// </summary>
        /// <param name="startX">Start X coordinate</param>
        /// <param name="startY">Start Y coordinate</param>
        /// <param name="endX">End X coordinate</param>
        /// <param name="endY">End Y coordinate</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser with the drag data</returns>
        public async Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
        {
            var result = new TaskCompletionSource<DragData>();

            void DragIntercepted(object sender, MessageEventArgs e)
            {
                if (e.MessageID == "Input.dragIntercepted")
                {
                    result.TrySetResult(e.MessageData.SelectToken("data").ToObject<DragData>());
                    _connection.MessageReceived -= DragIntercepted;
                }
            }
            _connection.MessageReceived += DragIntercepted;
            await MoveAsync(startX, startY).ConfigureAwait(false);
            await DownAsync().ConfigureAwait(false);
            await MoveAsync(endX, endY).ConfigureAwait(false);

            return await result.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Dispatches a `dragenter` event.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public Task DragEnterAsync(decimal x, decimal y, DragData data)
            => _connection.SendAsync(
                "Input.dispatchDragEvent",
                new InputDispatchDragEventRequest
                {
                    Type = DragEventType.DragEnter,
                    X = x,
                    Y = y,
                    Modifiers = _keyboard.Modifiers,
                    Data = data,
                });

        /// <summary>
        /// Dispatches a `dragover` event.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public Task DragOverAsync(decimal x, decimal y, DragData data)
            => _connection.SendAsync(
                "Input.dispatchDragEvent",
                new InputDispatchDragEventRequest
                {
                    Type = DragEventType.DragOver,
                    X = x,
                    Y = y,
                    Modifiers = _keyboard.Modifiers,
                    Data = data,
                });

        /// <summary>
        /// Dispatches a `drop` event.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public Task DropAsync(decimal x, decimal y, DragData data)
            => _connection.SendAsync(
                "Input.dispatchDragEvent",
                new InputDispatchDragEventRequest
                {
                    Type = DragEventType.Drop,
                    X = x,
                    Y = y,
                    Modifiers = _keyboard.Modifiers,
                    Data = data,
                });

        /// <summary>
        /// Performs a drag, dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="startX">Start X coordinate</param>
        /// <param name="startY">Start Y coordinate</param>
        /// <param name="endX">End X coordinate</param>
        /// <param name="endY">End Y coordinate</param>
        /// <param name="delay">If specified, is the time to wait between `dragover` and `drop` in milliseconds.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
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
    }
}
