namespace PuppeteerSharp
{
    /// <summary>
    /// View port options used on <see cref="Page.SetViewportAsync(ViewPortOptions)"/>.
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.ViewPortOptions class instead")]
    public class ViewPortOptions : Abstractions.ViewPortOptions
    {
        internal static ViewPortOptions From(Abstractions.ViewPortOptions options) => new ViewPortOptions
        {
            DeviceScaleFactor = options.DeviceScaleFactor,
            HasTouch = options.HasTouch,
            Height = options.Height,
            IsLandscape = options.IsLandscape,
            IsMobile = options.IsMobile,
            Width = options.Width
        };
    }
}