using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface ILauncher
    {
        ChromiumProcess Process { get; set; }
        Task<IBrowser> LaunchAsync(LaunchOptions options);
        Task<IBrowser> ConnectAsync(ConnectOptions options);
        string GetExecutablePath();
    }
}
