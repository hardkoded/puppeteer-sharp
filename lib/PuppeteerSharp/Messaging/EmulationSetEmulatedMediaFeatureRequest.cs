using System.Collections.Generic;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Messaging
{
    internal class EmulationSetEmulatedMediaFeatureRequest
    {
        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
