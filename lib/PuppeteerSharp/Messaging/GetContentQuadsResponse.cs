using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetContentQuadsResponse
    {
        [JsonProperty("quads")]
        public decimal[][] Quads { get; internal set; }
    }
}