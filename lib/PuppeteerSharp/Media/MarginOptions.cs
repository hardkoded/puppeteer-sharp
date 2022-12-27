using System;
using System.Collections.Generic;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// margin options used in <see cref="PdfOptions"/>.
    /// </summary>
    public class MarginOptions : IEquatable<MarginOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Media.MarginOptions"/> class.
        /// </summary>
        public MarginOptions()
        {
        }

        /// <summary>
        /// Top margin, accepts values labeled with units.
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// Left margin, accepts values labeled with units.
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Bottom margin, accepts values labeled with units.
        /// </summary>
        public string Bottom { get; set; }

        /// <summary>
        /// Right margin, accepts values labeled with units.
        /// </summary>
        public string Right { get; set; }

        /// <summary>Overriding == operator for <see cref="MarginOptions"/>.</summary>
        /// <param name="left">the value to compare against <paramref name="right" />.</param>
        /// <param name="right">the value to compare against <paramref name="left" />.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value.</returns>
        public static bool operator ==(MarginOptions left, MarginOptions right)
            => EqualityComparer<MarginOptions>.Default.Equals(left, right);

        /// <summary>Overriding != operator for <see cref="MarginOptions"/>.</summary>
        /// <param name="left">the value to compare against <paramref name="right" />.</param>
        /// <param name="right">the value to compare against <paramref name="left" />.</param>
        /// <returns><c>true</c> if the two instances are not equal to the same value.</returns>
        public static bool operator !=(MarginOptions left, MarginOptions right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((MarginOptions)obj);
        }

        /// <inheritdoc/>
        public bool Equals(MarginOptions options)
            => options != null &&
                   Top == options.Top &&
                   Left == options.Left &&
                   Bottom == options.Bottom &&
                   Right == options.Right;

        /// <inheritdoc/>
        public override int GetHashCode()
            => -481391125
                ^ EqualityComparer<string>.Default.GetHashCode(Top)
                ^ EqualityComparer<string>.Default.GetHashCode(Left)
                ^ EqualityComparer<string>.Default.GetHashCode(Bottom)
                ^ EqualityComparer<string>.Default.GetHashCode(Right);
    }
}
