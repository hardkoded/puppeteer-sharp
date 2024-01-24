using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal sealed class LifecycleWatcher : IDisposable
    {
        private static readonly Dictionary<WaitUntilNavigation, string> _puppeteerToProtocolLifecycle =
            new()
            {
                [WaitUntilNavigation.Load] = "load",
                [WaitUntilNavigation.DOMContentLoaded] = "DOMContentLoaded",
                [WaitUntilNavigation.Networkidle0] = "networkIdle",
                [WaitUntilNavigation.Networkidle2] = "networkAlmostIdle",
            };

        private static readonly WaitUntilNavigation[] _defaultWaitUntil = [WaitUntilNavigation.Load];

        private readonly NetworkManager _networkManager;
        private readonly Frame _frame;
        private readonly IEnumerable<string> _expectedLifecycle;
        private readonly int _timeout;
        private readonly string _initialLoaderId;
        private readonly TaskCompletionSource<bool> _newDocumentNavigationTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _sameDocumentNavigationTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _lifecycleTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> _terminationTaskWrapper = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly CancellationTokenSource _terminationCancellationToken = new();
        private IRequest _navigationRequest;
        private bool _hasSameDocumentNavigation;
        private bool _swapped;

        public LifecycleWatcher(
            NetworkManager networkManager,
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

            _networkManager = networkManager;
            _frame = frame;
            _initialLoaderId = frame.LoaderId;
            _timeout = timeout;
            _hasSameDocumentNavigation = false;

            frame.FrameManager.LifecycleEvent += FrameManager_LifecycleEvent;
            frame.FrameManager.FrameNavigatedWithinDocument += NavigatedWithinDocument;
            frame.FrameManager.FrameNavigated += Navigated;
            frame.FrameManager.FrameDetached += OnFrameDetached;
            _networkManager.Request += OnRequest;
            frame.FrameManager.Client.Disconnected += OnClientDisconnected;
            frame.FrameManager.FrameSwapped += FrameManager_FrameSwapped;
            CheckLifecycleComplete();
        }

        public Task<bool> SameDocumentNavigationTask => _sameDocumentNavigationTaskWrapper.Task;

        public Task<bool> NewDocumentNavigationTask => _newDocumentNavigationTaskWrapper.Task;

        public Response NavigationResponse => (Response)_navigationRequest?.Response;

        public Task TerminationTask => _terminationTaskWrapper.Task.WithTimeout(_timeout, cancellationToken: _terminationCancellationToken.Token);

        public Task LifecycleTask => _lifecycleTaskWrapper.Task;

        public void Dispose()
        {
            _frame.FrameManager.LifecycleEvent -= FrameManager_LifecycleEvent;
            _frame.FrameManager.FrameNavigatedWithinDocument -= NavigatedWithinDocument;
            _frame.FrameManager.FrameNavigated -= Navigated;
            _frame.FrameManager.FrameDetached -= OnFrameDetached;
            _frame.FrameManager.NetworkManager.Request -= OnRequest;
            _frame.FrameManager.Client.Disconnected -= OnClientDisconnected;
            _frame.FrameManager.FrameSwapped -= FrameManager_FrameSwapped;
            _terminationCancellationToken.Cancel();
            _terminationCancellationToken.Dispose();
        }

        private void OnClientDisconnected(object sender, EventArgs e)
            => Terminate(new TargetClosedException("Navigation failed because browser has disconnected!", _frame.FrameManager.Client.CloseReason));

        private void Navigated(object sender, FrameEventArgs e)
        {
            if (e.Frame != _frame)
            {
                return;
            }

            CheckLifecycleComplete();
        }

        private void FrameManager_LifecycleEvent(object sender, FrameEventArgs e) => CheckLifecycleComplete();

        private void FrameManager_FrameSwapped(object sender, FrameEventArgs e)
        {
            if (e.Frame != _frame)
            {
                return;
            }

            _swapped = true;
            CheckLifecycleComplete();
        }

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

            if (_hasSameDocumentNavigation)
            {
                _sameDocumentNavigationTaskWrapper.TrySetResult(true);
            }

            if (_swapped || _frame.LoaderId != _initialLoaderId)
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
            var enumerable = expectedLifecycle as string[] ?? expectedLifecycle.ToArray();
            foreach (var item in enumerable)
            {
                if (!frame.LifecycleEvents.Contains(item))
                {
                    return false;
                }
            }

            foreach (var childFrame in frame.ChildFrames)
            {
                var child = (Frame)childFrame;
                if (child.HasStartedLoading && !CheckLifecycle(child, enumerable))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
