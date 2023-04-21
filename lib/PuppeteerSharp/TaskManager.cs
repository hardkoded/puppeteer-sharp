using System;
using System.Linq;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class TaskManager
    {
        internal ConcurrentSet<WaitTask> WaitTasks { get; set; } = new ConcurrentSet<WaitTask>();

        internal void Add(WaitTask waitTask) => WaitTasks.Add(waitTask);

        internal void Delete(WaitTask waitTask) => WaitTasks.Remove(waitTask);

        internal void RerunAll()
        {
            foreach (var waitTask in WaitTasks)
            {
                _ = waitTask.Rerun();
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