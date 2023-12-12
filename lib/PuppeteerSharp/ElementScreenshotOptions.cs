namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="IElementHandle.ScreenshotAsync(string, ElementScreenshotOptions)"/>, <see cref="IElementHandle.ScreenshotStreamAsync(ElementScreenshotOptions)"/> and <see cref="IElementHandle.ScreenshotDataAsync(ElementScreenshotOptions)"/>.
    /// </summary>
    public class ElementScreenshotOptions : ScreenshotOptions
    {
        /// <summary>
        /// When <c>true</c>, it will scroll into view before taking the screenshot. Defaults to <c>true</c>.
        /// </summary>
        public bool ScrollIntoView { get; set; } = true;
    }
}
