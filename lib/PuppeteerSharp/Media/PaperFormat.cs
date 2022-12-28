using System;
using System.Collections.Generic;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Paper format.
    /// </summary>
    /// <seealso cref="PdfOptions.Format"/>
    public class PaperFormat : IEquatable<PaperFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaperFormat"/> class.
        /// Page width and height in inches.
        /// </summary>
        /// <param name="width">Page width in inches.</param>
        /// <param name="height">Page height in inches.</param>
        public PaperFormat(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Letter: 8.5 inches x 11 inches.
        /// </summary>
        public static PaperFormat Letter => new PaperFormat(8.5m, 11);

        /// <summary>
        /// Legal: 8.5 inches by 14 inches.
        /// </summary>
        public static PaperFormat Legal => new PaperFormat(8.5m, 14);

        /// <summary>
        /// Tabloid: 11 inches by 17 inches.
        /// </summary>
        public static PaperFormat Tabloid => new PaperFormat(11, 17);

        /// <summary>
        /// Ledger: 17 inches by 11 inches.
        /// </summary>
        public static PaperFormat Ledger => new PaperFormat(17, 11);

        /// <summary>
        /// A0: 33.1 inches by 46.8 inches.
        /// </summary>
        public static PaperFormat A0 => new PaperFormat(33.1m, 46.8m);

        /// <summary>
        /// A1: 23.4 inches by 33.1 inches.
        /// </summary>
        public static PaperFormat A1 => new PaperFormat(23.4m, 33.1m);

        /// <summary>
        /// A2: 16.5 inches by 23.4 inches.
        /// </summary>
        public static PaperFormat A2 => new PaperFormat(16.54m, 23.4m);

        /// <summary>
        /// A3: 11.7 inches by 16.5 inches.
        /// </summary>
        public static PaperFormat A3 => new PaperFormat(11.7m, 16.54m);

        /// <summary>
        /// A4: 8.27 inches by 11.7 inches.
        /// </summary>
        public static PaperFormat A4 => new PaperFormat(8.27m, 11.7m);

        /// <summary>
        /// A5: 5.83 inches by 8.27 inches.
        /// </summary>
        public static PaperFormat A5 => new PaperFormat(5.83m, 8.27m);

        /// <summary>
        /// A6: 4.13 inches by 5.83 inches.
        /// </summary>
        public static PaperFormat A6 => new PaperFormat(4.13m, 5.83m);

        /// <summary>
        /// Page width in inches.
        /// </summary>
        /// <value>The width.</value>
        public decimal Width { get; set; }

        /// <summary>
        /// Page height in inches.
        /// </summary>
        /// <value>The Height.</value>
        public decimal Height { get; set; }

        /// <summary>Overriding == operator for <see cref="PaperFormat"/>. </summary>
        /// <param name="left">the value to compare against <paramref name="right" />.</param>
        /// <param name="right">the value to compare against <paramref name="left" />.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value.</returns>
        public static bool operator ==(PaperFormat left, PaperFormat right)
            => EqualityComparer<PaperFormat>.Default.Equals(left, right);

        /// <summary>Overriding != operator for <see cref="PaperFormat"/>. </summary>
        /// <param name="left">the value to compare against <paramref name="right" />.</param>
        /// <param name="right">the value to compare against <paramref name="left" />.</param>
        /// <returns><c>true</c> if the two instances are not equal to the same value.</returns>
        public static bool operator !=(PaperFormat left, PaperFormat right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((PaperFormat)obj);
        }

        /// <inheritdoc/>
        public bool Equals(PaperFormat format)
            => format != null &&
                   Width == format.Width &&
                   Height == format.Height;

        /// <inheritdoc/>
        public override int GetHashCode()
            => 859600377
                ^ Width.GetHashCode()
                ^ Height.GetHashCode();
    }
}
