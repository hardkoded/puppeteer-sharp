namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="Page.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
    /// <seealso cref="Frame.WaitForFunctionAsync(string, WaitForFunctionOptions, object[])"/>
    /// <seealso cref="WaitForSelectorOptions"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.WaitForFunctionOptions class instead")]
    public class WaitForFunctionOptions : Abstractions.WaitForFunctionOptions
    {
    }
}