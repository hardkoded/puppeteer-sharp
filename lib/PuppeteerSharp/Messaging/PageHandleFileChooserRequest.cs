using System.Collections.Generic;
using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PageHandleFileChooserRequest
    {
        public FileChooserAction Action { get; set; }

        public IEnumerable<string> Files { get; set; }
    }
}
