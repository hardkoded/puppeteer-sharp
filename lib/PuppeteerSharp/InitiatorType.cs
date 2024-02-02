using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PuppeteerSharp;

/// <summary>
/// Type of the <see cref="Initiator"/>.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum InitiatorType
{
    /// <summary>
    /// Parser.
    /// </summary>
    Parser,

    /// <summary>
    /// Script.
    /// </summary>
    Script,

    /// <summary>
    /// Preload.
    /// </summary>
    Preload,

    /// <summary>
    /// SignedExchange.
    /// </summary>
    [EnumMember(Value = "SignedExchange")]
    SignedExchange,

    /// <summary>
    /// Preflight.
    /// </summary>
    Preflight,

    /// <summary>
    /// Other.
    /// </summary>
    Other,
}
