using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Tracing
    {
        private Session client;

        public Tracing(Session client)
        {
            this.client = client;
        }

        /// <summary>
        /// Starts tracing.
        /// </summary>
        /// <returns>Start task</returns>
        /// <param name="options">Tracing options</param>
        public async Task StartAsync(TracingOptions options)
        {

        }

        /// <summary>
        /// Stops tracing
        /// </summary>
        /// <returns>Stop task</returns>
        public async Task StopAsync()
        {

        }
    }
}