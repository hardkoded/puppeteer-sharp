using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// A protocol-agnostic description of an uncaught exception thrown in a page or worker.
    /// Dispatched via the <see cref="IPage.ExceptionThrown"/> and <see cref="WebWorker.ExceptionThrown"/> events.
    /// </summary>
    /// <remarks>
    /// Unlike the <see cref="IPage.PageError"/> event, <see cref="RemoteException"/> does not
    /// reconstruct an error value. When the protocol supports it (CDP), it exposes the live remote
    /// exception value through <see cref="Handle"/>. Each call frame of <see cref="StackTrace"/> is reported as a
    /// <see cref="ConsoleMessageLocation"/> (URL, line number and column number).
    /// </remarks>
    public class RemoteException
    {
        internal RemoteException(
            string text,
            string name,
            IReadOnlyList<ConsoleMessageLocation> stackTrace,
            ConsoleMessageLocation location,
            IJSHandle handle)
        {
            Text = text;
            Name = name;
            StackTrace = stackTrace ?? [];
            Location = (StackTrace.Count > 0 ? StackTrace[0] : location) ?? new ConsoleMessageLocation();
            Handle = handle;
        }

        /// <summary>
        /// Gets the text of the exception as rendered by the browser, for example <c>Error: boom</c>.
        /// For errors this is <c>${name}: ${message}</c> (or just the name if the message is empty), without the stack.
        /// For other thrown values it is their description, for example <c>Object</c> for a plain object.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the name of the exception, for example <c>Error</c>, <c>TypeError</c>, <c>SyntaxError</c> or <c>AbortError</c>.
        /// Empty string if the thrown value is not an error, for example a primitive, a plain object or a symbol.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the call frames on the stack at the time the exception was thrown, innermost first.
        /// Empty for compilation and syntax errors.
        /// </summary>
        public IReadOnlyList<ConsoleMessageLocation> StackTrace { get; }

        /// <summary>
        /// Gets the primary location where the exception occurred.
        /// For runtime errors this is the first element of <see cref="StackTrace"/>. For compilation and
        /// syntax errors (where <see cref="StackTrace"/> is empty), it falls back to the top-level
        /// coordinates reported for the exception.
        /// </summary>
        public ConsoleMessageLocation Location { get; }

        /// <summary>
        /// Gets the live remote exception value, if available. <c>null</c> for thrown primitives other than
        /// symbols, for exceptions thrown in other isolated worlds, and for exceptions thrown by code that
        /// Puppeteer injected.
        /// </summary>
        /// <remarks>
        /// The handle is disposed automatically right after the event has been dispatched. To keep using it
        /// beyond the synchronous part of your handler, call <see cref="IJSHandle.Move"/> synchronously inside
        /// the handler and dispose of the handle yourself once you are done with it.
        /// The handle is shared by all handlers of the event.
        /// </remarks>
        public IJSHandle Handle { get; }
    }
}
