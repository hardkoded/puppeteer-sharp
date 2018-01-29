using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.Contracts;
using PuppeteerSharp.Helpers;
using System.Threading;

namespace PuppeteerSharp
{
    internal class NavigationWatcher
    {
        private FrameManager _frameManager;
        private Frame _mainFrame;
        private dynamic _options;
        private static readonly Dictionary<string, string> _puppeteerToProtocolLifecycle = new Dictionary<string, string>()
        {
            {"load", "load"},
            {"domcontentloaded", "DOMContentLoaded"},
            {"networkidle0", "networkIdle"},
            {"networkidle2", "networkAlmostIdle"}
        };
        private IEnumerable<string> _expectedLifecycle;
        private int _timeout;
        private string _initialLoaderId;
        private Task _navigationPromise;

        public NavigationWatcher(FrameManager frameManager, Frame mainFrame, int timeout, dynamic options)
        {
            var waitUntil = new[] { "load" };

            if (options.waitUntil is Array)
            {
                waitUntil = options.waitUntil;
            }
            else if (options.waitUntil is string)
            {
                waitUntil = new string[] { options.waitUntil.ToString() };
            }

            _expectedLifecycle = waitUntil.Select(w =>
            {
                var protocolEvent = _puppeteerToProtocolLifecycle.GetValueOrDefault(w);
                Contract.Assert(protocolEvent != null, $"Unknown value for options.waitUntil: {w}");
                return protocolEvent;
            });

            _frameManager = frameManager;
            _mainFrame = mainFrame;
            _options = options;
            _initialLoaderId = mainFrame.LoaderId;
            _timeout = timeout;

            frameManager.LifecycleEvent += FrameManager_LifecycleEvent;
            frameManager.FrameDetached += FrameManager_LifecycleEvent;
            LifeCycleCompleteTaskWrapper = new TaskCompletionSource<bool>();

            _navigationPromise = Task.WhenAny(new[]
            {
                CreateTimeoutTask(),
                LifeCycleCompleteTask,
            }).ContinueWith((task) =>
            {
                CleanUp();
            });
        }

        #region Properties
        public Task<string> NavigationTask { get; internal set; }
        public Task<bool> LifeCycleCompleteTask => LifeCycleCompleteTaskWrapper.Task;
        public TaskCompletionSource<bool> LifeCycleCompleteTaskWrapper { get; }

        #endregion

        #region Public methods
        public void Cancel()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Private methods
        void FrameManager_LifecycleEvent(object sender, PuppeteerSharp.FrameEventArgs e)
        {

        }

        private void CleanUp()
        {
            throw new NotImplementedException();
        }

        private Task CreateTimeoutTask()
        {
            var wrapper = new TaskCompletionSource<bool>();

            if (_timeout == 0)
            {
                wrapper.SetResult(true);
            }
            else
            {
                Timer timer = null;
                timer = new Timer((state) =>
                {
                    wrapper.SetException(
                        new ChromeProcessException($"Navigation Timeout Exceeded: '{_timeout}'ms exceeded"));
                    timer.Dispose();
                }, null, _timeout, 0);
            }

            return wrapper.Task;
        }
        #endregion
    }
}