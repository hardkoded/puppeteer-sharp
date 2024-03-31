using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

/*
    The implementation of transactions is not the same as in the original Puppeteer
    due to the differences in the threading model.
*/
namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public abstract class Mouse : IMouse
    {
        /// <inheritdoc/>
        public abstract Task MoveAsync(decimal x, decimal y, MoveOptions options = null);

        /// <inheritdoc/>
        public abstract Task ClickAsync(decimal x, decimal y, ClickOptions options = null);

        /// <inheritdoc/>
        public abstract Task DownAsync(ClickOptions options = null);

        /// <inheritdoc/>
        public abstract Task UpAsync(ClickOptions options = null);

        /// <inheritdoc/>
        public abstract Task WheelAsync(decimal deltaX, decimal deltaY);

        /// <inheritdoc/>
        public abstract Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY);

        /// <inheritdoc/>
        public abstract Task DragEnterAsync(decimal x, decimal y, DragData data);

        /// <inheritdoc/>
        public abstract Task DragOverAsync(decimal x, decimal y, DragData data);

        /// <inheritdoc/>
        public abstract Task DropAsync(decimal x, decimal y, DragData data);

        /// <inheritdoc/>
        public abstract Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0);

        /// <inheritdoc/>
        public abstract Task ResetAsync();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        protected abstract void Dispose(bool disposing);
    }
}
