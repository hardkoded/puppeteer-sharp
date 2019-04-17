namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Options to use when typing
    /// </summary>
    /// <seealso cref="Page.TypeAsync(string, string, TypeOptions)"/>
    /// <seealso cref="ElementHandle.TypeAsync(string, TypeOptions)"/>
    /// <seealso cref="Keyboard.TypeAsync(string, TypeOptions)"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.Input.TypeOptions class instead")]
    public class TypeOptions : Abstractions.Input.TypeOptions
    {
    }
}