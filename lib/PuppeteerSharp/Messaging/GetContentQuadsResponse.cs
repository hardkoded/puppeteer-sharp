using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class GetContentQuadsResponse
    {
        [JsonProperty(Constants.QUADS)]
        public decimal[][] Quads { get; internal set; }
    }
}