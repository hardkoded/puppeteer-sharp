using System.Threading;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// CancellationTokenSource Helper.
    /// </summary>
    public static class CancellationTokenSourceHelper
    {
        /// <summary>
        /// Creates a CancellationToken with a timeout.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <returns>The timed cancellation token.</returns>
        public static CancellationToken GetCancellationToken(int timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            if (timeout == 0)
            {
                cancellationTokenSource.CancelAfter(-1);
            }
            else
            {
                cancellationTokenSource.CancelAfter(timeout);
            }

            var cancellationToken = cancellationTokenSource.Token;
            return cancellationToken;
        }
    }
}
