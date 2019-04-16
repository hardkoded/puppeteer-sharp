using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IBrowser
    {
        string WebSocketEndpoint { get; }
        Process Process { get; }
        bool IgnoreHTTPSErrors { get; set; }
        bool IsClosed { get; }
        IBrowserContext DefaultContext { get; }
        int DefaultWaitForTimeout { get; set; }
        Task<IPage> NewPageAsync();
        ITarget[] Targets();
        ITarget Target { get; }
        Task<IBrowserContext> CreateIncognitoBrowserContextAsync();
        IBrowserContext[] BrowserContexts();
        Task<IPage[]> PagesAsync();
        Task<string> GetVersionAsync();
        Task<string> GetUserAgentAsync();
        void Disconnect();
        Task CloseAsync();
        Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null);
        void Dispose();
    }
}
