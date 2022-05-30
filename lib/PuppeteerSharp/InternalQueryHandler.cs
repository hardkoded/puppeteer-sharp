using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class InternalQueryHandler
    {
        public Func<ElementHandle, string, Task<ElementHandle>> QueryOne { get; set; }

        public Func<DOMWorld, string, WaitForSelectorOptions, Task<ElementHandle>> WaitFor { get; set; }

        public Func<ElementHandle, string, Task<ElementHandle[]>> QueryAll { get; set; }

        public Func<ElementHandle, string, Task<JSHandle>> QueryAllArray { get; set; }
    }
}