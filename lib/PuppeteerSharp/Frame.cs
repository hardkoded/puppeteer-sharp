using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    public class Frame
    {
        private Session _client;
        private Page _page;
        private Frame _parentFrame;
        private string _id;
        private string _defaultContextId = "<not-initialized>";
        private object _context = null;
        private string _url = string.Empty;
        private List<WaitTask> _waitTasks;
        private bool _detached;

        public Frame(Session client, Page page, Frame parentFrame, string frameId)
        {
            _client = client;
            _page = page;
            _parentFrame = parentFrame;
            _id = frameId;

            if (parentFrame != null)
            {
                _parentFrame.ChildFrames.Add(this);
            }

            SetDefaultContext(null);

            _waitTasks = new List<WaitTask>();
            LifecycleEvents = new List<string>();
        }

        #region Properties
        public List<Frame> ChildFrames { get; set; } = new List<Frame>();
        public string Name { get; set; }

        public string Url { get; set; }
        public string ParentId { get; internal set; }
        public string Id { get; internal set; }
        public string LoaderId { get; set; }
        public TaskCompletionSource<ExecutionContext> ContextResolveTaskWrapper { get; internal set; }
        public List<string> LifecycleEvents { get; internal set; }

        #endregion

        #region Public Methods

        public async Task<dynamic> EvaluateAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateAsync(script, args);
        }

        public async Task<T> EvaluateAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateAsync<T>(script, args);
        }

        public Task<ExecutionContext> GetExecutionContextAsync() => ContextResolveTaskWrapper.Task;

        internal Task<ElementHandle> GetElementAsync(string selector)
        {
            throw new NotImplementedException();
        }

        internal Task<object> Eval(string selector, Func<object> pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> Eval(string selector, string pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> EvalMany(string selector, Func<object> pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal Task<object> EvalMany(string selector, string pageFunction, object[] args)
        {
            throw new NotImplementedException();
        }

        internal async Task<IEnumerable<ElementHandle>> GetElementsAsync(string selector)
        {
            throw new NotImplementedException();
        }

        internal Task<ElementHandle> AddStyleTag(dynamic options)
        {
            throw new NotImplementedException();
        }

        internal Task<ElementHandle> AddScriptTag(dynamic options)
        {
            throw new NotImplementedException();
        }

        internal async Task<string> GetTitleAsync()
        {
            var result = await EvaluateAsync<string>("document.title");
            return result;
        }

        internal void OnLifecycleEvent(string loaderId, string name)
        {
            if (name == "init")
            {
                LoaderId = loaderId;
                LifecycleEvents.Clear();
            }
            LifecycleEvents.Add(name);
        }

        internal void Navigated(FramePayload framePayload)
        {
            Name = framePayload.Name;
            Url = framePayload.Url;
        }

        internal void SetDefaultContext(ExecutionContext context)
        {
            if (context != null)
            {
                ContextResolveTaskWrapper.SetResult(context);

                foreach (var waitTask in _waitTasks)
                {
                    waitTask.Rerun();
                }
            }
            else
            {
                ContextResolveTaskWrapper = new TaskCompletionSource<ExecutionContext>();
            }
        }

        internal void Detach()
        {
            foreach (var waitTask in _waitTasks)
            {
                waitTask.Termiante(new Exception("waitForSelector failed: frame got detached."));
            }
            _detached = true;
            if (_parentFrame != null)
            {
                _parentFrame.ChildFrames.Remove(this);
            }
            _parentFrame = null;
        }

        #endregion
    }
}