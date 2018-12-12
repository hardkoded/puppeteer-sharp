using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Task helper.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Creates a timeout task. It will throw a <see cref="TimeoutException"/> after <paramref name="timeout"/> milliseconds
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>The timeout task.</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task CreateTimeoutTask(int timeout)
        {
            if (timeout == 0)
            {
                await Task.Delay(-1).ConfigureAwait(false);
            }
            else
            {
                await Task.Delay(timeout).ConfigureAwait(false);
                throw new TimeoutException($"Timeout Exceeded: {timeout}ms exceeded");
            }
        }
    }
}