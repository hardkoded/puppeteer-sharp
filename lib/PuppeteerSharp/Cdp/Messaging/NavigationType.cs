using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp.Messaging;

/// <summary>
/// Navigation types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter<NavigationType>))]
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
