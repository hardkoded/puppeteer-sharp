using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public abstract class Touchscreen : ITouchscreen
    {
        private int _idGenerator;

        /// <summary>
        /// Gets the list of active touch handles.
        /// </summary>
        internal List<ITouchHandle> Touches { get; } = new();

        /// <inheritdoc />
        public async Task TapAsync(decimal x, decimal y)
        {
            var touch = await TouchStartAsync(x, y).ConfigureAwait(false);
            await touch.EndAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public abstract Task<ITouchHandle> TouchStartAsync(decimal x, decimal y);

        /// <inheritdoc />
        public async Task TouchMoveAsync(decimal x, decimal y)
        {
            if (Touches.Count == 0)
            {
                throw new PuppeteerException("Must start a new Touch first");
            }

            await Touches[0].MoveAsync(x, y).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task TouchEndAsync()
        {
            if (Touches.Count == 0)
            {
                throw new PuppeteerException("Must start a new Touch first");
            }

            var touch = Touches[0];
            Touches.RemoveAt(0);
            await touch.EndAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Generates a new incremental touch ID.
        /// </summary>
        /// <returns>A new touch ID.</returns>
        internal int GenerateId() => _idGenerator++;

        /// <summary>
        /// Removes a touch handle from the active touches list.
        /// </summary>
        /// <param name="handle">The touch handle to remove.</param>
        internal void RemoveHandle(ITouchHandle handle)
        {
            Touches.Remove(handle);
        }
    }
}
