﻿using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="Page.Console"/> data.
    /// </summary>
    public class ConsoleEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public ConsoleMessage Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ConsoleEventArgs(ConsoleMessage message) => Message = message;
    }
}