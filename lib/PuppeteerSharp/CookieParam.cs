namespace PuppeteerSharp
{
    /// <summary>
    /// Cookie data.
    /// </summary>
    /// <seealso cref="Page.SetContentAsync(string, NavigationOptions)"/>
    /// <seealso cref="Page.DeleteCookieAsync(CookieParam[])"/>
    /// <seealso cref="Page.GetCookiesAsync(string[])"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.CookieParam class instead")]
    public class CookieParam : Abstractions.CookieParam
    {
    }
}