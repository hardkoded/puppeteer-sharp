using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.Issue"/> event arguments.
    /// </summary>
    public class IssueEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssueEventArgs"/> class.
        /// </summary>
        /// <param name="issue">The issue that was reported.</param>
        public IssueEventArgs(Issue issue) => Issue = issue;

        /// <summary>
        /// Gets the reported issue.
        /// </summary>
        public Issue Issue { get; }
    }
}
