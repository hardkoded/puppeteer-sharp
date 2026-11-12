using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// ConsoleMessage is part of <see cref="ConsoleEventArgs"/> used by <see cref="IPage.Console"/>.
    /// </summary>
    public class ConsoleMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMessage"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="text">Text.</param>
        /// <param name="args">Arguments.</param>
        /// <param name="location">Message location.</param>
        /// <param name="stackTrace">Stack trace.</param>
        /// <param name="frame">The frame that initiated the message, if it originated from a frame.</param>
        /// <param name="worker">The worker that initiated the message, if it originated from a worker.</param>
        public ConsoleMessage(
            ConsoleType type,
            string text,
            IList<IJSHandle> args,
            ConsoleMessageLocation location = null,
            IList<ConsoleMessageLocation> stackTrace = null,
            IFrame frame = null,
            WebWorker worker = null)
        {
            Type = type;
            Text = text;
            Args = args;
            Location = location;
            StackTrace = stackTrace ?? [];
            Frame = frame;
            Worker = worker;
        }

        /// <summary>
        /// Gets the ConsoleMessage type.
        /// </summary>
        /// <value>ConsoleMessageType.</value>
        public ConsoleType Type { get; }

        /// <summary>
        /// Gets the console text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<IJSHandle> Args { get; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public ConsoleMessageLocation Location { get; }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        public IList<ConsoleMessageLocation> StackTrace { get; }

        /// <summary>
        /// Gets the <see cref="IPage"/> that the console message originated from, or <c>null</c> if the
        /// message originated from a worker or outside of a frame context.
        /// </summary>
        public IPage Page => Frame?.Page;

        /// <summary>
        /// Gets the <see cref="IFrame"/> that initiated the console message, or <c>null</c> if the
        /// message originated from a worker.
        /// </summary>
        public IFrame Frame { get; }

        /// <summary>
        /// Gets the <see cref="WebWorker"/> that initiated the console message, or <c>null</c> if the
        /// message originated from a frame.
        /// </summary>
        public WebWorker Worker { get; }
    }
}
