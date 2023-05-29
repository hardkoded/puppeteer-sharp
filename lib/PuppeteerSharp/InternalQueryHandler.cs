using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class InternalQueryHandler
    {
        public Func<IElementHandle, string, IJSHandle, Task<IElementHandle>> QueryOne { get; set; }

        public Func<IElementHandle, string, WaitForSelectorOptions, Task<IElementHandle>> WaitFor { get; set; }

        public Func<IElementHandle, string, IJSHandle, Task<IElementHandle[]>> QueryAll { get; set; }

        public Func<IElementHandle, string, IJSHandle, Task<IJSHandle>> QueryAllArray { get; set; }
    }
}
