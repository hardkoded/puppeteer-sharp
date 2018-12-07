using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target type.
    /// </summary>
    [JsonConverter(typeof(FlexibleStringEnumConverter), Other)]
    public enum TargetType
    {
        /// <summary>
        /// The other.
        /// </summary>
        Other,
        /// <summary>
        /// Target type page.
        /// </summary>
        [EnumMember(Value = "page")]
        Page,
        /// <summary>
        /// Target type service worker.
        /// </summary>
        [EnumMember(Value = "service_worker")]
        ServiceWorker,
        /// <summary>
        /// Target type browser.
        /// </summary>
        [EnumMember(Value = "browser")]
        Browser,
        /// <summary>
        /// Target type background page.
        /// </summary>
        [EnumMember(Value = "background_page")]
        BackgroundPage,
        /// <summary>
        /// Target type worker.
        /// </summary>
        [EnumMember(Value = "worker")]
        Worker,
        /// <summary>
        /// Target type javascript.
        /// </summary>
        [EnumMember(Value = "javascript")]
        Javascript,
        /// <summary>
        /// Target type network
        /// </summary>
        [EnumMember(Value = "network")]
        Network,
        /// <summary>
        /// Target type network
        /// </summary>
        [EnumMember(Value = "deprecation")]
        Deprecation
    }
}