using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal sealed class WaitTask : IDisposable
    {
        private readonly Realm _realm;
        private readonly string _fn;
        private readonly WaitForFunctionPollingOption? _polling;
        private readonly int? _pollingInterval;
        private readonly object[] _args;
        private readonly Task _timeoutTimer;
        private readonly IElementHandle _root;
        private readonly CancellationTokenSource _cts = new();
        private readonly TaskCompletionSource<IJSHandle> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private bool _isDisposed;
        private IJSHandle _poller;
        private bool _terminated;

        internal WaitTask(
            Realm realm,
            string fn,
            bool isExpression,
            WaitForFunctionPollingOption polling,
            int? pollingInterval,
            int timeout,
            IElementHandle root,
            object[] args = null)
        {
            if (string.IsNullOrEmpty(fn))
            {
                throw new ArgumentNullException(nameof(fn));
            }

            if (pollingInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), "Cannot poll with non-positive interval");
            }

            _realm = realm;
            _fn = isExpression ? $"() => {{return ({fn});}}" : fn;
            _pollingInterval = pollingInterval;
            _polling = _pollingInterval.HasValue ? null : polling;
            _args = args ?? Array.Empty<object>();
            _root = root;

            _realm.TaskManager.Add(this);

            if (timeout > 0)
            {
                _timeoutTimer = System.Threading.Tasks.Task.Delay(timeout, _cts.Token)
                    .ContinueWith(
                        _ => TerminateAsync(new WaitTaskTimeoutException(timeout)),
                        TaskScheduler.Default);
            }

            _ = RerunAsync();
        }

        internal Task<IJSHandle> Task => _result.Task;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_timeoutTimer is { Status: TaskStatus.RanToCompletion or TaskStatus.Faulted or TaskStatus.Canceled } timeoutTimer)
            {
                timeoutTimer.Dispose();
            }

            _cts.Dispose();

            _isDisposed = true;
        }

        internal async Task RerunAsync()
        {
            try
            {
                if (_pollingInterval.HasValue)
                {
                    _poller = await _realm.EvaluateFunctionHandleAsync(
                            @"
                            ({IntervalPoller, createFunction}, ms, fn, ...args) => {
                                const fun = createFunction(fn);
                                return new IntervalPoller(() => {
                                return fun(...args);
                                }, ms);
                            }",
                            new object[]
                            {
                                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                                _pollingInterval,
                                _fn,
                            }.Concat(_args).ToArray()).ConfigureAwait(false);
                }
                else if (_polling == WaitForFunctionPollingOption.Raf)
                {
                    _poller = await _realm.EvaluateFunctionHandleAsync(
                            @"
                            ({RAFPoller, createFunction}, fn, ...args) => {
                                const fun = createFunction(fn);
                                return new RAFPoller(() => {
                                    return fun(...args);
                                });
                            }",
                            new object[]
                            {
                                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                                _fn,
                            }.Concat(_args).ToArray()).ConfigureAwait(false);
                }
                else
                {
                    _poller = await _realm.EvaluateFunctionHandleAsync(
                            @"
                            ({MutationPoller, createFunction}, root, fn, ...args) => {
                                const fun = createFunction(fn);
                                return new MutationPoller(() => {
                                    return fun(...args);
                                }, root || document);
                            }",
                            new object[]
                            {
                                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                                _root,
                                _fn,
                            }.Concat(_args).ToArray()).ConfigureAwait(false);
                }

                // Note that FrameWaitForFunctionTests listen for this particular message to orchestrate the test execution
                await _poller.EvaluateFunctionAsync("poller => poller.start()").ConfigureAwait(false);

                var success = await _poller.EvaluateFunctionHandleAsync("poller => poller.result()").ConfigureAwait(false);
                _result.TrySetResult(success);
                await TerminateAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exception = GetBadException(ex);
                if (exception != null)
                {
                    await TerminateAsync(exception).ConfigureAwait(false);
                }
            }
        }

        internal async Task TerminateAsync(Exception exception = null)
        {
            // The timeout timer might call this method on cleanup
            if (_terminated)
            {
                return;
            }

            _terminated = true;
            _realm.TaskManager.Delete(this);
            Cleanup(); // This matches the clearTimeout upstream

            if (exception != null)
            {
                _result.TrySetException(exception);
            }

            if (_poller is { } poller)
            {
                await using (poller.ConfigureAwait(false))
                {
                    try
                    {
                        await poller.EvaluateFunctionAsync(@"async poller => {
                            await poller.stop();
                        }").ConfigureAwait(false);

                        _poller = null;
                    }
                    catch (Exception)
                    {
                        // swallow error.
                    }
                }
            }
        }

        private Exception GetBadException(Exception exception)
        {
            // When frame is detached the task should have been terminated by the IsolatedWorld.
            // This can fail if we were adding this task while the frame was detached,
            // so we terminate here instead.
            if (exception.Message.Contains("Execution context is not available in detached frame"))
            {
                return new PuppeteerException("Waiting failed: Frame detached", exception);
            }

            // When the page is navigated, the promise is rejected.
            // We will try again in the new execution context.
            if (exception.Message.Contains("Execution context was destroyed"))
            {
                return null;
            }

            // We could have tried to evaluate in a context which was already destroyed.
            if (exception.Message.Contains("Cannot find context with specified id"))
            {
                return null;
            }

            // We don't have this check upstream.
            // We have a situation in our async code where a new navigation could be executed
            // before the WaitForFunction completes its initialization
            // See FrameWaitForSelectorTests.ShouldSurviveCrossProcessNavigation
            if (exception.Message.Contains("JSHandles can be evaluated only in the context they were created!"))
            {
                return null;
            }

            // This is a different message coming from Firefox in the same situation.
            // This is not upstream.
            if (exception.Message.Contains("Could not find object with given id"))
            {
                return null;
            }

            return exception;
        }

        private void Cleanup()
        {
            if (!_cts.IsCancellationRequested)
            {
                try
                {
                    _cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // Ignore
                }
            }
        }
    }
}
