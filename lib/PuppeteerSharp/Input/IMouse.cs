using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Provides methods to interact with the mouse.
    /// </summary>
    public interface IMouse : IDisposable
    {
        /// <summary>
        /// Shortcut for <see cref="MoveAsync(decimal, decimal, MoveOptions)"/>, <see cref="DownAsync(ClickOptions)"/> and <see cref="UpAsync(ClickOptions)"/>.
        /// </summary>
        /// <param name="x">The target mouse X location to click.</param>
        /// <param name="y">The target mouse Y location to click.</param>
        /// <param name="options">Options to apply to the click operation.</param>
        /// <returns>Task.</returns>
        Task ClickAsync(decimal x, decimal y, ClickOptions options = null);

        /// <summary>
        /// Dispatches a <c>mousedown</c> event.
        /// </summary>
        /// <param name="options">Options to apply to the mouse down operation.</param>
        /// <returns>Task.</returns>
        Task DownAsync(ClickOptions options = null);

        /// <summary>
        /// Performs a drag, dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="startX">Start X coordinate.</param>
        /// <param name="startY">Start Y coordinate.</param>
        /// <param name="endX">End X coordinate.</param>
        /// <param name="endY">End Y coordinate.</param>
        /// <param name="delay">If specified, is the time to wait between `dragover` and `drop` in milliseconds.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0);

        /// <summary>
        /// Dispatches a `drag` event.
        /// </summary>
        /// <param name="startX">Start X coordinate.</param>
        /// <param name="startY">Start Y coordinate.</param>
        /// <param name="endX">End X coordinate.</param>
        /// <param name="endY">End Y coordinate.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser with the drag data.</returns>
        Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY);

        /// <summary>
        /// Dispatches a `dragenter` event.
        /// </summary>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DragEnterAsync(decimal x, decimal y, DragData data);

        /// <summary>
        /// Dispatches a `dragover` event.
        /// </summary>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DragOverAsync(decimal x, decimal y, DragData data);

        /// <summary>
        /// Dispatches a `drop` event.
        /// </summary>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DropAsync(decimal x, decimal y, DragData data);

        /// <summary>
        /// Dispatches a <c>mousemove</c> event.
        /// </summary>
        /// <param name="x">The destination mouse X coordinate.</param>
        /// <param name="y">The destination mouse Y coordinate.</param>
        /// <param name="options">Options to apply to the move operation.</param>
        /// <returns>Task.</returns>
        Task MoveAsync(decimal x, decimal y, MoveOptions options = null);

        /// <summary>
        /// Dispatches a <c>mouseup</c> event.
        /// </summary>
        /// <param name="options">Options to apply to the mouse up operation.</param>
        /// <returns>Task.</returns>
        Task UpAsync(ClickOptions options = null);

        /// <summary>
        /// Dispatches a <c>wheel</c> event.
        /// </summary>
        /// <param name="deltaX">Delta X.</param>
        /// <param name="deltaY">Delta Y.</param>
        /// <returns>Task.</returns>
        Task WheelAsync(decimal deltaX, decimal deltaY);

        /// <summary>
        /// Resets the mouse to the default state: No buttons pressed; position at (0,0).
        /// </summary>
        /// <returns>Task.</returns>
        Task ResetAsync();
    }
}
