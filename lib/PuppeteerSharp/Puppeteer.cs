using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public static string[] DefaultArgs => Launcher.DefaultArgs;

        public static string GetExecutablePath()
        {
            var downloader = Downloader.CreateDefault();
            var revisionInfo = downloader.RevisionInfo(Downloader.CurrentPlatform, Downloader.DefaultRevision);
            return revisionInfo.ExecutablePath;
        }

        public static async Task<Browser> LaunchAsync(LaunchOptions options, int chromiumRevision)
        {
            return await new Launcher().LaunchAsync(options, chromiumRevision);
        }
    }
}
