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
        /// <param name="exceptionToThrow">Optional exception to be thrown.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static Task WithTimeout(
            this Task task,
            int milliseconds = 1_000,
            Exception exceptionToThrow = null,
            CancellationToken cancellationToken = default)
            => task.WithTimeout(
                () => throw exceptionToThrow ?? new TimeoutException($"Timeout Exceeded: {milliseconds}ms exceeded"),
                milliseconds,
                cancellationToken);

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task WithTimeout(
            this Task task,
            Func<Task> timeoutAction,
            int milliseconds = 1_000,
            CancellationToken cancellationToken = default)
        {
            if (await TimeoutTask(task, milliseconds) && !cancellationToken.IsCancellationRequested)
            {
                await timeoutAction();
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await task;
            }
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task<T> WithTimeout<T>(
            this Task<T> task,
            Action timeoutAction,
            int milliseconds = 1_000,
            CancellationToken cancellationToken = default)
        {
            if (await TimeoutTask(task, milliseconds) && !cancellationToken.IsCancellationRequested)
            {
                timeoutAction();
                return default;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                return await task;
            }
            return default;
        }

        //Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/
        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="exceptionToThrow">Optional exception to be thrown.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static async Task<T> WithTimeout<T>(
            this Task<T> task,
            int milliseconds = 1_000,
            Exception exceptionToThrow = null,
            CancellationToken cancellationToken = default)
        {
            if (await TimeoutTask(task, milliseconds) && !cancellationToken.IsCancellationRequested)
            {
                throw exceptionToThrow ?? new TimeoutException($"Timeout Exceeded: {milliseconds}ms exceeded");
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                return await task;
            }

            return default;
        }

        private static async Task<bool> TimeoutTask(Task task, int milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var cancellationToken = new CancellationTokenSource();

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
}