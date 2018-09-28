using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class IOReadResponse
    {
        [JsonProperty(Constants.EOF)]
        internal bool Eof { get; set; }
        [JsonProperty(Constants.DATA)]
        internal string Data { get; set; }
    }
}
