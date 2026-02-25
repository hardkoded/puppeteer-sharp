using System.Collections.Generic;

namespace PuppeteerSharp.Cdp.Messaging
{
    internal class RuntimeGetPropertiesResponse
    {
        public IEnumerable<RuntimeGetPropertiesResponseItem> Result { get; set; }

        public class RuntimeGetPropertiesResponseItem
        {
            public bool Enumerable { get; set; }

            public string Name { get; set; }

            public RemoteObject Value { get; set; }
        }
    }
}
