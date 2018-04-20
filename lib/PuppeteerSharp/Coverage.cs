using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Coverage
    {
        private readonly JSCoverage _jsCoverage;

        internal Coverage(Session client)
        {
            _jsCoverage = new JSCoverage(client);
        }

        public Task StartJSCoverageAsync(JSCoverageStartOptions options = null)
            => _jsCoverage.StartAsync(options ?? new JSCoverageStartOptions());

        public Task StopJSCoverageAsync() => _jsCoverage.StopAsync();        
    }
}