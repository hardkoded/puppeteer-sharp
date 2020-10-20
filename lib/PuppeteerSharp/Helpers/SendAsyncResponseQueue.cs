using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Provides an async queue for responses for <see cref="CDPSession.SendAsync"/>, so that responses can be handled
    /// async without risk callers causing a deadlock.
    /// </summary>
    /// <remarks>
    /// See https://github.com/hardkoded/puppeteer-sharp/issues/1354
    /// </remarks>
    internal class SendAsyncResponseQueue : IDisposable
    {
        private bool _disposed;
        private readonly List<MessageTask> _pendingTasks;
        private readonly bool _enqueueSendAsyncResponses;
        private readonly ILogger _logger;

        public SendAsyncResponseQueue(bool enqueueSendAsyncResponses, ILogger logger = null)
        {
            _enqueueSendAsyncResponses = enqueueSendAsyncResponses;
            _logger = logger ?? NullLogger.Instance;
            _pendingTasks = new List<MessageTask>();
        }

        public void Enqueue(MessageTask callback, ConnectionResponse obj)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (!_enqueueSendAsyncResponses)
            {
                HandleAsyncMessage(callback, obj);
                return;
            }

            // Keep a ref to this task until it completes. If it can't finish by the time we dispose this queue,
            // then we'll find it and cancel it.
            lock (_pendingTasks)
            {
                _pendingTasks.Add(callback);
            }

            var task = Task.Run(() => HandleAsyncMessage(callback, obj));

            // Unhandled error handler
            task.ContinueWith(t =>
            {
                _logger.LogError(t.Exception, "Failed to complete async handling of SendAsync for {callback}", callback.Method);
                callback.TaskWrapper.TrySetException(t.Exception!); // t.Exception is available since this runs only on faulted
            }, TaskContinuationOptions.OnlyOnFaulted);

            // Always remove from the queue when done, regardless of outcome.
            task.ContinueWith(_ =>
            {
                lock (_pendingTasks)
                {
                    _pendingTasks.Remove(callback);
                }
            });
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // Ensure all tasks are finished since we're disposing now. Any pending tasks will be canceled.
            MessageTask[] pendingTasks;
            lock (_pendingTasks)
            {
                pendingTasks = _pendingTasks.ToArray();
                _pendingTasks.Clear();
            }

            foreach (var pendingTask in pendingTasks)
            {
                pendingTask.TaskWrapper.TrySetCanceled();
            }

            _disposed = true;
        }

        private static void HandleAsyncMessage(MessageTask callback, ConnectionResponse obj)
        {
            if (obj.Error != null)
            {
                callback.TaskWrapper.TrySetException(new MessageException(callback, obj.Error));
            }
            else
            {
                callback.TaskWrapper.TrySetResult(obj.Result);
            }
        }
    }
}
