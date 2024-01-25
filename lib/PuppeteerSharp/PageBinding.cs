using System;

namespace PuppeteerSharp
{
    internal class PageBinding
    {
        public string Name { get; set; }

        public Delegate Function { get; set; }
    }
}
