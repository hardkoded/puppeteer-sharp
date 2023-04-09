using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal sealed class WaitTask : IDisposable
    {
        private const string WaitForPredicatePageFunction = @"
async function waitForPredicatePageFunction(
  root,
  predicateBody,
  predicateAcceptsContextElement,
  polling,
  timeout,
  ...args
) {
  root = root || document;
  const predicate = new Function('...args', predicateBody);
  let timedOut = false;
  if (timeout) setTimeout(() => (timedOut = true), timeout);
  if (polling === 'raf') return await pollRaf();
  if (polling === 'mutation') return await pollMutation();
  if (typeof polling === 'number') return await pollInterval(polling);

  /**
   * @returns {!Promise<*>}
   */
  async function pollMutation() {
    const success = predicateAcceptsContextElement
      ? await predicate(root, ...args)
      : await predicate(...args);
    if (success) return Promise.resolve(success);

    let fulfill;
    const result = new Promise((x) => (fulfill = x));
    const observer = new MutationObserver(async () => {
      if (timedOut) {
        observer.disconnect();
        fulfill();
      }
      const success = predicateAcceptsContextElement
        ? await predicate(root, ...args)
        : await predicate(...args);
      if (success) {
        observer.disconnect();
        fulfill(success);
      }
    });
    observer.observe(root, {
      childList: true,
      subtree: true,
      attributes: true,
    });
    return result;
  }

  async function pollRaf() {
    let fulfill;
    const result = new Promise((x) => (fulfill = x));
    await onRaf();
    return result;

    async function onRaf() {
      if (timedOut) {
        fulfill();
        return;
      }
      const success = predicateAcceptsContextElement
        ? await predicate(root, ...args)
        : await predicate(...args);
      if (success) fulfill(success);
      else requestAnimationFrame(onRaf);
    }
  }

  async function pollInterval(pollInterval) {
    let fulfill;
    const result = new Promise((x) => (fulfill = x));
    await onTimeout();
    return result;

    async function onTimeout() {
      if (timedOut) {
        fulfill();
        return;
      }
      const success = predicateAcceptsContextElement
        ? await predicate(root, ...args)
        : await predicate(...args);
      if (success) fulfill(success);
      else setTimeout(onTimeout, pollInterval);
    }
  }
}";

        private readonly IsolatedWorld _isolatedWorld;
        private readonly string _predicateBody;
        private readonly WaitForFunctionPollingOption _polling;
        private readonly int? _pollingInterval;
        private readonly int _timeout;
        private readonly object[] _args;
        private readonly string _title;
        private readonly Task _timeoutTimer;
        private readonly IElementHandle _root;
        private readonly bool _predicateAcceptsContextElement;
        private readonly CancellationTokenSource _cts;
        private readonly TaskCompletionSource<IJSHandle> _taskCompletion;
        private readonly PageBinding[] _bindings;

        private int _runCount;
        private bool _terminated;
        private bool _isDisposed;

        internal WaitTask(
            IsolatedWorld isolatedWorld,
            string predicateBody,
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
            if (string.IsNullOrEmpty(predicateBody))
            {
                throw new ArgumentNullException(nameof(predicateBody));
            }

            if (pollingInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), "Cannot poll with non-positive interval");
            }

            _isolatedWorld = isolatedWorld;
            _predicateBody = isExpression ? $"return ({predicateBody})" : $"return ({predicateBody})(...args)";
            _polling = polling;
            _pollingInterval = pollingInterval;
            _timeout = timeout;
            _args = args ?? Array.Empty<object>();
            _title = title;
            _root = root;
            _cts = new CancellationTokenSource();
            _predicateAcceptsContextElement = predicateAcceptsContextElement;
            _taskCompletion = new TaskCompletionSource<IJSHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
            _bindings = bidings ?? Array.Empty<PageBinding>();

            foreach (var binding in _bindings)
            {
                _isolatedWorld.BoundFunctions.AddOrUpdate(binding.Name, binding.Function, (_, __) => binding.Function);
            }

            _isolatedWorld.WaitTasks.Add(this);

            if (timeout > 0)
            {
                _timeoutTimer = System.Threading.Tasks.Task.Delay(timeout, _cts.Token)
                    .ContinueWith(
                        _ => Terminate(new WaitTaskTimeoutException(timeout, title)),
                        TaskScheduler.Default);
            }

            _ = Rerun();
        }

        internal Task<IJSHandle> Task => _taskCompletion.Task;

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
            var runCount = Interlocked.Increment(ref _runCount);
            IJSHandle success = null;
            Exception exception = null;

            var context = await _isolatedWorld.GetExecutionContextAsync().ConfigureAwait(false);
            await System.Threading.Tasks.Task.WhenAll(_bindings.Select(binding => _isolatedWorld.AddBindingToContextAsync(context, binding.Name))).ConfigureAwait(false);

            try
            {
                success = await context.EvaluateFunctionHandleAsync(
                    WaitForPredicatePageFunction,
                    new object[]
                    {
                        _root,
                        _predicateBody,
                        _predicateAcceptsContextElement,
                        _pollingInterval ?? (object)_polling,
                        _timeout,
                    }.Concat(_args).ToArray()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (_terminated || runCount != _runCount)
            {
                if (success != null)
                {
                    await success.DisposeAsync().ConfigureAwait(false);
                }

                return;
            }

            if (exception == null &&
                await _isolatedWorld.EvaluateFunctionAsync<bool>("s => !s", success)
                    .ContinueWith(
                        task => task.IsFaulted || task.Result,
                        TaskScheduler.Default)
                    .ConfigureAwait(false))
            {
                if (success != null)
                {
                    await success.DisposeAsync().ConfigureAwait(false);
                }

                return;
            }

            if (exception?.Message.Contains("Execution context was destroyed") == true)
            {
                _ = Rerun();
                return;
            }

            if (exception?.Message.Contains("Cannot find context with specified id") == true)
            {
                return;
            }

            if (exception != null)
            {
                _taskCompletion.TrySetException(exception);
            }
            else
            {
                _taskCompletion.TrySetResult(success);
            }

            Cleanup();
        }

        internal void Terminate(Exception exception)
        {
            _terminated = true;
            _taskCompletion.TrySetException(exception);
            Cleanup();
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

            _isolatedWorld.WaitTasks.Remove(this);
        }
    }
}
