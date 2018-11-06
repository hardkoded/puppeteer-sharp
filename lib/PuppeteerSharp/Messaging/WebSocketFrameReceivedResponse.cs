using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketFrameReceivedResponse
    {
        /// <summary>
        /// Request identifier
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// WebSocket response data
        /// </summary>
        [JsonProperty("response")]
        public WebSocketFrame Response { get; set; }
    }
}
