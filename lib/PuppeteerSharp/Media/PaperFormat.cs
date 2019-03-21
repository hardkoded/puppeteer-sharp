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
        private PaperFormat() { }
        private PaperFormat(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Page width in inches
        /// </summary>
        /// <value>The width.</value>
        public decimal Width { get; set; }

        /// <summary>
        /// Page height in inches
        /// </summary>
        /// <value>The Height.</value>
        public decimal Height { get; set; }

        /// <summary>
        /// Letter.
        /// </summary>
        public static readonly PaperFormat Letter = new PaperFormat(8.5m, 11);
        /// <summary>
        /// Legal.
        /// </summary>
        public static readonly PaperFormat Legal = new PaperFormat(8.5m, 14);
        /// <summary>
        /// Tabloid.
        /// </summary>
        public static readonly PaperFormat Tabloid = new PaperFormat(11, 17);
        /// <summary>
        /// Ledger.
        /// </summary>
        public static readonly PaperFormat Ledger = new PaperFormat(17, 11);
        /// <summary>
        /// A0.
        /// </summary>
        public static readonly PaperFormat A0 = new PaperFormat(33.1m, 46.8m);
        /// <summary>
        /// A1.
        /// </summary>
        public static readonly PaperFormat A1 = new PaperFormat(23.4m, 33.1m);
        /// <summary>
        /// A2.
        /// </summary>
        public static readonly PaperFormat A2 = new PaperFormat(16.5m, 23.4m);
        /// <summary>
        /// A3.
        /// </summary>
        public static readonly PaperFormat A3 = new PaperFormat(11.7m, 16.5m);
        /// <summary>
        /// A4.
        /// </summary>
        public static readonly PaperFormat A4 = new PaperFormat(8.27m, 11.7m);
        /// <summary>
        /// A5.
        /// </summary>
        public static readonly PaperFormat A5 = new PaperFormat(5.83m, 8.27m);
        /// <summary>
        /// A6.
        /// </summary>
        public static readonly PaperFormat A6 = new PaperFormat(4.13m, 5.83m);

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
        {
            var hashCode = 859600377;
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(PaperFormat left, PaperFormat right)
            => EqualityComparer<PaperFormat>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(PaperFormat left, PaperFormat right) => !(left == right);
    }
}