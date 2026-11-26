using System.Collections.Generic;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EmulationSetEmulatedMediaRequest
    {
        public MediaType Media { get; set; }

        public IEnumerable<MediaFeatureValue> Features { get; set; }
    }
}
