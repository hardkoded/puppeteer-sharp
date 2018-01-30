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
        private Frame _frame;
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
        private Timer _timer = null;

        public NavigationWatcher(FrameManager frameManager, Frame mainFrame, int timeout, dynamic options)
        {
            var waitUntil = new[] { "load" };

            if (options != null && options.waitUntil is Array)
            {
                waitUntil = options.waitUntil;
            }
            else if (options != null && options.waitUntil is string)
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
            _frame = mainFrame;
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
            CleanUp();
        }
        #endregion
        #region Private methods

        void FrameManager_LifecycleEvent(object sender, FrameEventArgs e)
        {
            // We expect navigation to commit.
            if (_frame.LoaderId == _initialLoaderId)
            {
                return;
            }
            if (!CheckLifecycle(_frame, _expectedLifecycle))
            {
                return;
            }

            LifeCycleCompleteTaskWrapper.SetResult(true);
        }

        private bool CheckLifecycle(Frame frame, IEnumerable<string> expectedLifecycle)
        {
            foreach (var item in expectedLifecycle)
            {
                if (!frame.LifecycleEvents.Contains(item))
                {
                    return false;
                }
            }
            foreach (var child in frame.ChildFrames)
            {
                if (!CheckLifecycle(child, expectedLifecycle))
                {
                    return false;
                }
            }
            return true;
        }

        private void CleanUp()
        {
            _frameManager.LifecycleEvent -= FrameManager_LifecycleEvent;
            _frameManager.FrameDetached -= FrameManager_LifecycleEvent;
            _timer?.Dispose();
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
                _timer = new Timer((state) =>
                {
                    wrapper.SetException(
                        new ChromeProcessException($"Navigation Timeout Exceeded: '{_timeout}'ms exceeded"));
                    _timer.Dispose();
                    _timer = null;
                }, null, _timeout, 0);
            }

            return wrapper.Task;
        }
        #endregion
    }
}