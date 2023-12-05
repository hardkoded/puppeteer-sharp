namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Clip data.
    /// </summary>
    /// <seealso cref="ScreenshotOptions.Clip"/>
    public record Clip(decimal X, decimal Y, decimal Width, decimal Height, decimal Scale = 1)
        : BoundingBox(X, Y, Width, Height)
    {
    }
}
