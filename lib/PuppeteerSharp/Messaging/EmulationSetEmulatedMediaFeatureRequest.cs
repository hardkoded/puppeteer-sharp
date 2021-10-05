using System.Collections.Generic;
using CefSharp.Puppeteer.Media;
using Newtonsoft.Json;

namespace CefSharp.Puppeteer.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
