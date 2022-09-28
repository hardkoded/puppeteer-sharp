using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class AccessibilityQueryAXTreeRequest
    {
        public string ObjectId { get; set; }

        public string AccessibleName { get; set; }

        public string Role { get; set; }
    }
}
