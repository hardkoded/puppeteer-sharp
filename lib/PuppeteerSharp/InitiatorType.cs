using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp;

/// <summary>
/// Type of the <see cref="Initiator"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter<InitiatorType>))]
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
