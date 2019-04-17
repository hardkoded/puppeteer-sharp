using System;

namespace PuppeteerSharp
{
    /// <summary>
    /// Optional waiting parameters.
    /// </summary>
    /// <seealso cref="Page.WaitForRequestAsync(Func{Request, bool}, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForRequestAsync(string, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForResponseAsync(string, WaitForOptions)"/>
    /// <seealso cref="Page.WaitForResponseAsync(Func{Response, bool}, WaitForOptions)"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.WaitForOptions class instead")]
    public class WaitForOptions : Abstractions.WaitForOptions
    {
    }
}