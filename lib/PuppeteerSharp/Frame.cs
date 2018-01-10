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

        public Frame(Session client, Page page, Frame parentFrame, string frameId)
        {
            _client = client;
            _page = page;
            _id = frameId;
            _parentFrame = parentFrame;

            if (parentFrame != null)
            {
                _parentFrame.ChildFrames.Add(this);
            }
        }

        public List<Frame> ChildFrames { get; set; } = new List<Frame>();
        public object ExecutionContext => _context;
        public string Url { get; set; }

        internal async Task<ElementHandle> GetElementAsync(string selector)
        {
            throw new NotImplementedException();
        }

        internal async Task<ExecutionContext> GetExecutionContext()
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

        internal void OnLifecycleEvent(string loaderId, string name)
        {
            throw new NotImplementedException();
        }
    }
}
