using System;
using System.Linq;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class TaskManager
    {
        private ConcurrentSet<WaitTask> WaitTasks { get; } = new();

        internal void Add(WaitTask waitTask) => WaitTasks.Add(waitTask);

        internal void Delete(WaitTask waitTask) => WaitTasks.Remove(waitTask);

        internal void RerunAll()
        {
            foreach (var waitTask in WaitTasks)
            {
                _ = waitTask.RerunAsync();
            }
        }

        internal void TerminateAll(Exception exception)
        {
            while (!WaitTasks.IsEmpty)
            {
                _ = WaitTasks.First().TerminateAsync(exception);
            }
        }
    }
}
