﻿using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp
{
    public struct ResponseData
    {
        public string Body { get; internal set; }
        public Dictionary<string, object> Headers { get; internal set; }
        public string ContentType { get; internal set; }
        public HttpStatusCode? Status { get; internal set; }
    }
}