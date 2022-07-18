using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers;

namespace CefSharp.DevTools.Dom
{
    internal class LifecycleWatcher : IDisposable
    {
        private static readonly Dictionary<WaitUntilNavigation, string> _puppeteerToProtocolLifecycle =
            new Dictionary<WaitUntilNavigation, string>
            {
                [WaitUntilNavigation.Load] = "load",
                [WaitUntilNavigation.DOMContentLoaded] = "DOMContentLoaded",
                [WaitUntilNavigation.Networkidle0] = "networkIdle",
                [WaitUntilNavigation.Networkidle2] = "networkAlmostIdle"
            };

        private static readonly WaitUntilNavigation[] _defaultWaitUntil = { WaitUntilNavigation.Load };

        private readonly FrameManager _frameManager;
        private readonly Frame _frame;
        private readonly IEnumerable<string> _expectedLifecycle;
        private readonly int _timeout;
        private readonly string _initialLoaderId;
        private readonly TaskCompletionSource<bool> _newDocumentNavigationTaskWrapper;
        private readonly TaskCompletionSource<bool> _sameDocumentNavigationTaskWrapper;
        private readonly TaskCompletionSource<bool> _lifecycleTaskWrapper;
        private readonly TaskCompletionSource<bool> _terminationTaskWrapper;
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False positive, as it is disposed.")]
        private readonly CancellationTokenSource _terminationCancellationToken;
        private Request _navigationRequest;
        private bool _hasSameDocumentNavigation;

        public LifecycleWatcher(
            FrameManager frameManager,
            Frame frame,
            WaitUntilNavigation[] waitUntil,
            int timeout)
        {
            _expectedLifecycle = (waitUntil ?? _defaultWaitUntil).Select(w =>
            {
                var protocolEvent = _puppeteerToProtocolLifecycle.GetValueOrDefault(w);
                Contract.Assert(protocolEvent != null, $"Unknown value for options.waitUntil: {w}");
                return protocolEvent;
            });

            _frameManager = frameManager;
            _frame = frame;
            _initialLoaderId = frame.LoaderId;
            _timeout = timeout;
            _hasSameDocumentNavigation = false;

            _sameDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _newDocumentNavigationTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _lifecycleTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _terminationTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _terminationCancellationToken = new CancellationTokenSource();

            frameManager.LifecycleEvent += FrameManager_LifecycleEvent;
            frameManager.FrameNavigatedWithinDocument += NavigatedWithinDocument;
            frameManager.FrameDetached += OnFrameDetached;
            frameManager.NetworkManager.Request += OnRequest;
            frameManager.Connection.Disconnected += OnClientDisconnected;

            CheckLifecycleComplete();
        }

        public Task<bool> SameDocumentNavigationTask => _sameDocumentNavigationTaskWrapper.Task;

        public Task<bool> NewDocumentNavigationTask => _newDocumentNavigationTaskWrapper.Task;

        public Response NavigationResponse => _navigationRequest?.Response;

        public Task TimeoutOrTerminationTask => _terminationTaskWrapper.Task.WithTimeout(_timeout, cancellationToken: _terminationCancellationToken.Token);

        public Task LifecycleTask => _lifecycleTaskWrapper.Task;

        private void OnClientDisconnected(object sender, EventArgs e)
            => Terminate(new TargetClosedException("Navigation failed because browser has disconnected!", _frameManager.Connection.CloseReason));

        private void FrameManager_LifecycleEvent(object sender, LifecycleEventArgs e) => CheckLifecycleComplete();

        private void OnFrameDetached(object sender, FrameEventArgs e)
        {
            var frame = e.Frame;
            if (_frame == frame)
            {
                Terminate(new PuppeteerException("Navigating frame was detached"));
                return;
            }
            CheckLifecycleComplete();
        }

        private void CheckLifecycleComplete()
        {
            // We expect navigation to commit.
            if (!CheckLifecycle(_frame, _expectedLifecycle))
            {
                return;
            }
            _lifecycleTaskWrapper.TrySetResult(true);
            if (_frame.LoaderId == _initialLoaderId && !_hasSameDocumentNavigation)
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
            CheckLifecycleComplete();
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

        public void Dispose()
        {
            _frameManager.LifecycleEvent -= FrameManager_LifecycleEvent;
            _frameManager.FrameNavigatedWithinDocument -= NavigatedWithinDocument;
            _frameManager.FrameDetached -= OnFrameDetached;
            _frameManager.NetworkManager.Request -= OnRequest;
            _frameManager.Connection.Disconnected -= OnClientDisconnected;
            _terminationCancellationToken.Cancel();
        }
    }
}
