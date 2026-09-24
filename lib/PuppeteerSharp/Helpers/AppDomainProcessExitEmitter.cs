using System;
using System.Collections.Concurrent;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// <see cref="IProcessExitEmitter"/> backed by <see cref="AppDomain.ProcessExit"/>.
    /// </summary>
    internal sealed class AppDomainProcessExitEmitter : IProcessExitEmitter
    {
        private readonly ConcurrentDictionary<Action, EventHandler> _handlers = new();

        public static AppDomainProcessExitEmitter Instance { get; } = new();

        public int ListenerCount => _handlers.Count;

        public void Once(Action listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            EventHandler handler = null;
            handler = (_, _) =>
            {
                AppDomain.CurrentDomain.ProcessExit -= handler;
                _handlers.TryRemove(listener, out _);
                listener();
            };

            if (!_handlers.TryAdd(listener, handler))
            {
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += handler;
        }

        public void Off(Action listener)
        {
            if (listener == null || !_handlers.TryRemove(listener, out var handler))
            {
                return;
            }

            AppDomain.CurrentDomain.ProcessExit -= handler;
        }
    }
}
