using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Console type used on <see cref="ConsoleMessage"/>.
    /// </summary>
    public enum ConsoleType
    {
        /// <summary>
        /// Log.
        /// </summary>
        Log,

        /// <summary>
        /// Debug.
        /// </summary>
        Debug,

        /// <summary>
        /// Info.
        /// </summary>
        Info,

        /// <summary>
        /// Error.
        /// </summary>
        Error,

        /// <summary>
        /// Warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Dir.
        /// </summary>
        Dir,

        /// <summary>
        /// Dirxml.
        /// </summary>
        Dirxml,

        /// <summary>
        /// Table.
        /// </summary>
        Table,

        /// <summary>
        /// Trace.
        /// </summary>
        Trace,

        /// <summary>
        /// Clear.
        /// </summary>
        Clear,

        /// <summary>
        /// StartGroup.
        /// </summary>
        StartGroup,

        /// <summary>
        /// StartGroupCollapsed.
        /// </summary>
        StartGroupCollapsed,

        /// <summary>
        /// EndGroup.
        /// </summary>
        EndGroup,

        /// <summary>
        /// Assert.
        /// </summary>
        Assert,

        /// <summary>
        /// Profile.
        /// </summary>
        Profile,

        /// <summary>
        /// ProfileEnd.
        /// </summary>
        ProfileEnd,

        /// <summary>
        /// Count.
        /// </summary>
        Count,

        /// <summary>
        /// TimeEnd.
        /// </summary>
        TimeEnd,

        /// <summary>
        /// Verbose.
        /// </summary>
        Verbose,

        /// <summary>
        /// Time Stamp.
        /// </summary>
        [EnumMember(Value = "timeStamp")]
        Timestamp,
    }
}
