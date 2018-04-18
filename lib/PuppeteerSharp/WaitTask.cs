﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class WaitTask
    {
        private readonly Frame _frame;
        private readonly string _predicateBody;
        private readonly WaitForFunctionPollingOption _polling;
        private readonly int _timeout;
        private readonly object[] _args;
        private readonly Task _timeoutTimer;

        private readonly CancellationTokenSource _cts;
        private readonly TaskCompletionSource<JSHandle> _taskCompletion;

        private int _runCount = 0;
        private bool _terminated;

        private const string WaitForPredicatePageFunction = @"
async function waitForPredicatePageFunction(predicateBody, polling, timeout, ...args) {
  const predicate = new Function('...args', predicateBody);
  let timedOut = false;
  setTimeout(() => timedOut = true, timeout);
  if (polling === 'raf')
    return await pollRaf();
  if (polling === 'mutation')
    return await pollMutation();
  if (typeof polling === 'number')
    return await pollInterval(polling);

  /**
   * @return {!Promise<*>}
   */
  function pollMutation() {
    const success = predicate.apply(null, args);
    if (success)
      return Promise.resolve(success);

    let fulfill;
    const result = new Promise(x => fulfill = x);
    const observer = new MutationObserver(mutations => {
      if (timedOut) {
        observer.disconnect();
        fulfill();
      }
      const success = predicate.apply(null, args);
      if (success) {
        observer.disconnect();
        fulfill(success);
      }
    });
    observer.observe(document, {
      childList: true,
      subtree: true,
      attributes: true
    });
    return result;
  }

  /**
   * @return {!Promise<*>}
   */
  function pollRaf() {
    let fulfill;
    const result = new Promise(x => fulfill = x);
    onRaf();
    return result;

    function onRaf() {
      if (timedOut) {
        fulfill();
        return;
      }
      const success = predicate.apply(null, args);
      if (success)
        fulfill(success);
      else
        requestAnimationFrame(onRaf);
    }
  }

  /**
   * @param {number} pollInterval
   * @return {!Promise<*>}
   */
  function pollInterval(pollInterval) {
    let fulfill;
    const result = new Promise(x => fulfill = x);
    onTimeout();
    return result;

    function onTimeout() {
      if (timedOut) {
        fulfill();
        return;
      }
      const success = predicate.apply(null, args);
      if (success)
        fulfill(success);
      else
        setTimeout(onTimeout, pollInterval);
    }
  }
}";

        internal WaitTask(Frame frame, string predicateBody, WaitForFunctionPollingOption polling, int timeout, object[] args)
        {
            if (string.IsNullOrEmpty(predicateBody))
            {
                throw new ArgumentNullException(nameof(predicateBody));
            }

            _frame = frame;
            _predicateBody = $"return ( {predicateBody} )(...args)";
            _polling = polling;
            _timeout = timeout;
            _args = args;

            frame.WaitTasks.Add(this);
            _taskCompletion = new TaskCompletionSource<JSHandle>();

            _cts = new CancellationTokenSource();
            
            _timeoutTimer = System.Threading.Tasks.Task.Delay(timeout, _cts.Token).ContinueWith(_
                => Termiante(new PuppeteerException($"waiting failed: timeout {timeout}ms exceeded")));

            Rerun();
        }

        internal Task<JSHandle> Task => _taskCompletion.Task;

        internal async void Rerun()
        {
            var runCount = ++_runCount;
            JSHandle success = null;
            Exception exception = null;

            var context = await _frame.GetExecutionContextAsync();
            try
            {
                success = await context.EvaluateFunctionHandleAsync(WaitForPredicatePageFunction,
                    new object[] { _predicateBody, _polling, _timeout }.Concat(_args).ToArray());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (_terminated || runCount != _runCount)
            {
                if (success != null) await success.Dispose();
                return;
            }
            if (exception != null && await _frame.EvaluateFunctionAsync<bool>("s => !s", success))
            {
                if (success != null) await success.Dispose();
                return;
            }

            if (exception?.Message.Contains("Execution context was destroyed") == true)
            {
                return;
            }

            if (exception?.Message.Contains("Cannot find context with specified id") == true)
            {
                return;
            }

            if (exception != null)
            {
                _taskCompletion.SetException(exception);
            }
            else
            {
                _taskCompletion.SetResult(success);
            }
            Cleanup();
        }

        internal void Termiante(Exception exception)
        {
            _terminated = true;
            _taskCompletion.TrySetException(exception);
            Cleanup();
        }

        private void Cleanup()
        {
            _cts.Cancel();
            _frame.WaitTasks.Remove(this);
        }
    }
}