using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Messaging
{
    /// <summary>
    /// Remote object type.
    /// </summary>
    [JsonConverter(typeof(FlexibleStringEnumConverter), Other)]
    public enum RemoteObjectType
    {
        /// <summary>
        /// Other.
        /// </summary>
        Other,
        /// <summary>
        /// Object.
        /// </summary>
        ObjectType,
        /// <summary>
        /// Function.
        /// </summary>
        Function,
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,
        /// <summary>
        /// String.
        /// </summary>
        StringType,
        /// <summary>
        /// Number.
        /// </summary>
        Number,
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean,
        /// <summary>
        /// Symbol.
        /// </summary>
        Symbol,
        /// <summary>
        /// Bigint.
        /// </summary>
        Bigint
    }
}
