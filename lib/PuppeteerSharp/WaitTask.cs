using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal sealed class WaitTask : IDisposable
    {
        private readonly IsolatedWorld _isolatedWorld;
        private readonly string _fn;
        private readonly WaitForFunctionPollingOption _polling;
        private readonly int? _pollingInterval;
        private readonly object[] _args;
        private readonly string _title;
        private readonly Task _timeoutTimer;
        private readonly IElementHandle _root;
        private readonly bool _predicateAcceptsContextElement;
        private readonly CancellationTokenSource _cts;
        private readonly TaskCompletionSource<IJSHandle> _result;
        private readonly PageBinding[] _bindings;

        private bool _isDisposed;
        private IJSHandle _poller;

        internal WaitTask(
            IsolatedWorld isolatedWorld,
            string fn,
            bool isExpression,
            string title,
            WaitForFunctionPollingOption polling,
            int? pollingInterval,
            int timeout,
            IElementHandle root,
            PageBinding[] bidings = null,
            object[] args = null,
            bool predicateAcceptsContextElement = false)
        {
            if (string.IsNullOrEmpty(fn))
            {
                throw new ArgumentNullException(nameof(fn));
            }

            if (pollingInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), "Cannot poll with non-positive interval");
            }

            _isolatedWorld = isolatedWorld;
            _fn = isExpression ? $"() => {{return ({fn});}}" : fn;
            _polling = polling;
            _pollingInterval = pollingInterval;
            _args = args ?? Array.Empty<object>();
            _title = title;
            _root = root;
            _cts = new CancellationTokenSource();
            _predicateAcceptsContextElement = predicateAcceptsContextElement;
            _result = new TaskCompletionSource<IJSHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
            _bindings = bidings ?? Array.Empty<PageBinding>();

            foreach (var binding in _bindings)
            {
                _isolatedWorld.BoundFunctions.AddOrUpdate(binding.Name, binding.Function, (_, __) => binding.Function);
            }

            _isolatedWorld.TaskManager.Add(this);

            if (timeout > 0)
            {
                _timeoutTimer = System.Threading.Tasks.Task.Delay(timeout, _cts.Token)
                    .ContinueWith(
                        _ => TerminateAsync(new WaitTaskTimeoutException(timeout, title)),
                        TaskScheduler.Default);
            }

            _ = Rerun();
        }

        internal Task<IJSHandle> Task => _result.Task;

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _cts.Dispose();

            _isDisposed = true;
        }

        internal async Task Rerun()
        {
            try
            {
                var context = await _isolatedWorld.GetExecutionContextAsync().ConfigureAwait(false);
                await System.Threading.Tasks.Task.WhenAll(_bindings.Select(binding => _isolatedWorld.AddBindingToContextAsync(context, binding.Name))).ConfigureAwait(false);

                _poller = _polling switch
                {
                    WaitForFunctionPollingOption.Raf => await _isolatedWorld.EvaluateFunctionHandleAsync(
                        @"
                        ({RAFPoller, createFunction}, fn, ...args) => {
                            const fun = createFunction(fn);
                            return new RAFPoller(() => {
                                return fun(...args);
                            });
                        }",
                        new object[]
                        {
                            await _isolatedWorld.GetPuppeteerUtilAsync().ConfigureAwait(false),
                            _fn,
                        }.Concat(_args).ToArray()).ConfigureAwait(false),
                    WaitForFunctionPollingOption.Mutation => await _isolatedWorld.EvaluateFunctionHandleAsync(
                        @"
                        ({MutationPoller, createFunction}, root, fn, ...args) => {
                            const fun = createFunction(fn);
                            return new MutationPoller(() => {
                            return fun(...args);
                            }, root || document);
                        }",
                        new object[]
                        {
                            await _isolatedWorld.GetPuppeteerUtilAsync().ConfigureAwait(false),
                            _root,
                            _fn,
                        }.Concat(_args).ToArray()).ConfigureAwait(false),
                    _ => await _isolatedWorld.EvaluateFunctionHandleAsync(
                        @"
                        ({IntervalPoller, createFunction}, ms, fn, ...args) => {
                            const fun = createFunction(fn);
                            return new IntervalPoller(() => {
                            return fun(...args);
                            }, ms);
                        }",
                        new object[]
                        {
                            await _isolatedWorld.GetPuppeteerUtilAsync().ConfigureAwait(false),
                            _pollingInterval,
                            _fn,
                        }.Concat(_args).ToArray()).ConfigureAwait(false),
                };
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
            _isolatedWorld.TaskManager.Delete(this);
            Cleanup(); // This matches the clearTimeout upstream

            if (exception != null)
            {
                _result.TrySetException(exception);
            }

            if (_poller != null)
            {
                try
                {
                    await _poller.EvaluateFunctionAsync(@"async poller => {
                        await poller.stop();
                    }").ConfigureAwait(false);

                    await _poller.DisposeAsync().ConfigureAwait(false);
                    _poller = null;
                }
                catch (Exception)
                {
                    // swallow error.
                }
            }
        }

        private Exception GetBadException(Exception exception)
        {
            if (exception.Message.Contains("Execution context is not available in detached frame"))
            {
                return new PuppeteerException("Waiting failed: Frame detached");
            }

            if (exception.Message.Contains("Execution context was destroyed") ||
                exception.Message.Contains("Cannot find context with specified id"))
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

            _isolatedWorld.TaskManager.Delete(this);
        }
    }
}
