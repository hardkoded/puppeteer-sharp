using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class DOMWorld
    {
        private readonly FrameManager _frameManager;
        private readonly Frame _frame;
        private readonly List<WaitTask> _waitTasks;

        private bool _detached;
        private Task<ElementHandle> _documentTask;
        private Task<ExecutionContext> _contextTask;

        private TaskCompletionSource<ExecutionContext> _contextResolveTaskWrapper;
        private TaskCompletionSource<ElementHandle> _documentCompletionSource;

        public DOMWorld(FrameManager frameManager, Frame frame)
        {
            _frameManager = frameManager;
            _frame = frame;
            
            SetContext(null);

            _waitTasks = new List<WaitTask>();
            _detached = false;
        }

        private void SetContext(ExecutionContext context)
        {
            if (context != null)
            {
                _contextResolveTaskWrapper.TrySetResult(context);
                foreach (var waitTask in _waitTasks)
                {
                    _ = waitTask.Rerun();
                }
            }
            else
            {
                _documentCompletionSource = null;
                _contextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>();
            }
        }

        internal void Detach()
        {
            _detached = true;
            while (_waitTasks.Count > 0)
            {
                _waitTasks[0].Terminate(new Exception("waitForFunction failed: frame got detached."));
            }
        }

        internal Task<ExecutionContext> GetExecutionContextAsync()
        {
            if(_detached)
            {

            }
            return _contextResolveTaskWrapper.Task;
        }
    }
}
