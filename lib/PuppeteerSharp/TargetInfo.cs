using System;
namespace PuppeteerSharp
{
    /// <summary>
    /// Target info.
    /// </summary>
    public class TargetInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInfo"/> class.
        /// </summary>
        public TargetInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInfo"/> class.
        /// </summary>
        /// <param name="targetInfo">Target info.</param>
        public TargetInfo(dynamic targetInfo)
        {
            Type = targetInfo.type;
            Url = targetInfo.url;
            TargetId = targetInfo.targetId;
            SourceObject = targetInfo;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; internal set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; internal set; }
        /// <summary>
        /// Gets or sets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId { get; internal set; }
        /// <summary>
        /// Gets the source object.
        /// </summary>
        /// <value>The source object.</value>
        public dynamic SourceObject { get; }
    }
}
