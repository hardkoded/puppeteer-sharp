using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Console message location.
    /// </summary>
    public class ConsoleMessageLocation : IEquatable<ConsoleMessageLocation>
    {
        /// <summary>
        /// URL of the resource if known.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 0-based line number in the resource if known.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// 0-based column number in the resource if known.
        /// </summary>
        public int? ColumnNumber { get; set; }

        /// <summary>Overriding == operator for <see cref="ConsoleMessageLocation"/>.</summary>
        /// <param name="location1">the value to compare against <paramref name="location2" />.</param>
        /// <param name="location2">the value to compare against <paramref name="location1" />.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value.</returns>
        public static bool operator ==(ConsoleMessageLocation location1, ConsoleMessageLocation location2)
            => EqualityComparer<ConsoleMessageLocation>.Default.Equals(location1, location2);

        /// <summary>Overriding != operator for <see cref="ConsoleMessageLocation"/>.</summary>
        /// <param name="location1">the value to compare against <paramref name="location2" />.</param>
        /// <param name="location2">the value to compare against <paramref name="location1" />.</param>
        /// <returns><c>true</c> if the two instances are not equal to the same value.</returns>
        public static bool operator !=(ConsoleMessageLocation location1, ConsoleMessageLocation location2)
            => !(location1 == location2);

        /// <inheritdoc/>
        public bool Equals(ConsoleMessageLocation other)
            => (URL, LineNumber, ColumnNumber) == (other?.URL, other?.LineNumber, other?.ColumnNumber);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as ConsoleMessageLocation);

        /// <inheritdoc/>
        public override int GetHashCode()
            => 412870874 +
                EqualityComparer<string>.Default.GetHashCode(URL) +
                EqualityComparer<int?>.Default.GetHashCode(LineNumber) +
                EqualityComparer<int?>.Default.GetHashCode(ColumnNumber);
    }
}
