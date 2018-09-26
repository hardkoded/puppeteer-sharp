using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.Contracts;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class NavigatorWatcher
    {
        private static readonly Dictionary<WaitUntilNavigation, string> _puppeteerToProtocolLifecycle =
            new Dictionary<WaitUntilNavigation, string>
            {
                [WaitUntilNavigation.Load] = "load",
                [WaitUntilNavigation.DOMContentLoaded] = "DOMContentLoaded",
                [WaitUntilNavigation.Networkidle0] = "networkIdle",
                [WaitUntilNavigation.Networkidle2] = "networkAlmostIdle"
            };

        private readonly FrameManager _frameManager;
        private readonly Frame _frame;
        private readonly NavigationOptions _options;
        private readonly IEnumerable<string> _expectedLifecycle;
        private readonly int _timeout;
        private readonly string _initialLoaderId;

        private bool _hasSameDocumentNavigation;

        public NavigatorWatcher(FrameManager frameManager, Frame mainFrame, int timeout, NavigationOptions options)
        {
            var waitUntil = new[] { WaitUntilNavigation.Load };

            if (options?.WaitUntil != null)
            {
                waitUntil = options.WaitUntil;
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
            _hasSameDocumentNavigation = false;

            frameManager.LifecycleEvent += CheckLifecycleComplete;
            frameManager.FrameNavigatedWithinDocument += NavigatedWithinDocument;
            frameManager.FrameDetached += CheckLifecycleComplete;
            SameDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>();
            NewDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>();
            TimeoutTask = TaskHelper.CreateTimeoutTask(timeout);
        }

        #region Properties
        public Task<Task> NavigationTask { get; internal set; }
        public Task<bool> SameDocumentNavigationTask => SameDocumentNavigationTaskWrapper.Task;
        public TaskCompletionSource<bool> SameDocumentNavigationTaskWrapper { get; }
        public Task<bool> NewDocumentNavigationTask => NewDocumentNavigationTaskWrapper.Task;
        public TaskCompletionSource<bool> NewDocumentNavigationTaskWrapper { get; }
        public Task TimeoutTask { get; }


        #endregion

        #region Private methods

        private void CheckLifecycleComplete(object sender, FrameEventArgs e)
        {
            // We expect navigation to commit.
            if (_frame.LoaderId == _initialLoaderId && !_hasSameDocumentNavigation)
            {
                return;
            }
            if (!CheckLifecycle(_frame, _expectedLifecycle))
            {
                return;
            }

            if (_hasSameDocumentNavigation)
            {
                SameDocumentNavigationTaskWrapper.TrySetResult(true);
            }
            if (_frame.LoaderId != _initialLoaderId)
            {
                NewDocumentNavigationTaskWrapper.TrySetResult(true);
            }
        }

        private void NavigatedWithinDocument(object sender, FrameEventArgs e)
        {
            if (e.Frame != _frame)
            {
                return;
            }
            _hasSameDocumentNavigation = true;
            CheckLifecycleComplete(sender, e);
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
            _frameManager.LifecycleEvent -= CheckLifecycleComplete;
            _frameManager.FrameDetached -= CheckLifecycleComplete;
        }

        #endregion
    }
}