using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public static string[] DefaultArgs => Launcher.DefaultArgs;

        public static string GetExecutablePath() => new Launcher().GetExecutablePath();

        public static async Task<Browser> LaunchAsync(LaunchOptions options, int chromiumRevision)
        {
            return await new Launcher().LaunchAsync(options, chromiumRevision);
        }
    }
}
