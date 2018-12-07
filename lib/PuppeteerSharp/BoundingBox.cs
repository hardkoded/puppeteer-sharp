using System;
using PuppeteerSharp.Media;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Bounding box data returned by <see cref="ElementHandle.BoundingBoxAsync"/>.
    /// </summary>
    public class BoundingBox : IEquatable<BoundingBox>
    {
        /// <summary>
        /// The x coordinate of the element in pixels.
        /// </summary>
        /// <value>The x.</value>
        public decimal X { get; set; }
        /// <summary>
        /// The y coordinate of the element in pixels.
        /// </summary>
        /// <value>The y.</value>
        public decimal Y { get; set; }
        /// <summary>
        /// The width of the element in pixels.
        /// </summary>
        /// <value>The width.</value>
        public decimal Width { get; set; }
        /// <summary>
        /// The height of the element in pixels.
        /// </summary>
        /// <value>The height.</value>
        public decimal Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppeteerSharp.BoundingBox"/> class.
        /// </summary>
        public BoundingBox() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public BoundingBox(decimal x, decimal y, decimal width, decimal height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal Clip ToClip()
        {
            return new Clip
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null && GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((BoundingBox)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="PuppeteerSharp.BoundingBox"/> is equal to the current <see cref="T:PuppeteerSharp.BoundingBox"/>.
        /// </summary>
        /// <param name="obj">The <see cref="PuppeteerSharp.BoundingBox"/> to compare with the current <see cref="T:PuppeteerSharp.BoundingBox"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="PuppeteerSharp.BoundingBox"/> is equal to the current
        /// <see cref="T:PuppeteerSharp.BoundingBox"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(BoundingBox obj)
            => obj != null &&
                obj.X == X &&
                obj.Y == Y &&
                obj.Height == Height &&
                obj.Width == Width;

        /// <inheritdoc/>
        public override int GetHashCode()
            => X.GetHashCode() * 397
                ^ Y.GetHashCode() * 397
                ^ Width.GetHashCode() * 397
                ^ Height.GetHashCode() * 397;
    }
}