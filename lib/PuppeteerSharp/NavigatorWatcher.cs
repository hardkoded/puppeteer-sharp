using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.Contracts;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class NavigatorWatcher : IDisposable
    {
        private static readonly Dictionary<WaitUntilNavigation, string> _puppeteerToProtocolLifecycle =
            new Dictionary<WaitUntilNavigation, string>
            {
                [WaitUntilNavigation.Load] = "load",
                [WaitUntilNavigation.DOMContentLoaded] = "DOMContentLoaded",
                [WaitUntilNavigation.Networkidle0] = "networkIdle",
                [WaitUntilNavigation.Networkidle2] = "networkAlmostIdle"
            };
        private static readonly WaitUntilNavigation[] _defaultWaitUntil = new[] { WaitUntilNavigation.Load };

        private readonly IConnection _connection;
        private readonly NetworkManager _networkManager;
        private readonly FrameManager _frameManager;
        private readonly Frame _frame;
        private readonly IEnumerable<string> _expectedLifecycle;
        private readonly string _initialLoaderId;
        private Request _navigationRequest;
        private bool _hasSameDocumentNavigation;
        private TaskCompletionSource<bool> _newDocumentNavigationTaskWrapper;
        private TaskCompletionSource<bool> _sameDocumentNavigationTaskWrapper;
        private TaskCompletionSource<bool> _terminationTaskWrapper;
        private Task _timeoutTask;

        public NavigatorWatcher(
            CDPSession client,
            FrameManager frameManager,
            Frame mainFrame,
            NetworkManager networkManager,
            int timeout,
            NavigationOptions options)
        {
            var waitUntil = _defaultWaitUntil;

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
            _networkManager = networkManager;
            _frame = mainFrame;
            _initialLoaderId = mainFrame.LoaderId;
            _hasSameDocumentNavigation = false;

            frameManager.LifecycleEvent += CheckLifecycleComplete;
            frameManager.FrameNavigatedWithinDocument += NavigatedWithinDocument;
            frameManager.FrameDetached += OnFrameDetached;
            networkManager.Request += OnRequest;
            _connection = Connection.FromSession(client);
            _connection.Closed += OnConnectionClosed;

            _sameDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>();
            _newDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>();
            _terminationTaskWrapper = new TaskCompletionSource<bool>();
            _timeoutTask = TaskHelper.CreateTimeoutTask(timeout);
        }

        #region Properties
        public Task<Task> NavigationTask { get; internal set; }
        public Task<bool> SameDocumentNavigationTask => _sameDocumentNavigationTaskWrapper.Task;
        public Task<bool> NewDocumentNavigationTask => _newDocumentNavigationTaskWrapper.Task;
        public Response NavigationResponse => _navigationRequest?.Response;
        public Task<Task> TimeoutOrTerminationTask => Task.WhenAny(_timeoutTask, _terminationTaskWrapper.Task);

        #endregion

        #region Private methods

        private void OnConnectionClosed(object sender, EventArgs e) 
            => Terminate(new TargetClosedException("Navigation failed because browser has disconnected!", _connection.CloseReason));

        private void OnFrameDetached(object sender, FrameEventArgs e)
        {
            var frame = e.Frame;
            if (_frame == frame)
            {
                Terminate(new PuppeteerException("Navigating frame was detached"));
                return;
            }
            CheckLifecycleComplete(sender, e);
        }

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
                _sameDocumentNavigationTaskWrapper.TrySetResult(true);
            }
            if (_frame.LoaderId != _initialLoaderId)
            {
                _newDocumentNavigationTaskWrapper.TrySetResult(true);
            }
        }

        private void Terminate(PuppeteerException ex) => _terminationTaskWrapper.TrySetException(ex);

        private void OnRequest(object sender, RequestEventArgs e)
        {
            if (e.Request.Frame != _frame || !e.Request.IsNavigationRequest)
            {
                return;
            }
            _navigationRequest = e.Request;
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

        public void Dispose() => Dispose(true);

        ~NavigatorWatcher() => Dispose(false);

        public void Dispose(bool disposing)
        {
            _frameManager.LifecycleEvent -= CheckLifecycleComplete;
            _frameManager.FrameNavigatedWithinDocument -= NavigatedWithinDocument;
            _frameManager.FrameDetached -= OnFrameDetached;
            _networkManager.Request -= OnRequest;
            _connection.Closed -= OnConnectionClosed;
        }

        #endregion
    }
}