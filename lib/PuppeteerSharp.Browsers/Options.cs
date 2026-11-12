using CommandLine;

namespace PuppeteerSharp.Browsers
{
    internal abstract class CommonOptions
    {
        [Option("path", HelpText = "Browsers cache directory. Defaults to the PuppeteerSharp browsers location.")]
        public string Path { get; set; }

        [Option("platform", HelpText = "Platform to download for (MacOS, MacOSArm64, Linux, LinuxArm64, Win32, Win64). Defaults to the current platform.")]
        public Platform? Platform { get; set; }

        [Option("base-url", HelpText = "Base URL to download the browser from.")]
        public string BaseUrl { get; set; }
    }

    [Verb("install", HelpText = "Download a browser, e.g. chrome@stable, chrome@116.0.5793.0 or firefox@nightly.")]
    internal sealed class InstallOptions : CommonOptions
    {
        [Value(0, Required = true, MetaName = "browser", HelpText = "browser[@tag|buildId]")]
        public string Browser { get; set; }
    }

    [Verb("list", HelpText = "List the installed browsers.")]
    internal sealed class ListOptions : CommonOptions
    {
    }

    [Verb("uninstall", HelpText = "Remove an installed browser, e.g. chrome@116.0.5793.0.")]
    internal sealed class UninstallOptions : CommonOptions
    {
        [Value(0, Required = true, MetaName = "browser", HelpText = "browser@buildId")]
        public string Browser { get; set; }
    }
}
