using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace PuppeteerSharp.Browsers
{
    internal static class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<InstallOptions, ListOptions, UninstallOptions>(args);
            return await result.MapResult(
                (InstallOptions o) => RunAsync(() => InstallAsync(o)),
                (ListOptions o) => RunAsync(() => Task.FromResult(List(o))),
                (UninstallOptions o) => RunAsync(() => Task.FromResult(Uninstall(o))),
                _ => Task.FromResult(1)).ConfigureAwait(false);
        }

        private static async Task<int> RunAsync(Func<Task<int>> action)
        {
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is ArgumentException or PuppeteerException)
            {
                await Console.Error.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return 1;
            }
        }

        private static BrowserFetcher CreateFetcher(SupportedBrowser browser, CommonOptions options)
            => new(new BrowserFetcherOptions { Browser = browser, Path = options.Path, Platform = options.Platform, SkipChromeHeadlessShellDownload = true })
            {
                BaseUrl = options.BaseUrl,
            };

        private static async Task<int> InstallAsync(InstallOptions options)
        {
            var spec = BrowserSpec.Parse(options.Browser);
            var fetcher = CreateFetcher(spec.Browser, options);
            var installed = spec.Version == null
                ? await fetcher.DownloadAsync().ConfigureAwait(false)
                : spec.TryGetTag(out var tag)
                    ? await fetcher.DownloadAsync(tag).ConfigureAwait(false)
                    : await fetcher.DownloadAsync(spec.Version).ConfigureAwait(false);

            Console.WriteLine($"{installed.Browser}@{installed.BuildId} {installed.GetExecutablePath()}");
            return 0;
        }

        private static int List(ListOptions options)
        {
            foreach (var installed in new BrowserFetcher(new BrowserFetcherOptions { Path = options.Path }).GetInstalledBrowsers().OrderBy(b => b.Browser).ThenBy(b => b.BuildId))
            {
                Console.WriteLine($"{installed.Browser}@{installed.BuildId} {installed.GetExecutablePath()}");
            }

            return 0;
        }

        private static int Uninstall(UninstallOptions options)
        {
            var spec = BrowserSpec.Parse(options.Browser);
            if (spec.Version == null)
            {
                throw new ArgumentException("A build id is required, e.g. chrome@116.0.5793.0.");
            }

            CreateFetcher(spec.Browser, options).Uninstall(spec.Version);
            return 0;
        }
    }
}
