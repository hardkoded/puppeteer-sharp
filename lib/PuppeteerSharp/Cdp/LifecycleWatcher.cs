using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp
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
        private readonly CdpFrame _frame;
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
            CdpFrame frame,
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
            frame.FrameNavigatedWithinDocument += NavigatedWithinDocument;
            frame.FrameNavigated += Navigated;
            frame.FrameSwapped += FrameSwapped;
            frame.FrameSwappedByActivation += FrameSwapped;
            frame.FrameDetached += OnFrameDetached;
            _networkManager.Request += OnRequest;
            CheckLifecycleComplete();
        }

        public Task<bool> SameDocumentNavigationTask => _sameDocumentNavigationTaskWrapper.Task;

        public Task<bool> NewDocumentNavigationTask => _newDocumentNavigationTaskWrapper.Task;

        public CdpHttpResponse NavigationResponse => (CdpHttpResponse)_navigationRequest?.Response;

        public Task TerminationTask => _terminationTaskWrapper.Task.WithTimeout(_timeout, cancellationToken: _terminationCancellationToken.Token);

        public Task LifecycleTask => _lifecycleTaskWrapper.Task;

        public void Dispose()
        {
            _frame.FrameManager.LifecycleEvent -= FrameManager_LifecycleEvent;
            _frame.FrameNavigatedWithinDocument -= NavigatedWithinDocument;
            _frame.FrameNavigated -= Navigated;
            _frame.FrameDetached -= OnFrameDetached;
            _frame.FrameSwapped -= FrameSwapped;
            _networkManager.Request -= OnRequest;
            _terminationCancellationToken.Cancel();
            _terminationCancellationToken.Dispose();
        }

        private void Navigated(object sender, FrameNavigatedEventArgs e)
        {
            if (e.Type == NavigationType.BackForwardCacheRestore)
            {
                FrameSwapped(sender, EventArgs.Empty);
            }

            CheckLifecycleComplete();
        }

        private void FrameManager_LifecycleEvent(object sender, FrameEventArgs e) => CheckLifecycleComplete();

        private void FrameSwapped(object sender, EventArgs e)
        {
            _swapped = true;
            CheckLifecycleComplete();
        }

        private void OnFrameDetached(object sender, EventArgs e)
        {
            var frame = sender as Frame;
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

        private void NavigatedWithinDocument(object sender, EventArgs e)
        {
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
