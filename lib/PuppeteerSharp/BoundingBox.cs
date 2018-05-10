using System;
namespace PuppeteerSharp
{
    public class BoundingBox
    {
        /// <summary>
        /// The x coordinate of the element in pixels.
        /// </summary>
        /// <value>The x.</value>
        public decimal X { get; }
        /// <summary>
        /// The y coordinate of the element in pixels.
        /// </summary>
        /// <value>The y.</value>
        public decimal Y { get; }
        /// <summary>
        /// The width of the element in pixels.
        /// </summary>
        /// <value>The width.</value>
        public decimal Width { get; }
        /// <summary>
        /// The height of the element in pixels.
        /// </summary>
        /// <value>The height.</value>
        public decimal Height { get; }

        public BoundingBox(decimal x, decimal y, decimal width, decimal height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj == null && GetType() != obj.GetType())
            {
                return false;
            }

            var boundingBox = obj as BoundingBox;

            return boundingBox.X == X &&
               boundingBox.Y == Y &&
               boundingBox.Height == Height &&
               boundingBox.Width == Width;
        }

        public override int GetHashCode()
            => X.GetHashCode() * 397
                ^ Y.GetHashCode() * 397
                ^ Width.GetHashCode() * 397
                ^ Height.GetHashCode() * 397;
    }
}
