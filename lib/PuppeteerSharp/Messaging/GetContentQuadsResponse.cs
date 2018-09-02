using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class GetContentQuadsResponse
    {
        public GetContentQuadsResponse()
        {
        }

        [JsonProperty("quads")]
        public decimal[][] Quads { get; internal set; }
    }
}