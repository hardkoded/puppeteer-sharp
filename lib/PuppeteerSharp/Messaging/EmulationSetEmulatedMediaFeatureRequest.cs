using System.Collections.Generic;
using Newtonsoft.Json;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
