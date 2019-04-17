using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    public interface ITarget
    {
        string Url { get; }
        TargetType Type { get; }
        string TargetId { get; }
        ITarget Opener { get; }
        IBrowser Browser { get; }
        IBrowserContext BrowserContext { get; }
        Task<IPage> PageAsync();
        Task<ICDPSession> CreateCDPSessionAsync();
    }
}