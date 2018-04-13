using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public static string[] DefaultArgs => Launcher.DefaultArgs;

        public static string GetExecutablePath() => Launcher.GetExecutablePath();

        public static async Task<Browser> LaunchAsync(LaunchOptions options, int chromiumRevision)
        {
            return await new Launcher().LaunchAsync(options, chromiumRevision);
        }

        public static async Task<Browser> ConnectAsync(ConnectOptions options)
        {
            return await new Launcher().ConnectAsync(options);
        }
    }
}
