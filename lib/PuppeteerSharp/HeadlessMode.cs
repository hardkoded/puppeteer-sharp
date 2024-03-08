namespace PuppeteerSharp;

/// <summary>
/// Headless mode.
/// </summary>
public enum HeadlessMode
{
    /// <summary>
    /// Launches the browser in the new headless mode <see href="https://developer.chrome.com/articles/new-headless/"/>.
    /// </summary>
    True,

    /// <summary>
    /// Run browser in non-headless mode.
    /// </summary>
    False,

    /// <summary>
    /// Launches the browser in the old headless mode <see href="https://developer.chrome.com/blog/chrome-headless-shell"/>.
    /// </summary>
    Shell,
}
