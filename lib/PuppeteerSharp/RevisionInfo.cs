using System;
using System.IO;

namespace PuppeteerSharp
{
    /// <summary>
    /// Revision info.
    /// </summary>
    public class RevisionInfo
    {
        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public int Revision { get; set; }
        /// <summary>
        /// Gets or sets the folder path.
        /// </summary>
        /// <value>The folder path.</value>
        public string FolderPath { get; set; }
        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        /// <value>The executable path.</value>
        public string ExecutablePath { get; set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="RevisionInfo"/> is downloaded.
        /// </summary>
        /// <value><c>true</c> if <see cref="RevisionInfo.FolderPath"/> exists; otherwise, <c>false</c>.</value>
        public bool Downloaded => Directory.Exists(FolderPath);
        /// <summary>
        /// URL this revision can be downloaded from.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Whether the revision is locally available on disk.
        /// </summary>
        public bool Local { get; set; }
        /// <summary>
        /// Revision platform.
        /// </summary>
        public Platform Platform { get; set; }
    }
}