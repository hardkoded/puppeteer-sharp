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
        private static readonly Func<TimeSpan, Exception> DefaultExceptionFactory =
            timeout => new TimeoutException($"Timeout of {timeout.TotalMilliseconds} ms exceeded");

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task WithTimeout(
            this Task task,
            int milliseconds = 1_000,
            Func<TimeSpan, Exception> exceptionFactory = null,
            CancellationToken cancellationToken = default)
            => WithTimeout(task, TimeSpan.FromMilliseconds(milliseconds), exceptionFactory, cancellationToken);

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task WithTimeout(
            this Task task,
            TimeSpan timeout,
            Func<TimeSpan, Exception> exceptionFactory = null,
            CancellationToken cancellationToken = default)
            => task.WithTimeout(
                () => throw (exceptionFactory ?? DefaultExceptionFactory)(timeout),
                timeout,
                cancellationToken);

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task WithTimeout(
            this Task task,
            Func<Task> timeoutAction,
            int milliseconds = 1_000,
            CancellationToken cancellationToken = default)
            => WithTimeout(task, timeoutAction, TimeSpan.FromMilliseconds(milliseconds), cancellationToken);

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task WithTimeout(this Task task, Func<Task> timeoutAction, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (timeoutAction == null)
            {
                throw new ArgumentNullException(nameof(timeoutAction));
            }

            if (await TimeoutTask(task, timeout, cancellationToken).ConfigureAwait(false))
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await timeoutAction().ConfigureAwait(false);
                }

                return;
            }

            await task.ConfigureAwait(false);
        }

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public static Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, int milliseconds = 1_000)
            => WithTimeout(task, timeoutAction, TimeSpan.FromMilliseconds(milliseconds));

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeoutAction">Action to be executed on Timeout.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public static async Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, TimeSpan timeout)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (timeoutAction == null)
            {
                throw new ArgumentNullException(nameof(timeoutAction));
            }

            if (await TimeoutTask(task, timeout).ConfigureAwait(false))
            {
                timeoutAction();
                return default;
            }

            return await task.ConfigureAwait(false);
        }

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after <paramref name="milliseconds"/> milliseconds.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="milliseconds">Milliseconds timeout.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static Task<T> WithTimeout<T>(this Task<T> task, int milliseconds = 1_000, Func<TimeSpan, Exception> exceptionFactory = null)
            => WithTimeout(task, TimeSpan.FromMilliseconds(milliseconds), exceptionFactory);

        // Recipe from https://blogs.msdn.microsoft.com/pfxteam/2012/10/05/how-do-i-cancel-non-cancelable-async-operations/

        /// <summary>
        /// Cancels the <paramref name="task"/> after a given <paramref name="timeout"/> period.
        /// </summary>
        /// <returns>The task result.</returns>
        /// <param name="task">Task to wait for.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="exceptionFactory">Optional timeout exception factory.</param>
        /// <typeparam name="T">Task return type.</typeparam>
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, Func<TimeSpan, Exception> exceptionFactory = null)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (await TimeoutTask(task, timeout).ConfigureAwait(false))
            {
                throw (exceptionFactory ?? DefaultExceptionFactory)(timeout);
            }

            return await task.ConfigureAwait(false);
        }

        private static async Task<bool> TimeoutTask(Task task, TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                await task.ConfigureAwait(false);
                return false;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var cancellationToken = new CancellationTokenSource(timeout);
            using (cancellationToken.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                return tcs.Task == await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
            }
        }

        private static async Task<bool> TimeoutTask(Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero)
            {
                await task.ConfigureAwait(false);
                return false;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCancellationToken.CancelAfter(timeout);

            using (linkedCancellationToken.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                return tcs.Task == await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
            }
        }
    }
}
