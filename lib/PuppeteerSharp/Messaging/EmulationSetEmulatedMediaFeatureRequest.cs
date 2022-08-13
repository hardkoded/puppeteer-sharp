using System.Collections.Generic;
using CefSharp.Dom.Media;
using Newtonsoft.Json;

namespace CefSharp.Dom.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
