using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp;

/// <summary>
/// Emulated bluetooth adapter state.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberConverter<AdapterState>))]
public enum AdapterState
{
    /// <summary>
    /// The Bluetooth adapter is absent.
    /// </summary>
    [EnumMember(Value = "absent")]
    Absent,

    /// <summary>
    /// The Bluetooth adapter is present, but powered off.
    /// </summary>
    [EnumMember(Value = "powered-off")]
    PoweredOff,

    /// <summary>
    /// The Bluetooth adapter is present, and powered on.
    /// </summary>
    [EnumMember(Value = "powered-on")]
    PoweredOn,
}
