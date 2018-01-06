using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class EmulationManager
    {
        private Session client;

        public EmulationManager(Session client)
        {
            this.client = client;
        }

        internal async Task<bool> EmulateViewport(Session client, ViewPortOptions viewport)
        {
            throw new NotImplementedException();
        }
    }
}