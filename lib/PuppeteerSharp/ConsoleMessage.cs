﻿using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// ConsoleMessage is part of <see cref="ConsoleEventArgs"/> used by <see cref="Page.Console"/>
    /// </summary>
    public class ConsoleMessage
    {
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
        public IList<JSHandle> Args { get; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public ConsoleMessageLocation Location { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMessage"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="text">Text.</param>
        /// <param name="args">Arguments.</param>
        /// <param name="location">Message location</param>
        public ConsoleMessage(ConsoleType type, string text, IList<JSHandle> args, ConsoleMessageLocation location = null)
        {
            Type = type;
            Text = text;
            Args = args;
            Location = location;
        }
    }
}