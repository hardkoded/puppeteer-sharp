using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp.Cdp.Messaging;

/// <summary>
/// Navigation types.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum NavigationType
{
    /// <summary>
    /// Normal navigation.
    /// </summary>
    Navigation,

    /// <summary>
    /// Back forward cache restore.
    /// </summary>
    BackForwardCacheRestore,
}
