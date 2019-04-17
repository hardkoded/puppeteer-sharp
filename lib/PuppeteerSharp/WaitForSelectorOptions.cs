namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="Page.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    /// <seealso cref="Frame.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.WaitForSelectorOptions class instead")]
    public class WaitForSelectorOptions : Abstractions.WaitForSelectorOptions
    {
    }
}