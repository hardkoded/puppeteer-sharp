using System;
using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    /// <summary>
    /// Bounding box data returned by <see cref="IElementHandle.BoundingBoxAsync"/>.
    /// </summary>
    public record BoundingBox(decimal X, decimal Y, decimal Width, decimal Height) : Point(X, Y)
    {
    }
}
