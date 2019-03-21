using System;
using System.Collections.Generic;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// margin options used in <see cref="PdfOptions"/>
    /// </summary>
    public class MarginOptions : IEquatable<MarginOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Media.MarginOptions"/> class.
        /// </summary>
        public MarginOptions() { }

        /// <summary>
        /// Top margin, accepts values labeled with units
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// Left margin, accepts values labeled with units
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Bottom margin, accepts values labeled with units
        /// </summary>
        public string Bottom { get; set; }

        /// <summary>
        /// Right margin, accepts values labeled with units
        /// </summary>
        public string Right { get; set; }

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
        {
            var hashCode = -481391125;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Top);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Bottom);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Right);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(MarginOptions left, MarginOptions right)
            => EqualityComparer<MarginOptions>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(MarginOptions left, MarginOptions right) => !(left == right);
    }
}