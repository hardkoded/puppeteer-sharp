using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Task helper.
    /// </summary>
    public static class TaskHelper
    {
        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        public static async Task WithTimeout(this Task task, int milliseconds = 1_000)
        {
            var tcs = new TaskCompletionSource<bool>();
            var cancellationToken = new CancellationTokenSource();

            if (milliseconds > 0)
            {
                cancellationToken.CancelAfter(milliseconds);
            }

            using (cancellationToken.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new TimeoutException($"Timeout Exceeded: {milliseconds}ms exceeded");
                }
            }

            await task;
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static async Task<T> WithTimeout<T>(this Task<T> task, int milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            var cancellationToken = new CancellationTokenSource();

            if (milliseconds > 0)
            {
                cancellationToken.CancelAfter(milliseconds);
            }

            using (cancellationToken.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new TimeoutException($"Timeout Exceeded: {milliseconds}ms exceeded");
                }
            }

            return await task;
        }

        internal static async Task<T> WithConnectionCheck<T>(this Task<T> task, IConnection connection)
        {
            var tcs = new TaskCompletionSource<bool>();

            void Connection_Disconnected(object sender, EventArgs e) => tcs.SetResult(true);
            connection.Disconnected += Connection_Disconnected;

            try
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new TargetClosedException("Navigation failed because browser has disconnected!", connection.CloseReason);
                }
            }
            finally
            {
                connection.Disconnected -= Connection_Disconnected;
            }
            return await task;
        }
    }
}