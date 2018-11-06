using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    /// <summary>
    /// WebSocket frame data
    /// </summary>
    public class WebSocketFrame
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("opcode")]
        public string Opcode { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("mask")]
        public string Mask { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("payloadData")]
        public string PayloadData { get; set; }
    }
}
