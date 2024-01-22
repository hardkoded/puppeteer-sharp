using System.Runtime.Serialization;

namespace PuppeteerSharp.Messaging.Protocol.Network;

internal enum InitiatorType
{
    Parser,
    Script,
    Preload,
    [EnumMember(Value = "SignedExchange")]
    SignedExchange,
    Preflight,
    Other,
}
