﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp
{
    [JsonConverter(typeof(FlexibleStringEnumConverter), Other)]
    internal enum DOMWorldType
    {
        Other,
        [EnumMember(Value = "isolated")]
        Isolated
    }
}