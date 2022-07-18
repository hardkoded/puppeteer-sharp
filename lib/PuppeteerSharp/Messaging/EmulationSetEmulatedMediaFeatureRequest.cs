using System.Collections.Generic;
using CefSharp.DevTools.Dom.Media;
using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
