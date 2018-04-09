using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Frame
    {
        private Session _client;
        private Page _page;
        private Frame _parentFrame;
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
            Id = frameId;

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

        public async Task<dynamic> EvaluateExpressionAsync(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync(script);
        }

        public async Task<T> EvaluateExpressionAsync<T>(string script)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateExpressionAsync<T>(script);
        }

        public async Task<dynamic> EvaluateFunctionAsync(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync(script, args);
        }

        public async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var context = await GetExecutionContextAsync();
            return await context.EvaluateFunctionAsync<T>(script, args);
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

        internal async Task<ElementHandle> AddStyleTag(AddTagOptions options)
        {
            const string addStyleUrl = @"async (url) => {
                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = url;
                document.head.appendChild(link);
                await new Promise((res, rej) => {
                    link.onload = res;
                    link.onerror = rej;
                });
                return link;
            }";

            const string addStyleContent = @"(content) => {
                const style = document.createElement('style');
                style.type = 'text/css';
                style.appendChild(document.createTextNode(content));
                document.head.appendChild(style);
                return style;
            }";
            
            if (options.Url != null) {
                try {
                    return await EvaluateFunctionAsync<ElementHandle>(addStyleUrl, options.Url);
                } catch (PuppeteerException) {
                    throw new PuppeteerException($"Loading style from {options.Url} failed");
                }
            }
            
            if (options.Path != null) {
                var contents = File.ReadAllText(options.Path, Encoding.UTF8);
                contents += $"/*# sourceURL={Regex.Replace(options.Path, "\n", "")}*/";
                
                return await EvaluateFunctionAsync<ElementHandle>(addStyleContent, contents);
            }
            
            if (options.Content != null) {
                return await EvaluateFunctionAsync<ElementHandle>(addStyleContent, options.Content);
            }
            
            throw new PuppeteerException("Provide an object with a `Url`, `Path` or `Content` property");
        }

        internal Task<ElementHandle> AddScriptTag(AddTagOptions options)
        {
            throw new NotImplementedException();
        }

        internal async Task<string> GetContentAsync()
        {
            return await EvaluateFunctionAsync<string>(@"() => {
                let retVal = '';
                if (document.doctype)
                    retVal = new XMLSerializer().serializeToString(document.doctype);
                if (document.documentElement)
                    retVal += document.documentElement.outerHTML;
                return retVal;
            }");
        }

        internal async Task SetContentAsync(string html)
        {
            await EvaluateFunctionAsync(@"html => {
                document.open();
                document.write(html);
                document.close();
            }", html);
        }
        
        internal async Task<string> GetTitleAsync() => await EvaluateExpressionAsync<string>("document.title");

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