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
        private static readonly Func<int, Exception> DefaultExceptionFactory =
            msecs => new TimeoutException($"Timeout Exceeded: {msecs}ms exceeded");

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        public static Task WithTimeout(this Task task, int milliseconds = 1_000, Func<int, Exception> exceptionFactory = null)
            => task.WithTimeout(
                () => throw (exceptionFactory ?? DefaultExceptionFactory)(milliseconds),
                milliseconds);

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        public static Task WithTimeout(this Task task, TimeSpan timeout, Func<int, Exception> exceptionFactory = null) 
            => WithTimeout(task, ToTimeoutInt32(timeout), exceptionFactory);

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        public static async Task WithTimeout(this Task task, Func<Task> timeoutAction, int milliseconds = 1_000)
        {
            if (await TimeoutTask(task, milliseconds))
            {
                await timeoutAction();
            }

            await task;
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="timeout">The timeout period.</param>
        public static Task WithTimeout(this Task task, Func<Task> timeoutAction, TimeSpan timeout)
            => WithTimeout(task, timeoutAction, ToTimeoutInt32(timeout));

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        public static async Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, int milliseconds = 1_000)
        {
            if (await TimeoutTask(task, milliseconds))
            {
                timeoutAction();
                return default;
            }

            return await task;
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="timeout">The timeout period.</param>
        public static Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, TimeSpan timeout)
            => WithTimeout(task, timeoutAction, ToTimeoutInt32(timeout));

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static async Task<T> WithTimeout<T>(this Task<T> task, int milliseconds = 1_000, Func<int, Exception> exceptionFactory = null)
        {
            if (await TimeoutTask(task, milliseconds))
            {
                throw (exceptionFactory ?? DefaultExceptionFactory)(milliseconds);
            }

            return await task;
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, Func<int, Exception> exceptionFactory = null)
            => WithTimeout(task, ToTimeoutInt32(timeout), exceptionFactory);

        private static async Task<bool> TimeoutTask(Task task, int milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (var cancellationToken = new CancellationTokenSource())
            {
                if (milliseconds > 0)
                {
                    cancellationToken.CancelAfter(milliseconds);
                }
                using (cancellationToken.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                {
                    if (task != await Task.WhenAny(task, tcs.Task))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static int ToTimeoutInt32(TimeSpan timeout)
        {
            var totalMilliseconds = timeout.TotalMilliseconds;
            if (totalMilliseconds > int.MaxValue || totalMilliseconds < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            return (int)totalMilliseconds;
        }
    }
}