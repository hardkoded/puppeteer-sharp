using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
