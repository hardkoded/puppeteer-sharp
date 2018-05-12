using System;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal class TaskQueue
    {
        private SemaphoreSlim _semaphore;

        internal TaskQueue()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        internal async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        internal async Task Enqueue(Func<Task> taskGenerator)
        {
            await _semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}