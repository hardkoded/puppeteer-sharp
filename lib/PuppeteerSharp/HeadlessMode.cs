namespace PuppeteerSharp;

/// <summary>
/// Headless mode.
/// </summary>
public enum HeadlessMode
{
    /// <summary>
    /// Launches the browser in the new headless mode <see href="https://developer.chrome.com/docs/automation-and-testing/headless"/>.
    /// </summary>
    True,

    /// <summary>
    /// Run browser in non-headless mode.
    /// </summary>
    False,

    /// <summary>
    /// Launches the browser in the old headless mode <see href="https://developer.chrome.com/docs/automation-and-testing/headless-chrome-shell"/>.
    /// </summary>
    Shell,
}
