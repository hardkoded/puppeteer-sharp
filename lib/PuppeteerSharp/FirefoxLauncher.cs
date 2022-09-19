using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a Firefox process and any associated temporary user data directory that have created
    /// by Puppeteer and therefore must be cleaned up when no longer needed.
    /// </summary>
    public class FirefoxLauncher : LauncherBase
    {
        internal static readonly string[] _defaultArgs =
        {
          "--no-remote",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxLauncher"/> class.
        /// </summary>
        /// <param name="executable">Full path of executable.</param>
        /// <param name="options">Options for launching Firefox.</param>
        public FirefoxLauncher(string executable, LaunchOptions options)
            : base(executable, options)
        {
            List<string> firefoxArgs;
            (firefoxArgs, TempUserDataDir) = PrepareFirefoxArgs(options);

            Process.StartInfo.Arguments = string.Join(" ", firefoxArgs);
        }

        /// <inheritdoc />
        public override string ToString() => $"Firefox process; EndPoint={EndPoint}; State={CurrentState}";

        private static (List<string> FirefoxArgs, TempDirectory TempUserDataDirectory) PrepareFirefoxArgs(LaunchOptions options)
        {
            var firefoxArgs = new List<string>();

            if (!options.IgnoreDefaultArgs)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options));
            }
            else if (options.IgnoredDefaultArgs?.Length > 0)
            {
                firefoxArgs.AddRange(GetDefaultArgs(options).Except(options.IgnoredDefaultArgs));
            }
            else
            {
                firefoxArgs.AddRange(options.Args);
            }

            if (!firefoxArgs.Any(a => a.StartsWith("-remote-debugging", StringComparison.OrdinalIgnoreCase)))
            {
                firefoxArgs.Add("--remote-debugging-port=0");
            }

            TempDirectory tempUserDataDirectory = null;

            if (!firefoxArgs.Contains("-profile") && !firefoxArgs.Contains("--profile"))
            {
                tempUserDataDirectory = new TempDirectory();
                CreateProfile(tempUserDataDirectory);
                firefoxArgs.Add("--profile");
                firefoxArgs.Add($"{tempUserDataDirectory.Path.Quote()}");
            }

            return (firefoxArgs, tempUserDataDirectory);
        }

        private static void CreateProfile(TempDirectory tempUserDataDirectory)
        {
            var userJS = new List<string>();
            const string server = "dummy.test";
            var defaultPreferences = new Dictionary<string, object>
            {
                // Make sure Shield doesn"t hit the network.
                ["app.normandy.api_url"] = string.Empty,

                // Disable Firefox old build background check
                ["app.update.checkInstallTime"] = false,

                // Disable automatically upgrading Firefox
                ["app.update.disabledForTesting"] = true,

                // Increase the APZ content response timeout to 1 minute
                ["apz.content_response_timeout"] = 60000,

                // Prevent various error message on the console
                // jest-puppeteer asserts that no error message is emitted by the console
                ["browser.contentblocking.features.standard"] = "-tp,tpPrivate,cookieBehavior0,-cm,-fp",

                // Enable the dump function: which sends messages to the system
                // console
                // https://bugzilla.mozilla.org/show_bug.cgi?id=1543115
                ["browser.dom.window.dump.enabled"] = true,

                // Disable topstories
                ["browser.newtabpage.activity-stream.feeds.system.topstories"] = false,

                // Always display a blank page
                ["browser.newtabpage.enabled"] = false,

                // Background thumbnails in particular cause grief: and disabling
                // thumbnails in general cannot hurt
                ["browser.pagethumbnails.capturing_disabled"] = true,

                // Disable safebrowsing components.
                ["browser.safebrowsing.blockedURIs.enabled"] = false,
                ["browser.safebrowsing.downloads.enabled"] = false,
                ["browser.safebrowsing.malware.enabled"] = false,
                ["browser.safebrowsing.passwords.enabled"] = false,
                ["browser.safebrowsing.phishing.enabled"] = false,

                // Disable updates to search engines.
                ["browser.search.update"] = false,

                // Do not restore the last open set of tabs if the browser has crashed
                ["browser.sessionstore.resume_from_crash"] = false,

                // Skip check for default browser on startup
                ["browser.shell.checkDefaultBrowser"] = false,

                // Disable newtabpage
                ["browser.startup.homepage"] = "about:blank",

                // Do not redirect user when a milstone upgrade of Firefox is detected
                ["browser.startup.homepage_override.mstone"] = "ignore",

                // Start with a blank page about:blank
                ["browser.startup.page"] = 0,

                // Do not allow background tabs to be zombified on Android: otherwise for
                // tests that open additional tabs: the test harness tab itself might get
                // unloaded
                ["browser.tabs.disableBackgroundZombification"] = false,

                // Do not warn when closing all other open tabs
                ["browser.tabs.warnOnCloseOtherTabs"] = false,

                // Do not warn when multiple tabs will be opened
                ["browser.tabs.warnOnOpen"] = false,

                // Disable the UI tour.
                ["browser.uitour.enabled"] = false,

                // Turn off search suggestions in the location bar so as not to trigger
                // network connections.
                ["browser.urlbar.suggest.searches"] = false,

                // Disable first run splash page on Windows 10
                ["browser.usedOnWindows10.introURL"] = string.Empty,

                // Do not warn on quitting Firefox
                ["browser.warnOnQuit"] = false,

                // Defensively disable data reporting systems
                ["datareporting.healthreport.documentServerURI"] = $"http://{server}/dummy/healthreport/",
                ["datareporting.healthreport.logging.consoleEnabled"] = false,
                ["datareporting.healthreport.service.enabled"] = false,
                ["datareporting.healthreport.service.firstRun"] = false,
                ["datareporting.healthreport.uploadEnabled"] = false,

                // Do not show datareporting policy notifications which can interfere with tests
                ["datareporting.policy.dataSubmissionEnabled"] = false,
                ["datareporting.policy.dataSubmissionPolicyBypassNotification"] = true,

                // DevTools JSONViewer sometimes fails to load dependencies with its require.js.
                // This doesn"t affect Puppeteer but spams console (Bug 1424372)
                ["devtools.jsonview.enabled"] = false,

                // Disable popup-blocker
                ["dom.disable_open_during_load"] = false,

                // Enable the support for File object creation in the content process
                // Required for |Page.setFileInputFiles| protocol method.
                ["dom.file.createInChild"] = true,

                // Disable the ProcessHangMonitor
                ["dom.ipc.reportProcessHangs"] = false,

                // Disable slow script dialogues
                ["dom.max_chrome_script_run_time"] = 0,
                ["dom.max_script_run_time"] = 0,

                // Only load extensions from the application and user profile
                // AddonManager.SCOPE_PROFILE + AddonManager.SCOPE_APPLICATION
                ["extensions.autoDisableScopes"] = 0,
                ["extensions.enabledScopes"] = 5,

                // Disable metadata caching for installed add-ons by default
                ["extensions.getAddons.cache.enabled"] = false,

                // Disable installing any distribution extensions or add-ons.
                ["extensions.installDistroAddons"] = false,

                // Disabled screenshots extension
                ["extensions.screenshots.disabled"] = true,

                // Turn off extension updates so they do not bother tests
                ["extensions.update.enabled"] = false,

                // Turn off extension updates so they do not bother tests
                ["extensions.update.notifyUser"] = false,

                // Make sure opening about:addons will not hit the network
                ["extensions.webservice.discoverURL"] = $"http://{server}/dummy/discoveryURL",

                // Temporarily force disable BFCache in parent (https://bit.ly/bug-1732263)
                ["fission.bfcacheInParent"] = false,

                // Force all web content to use a single content process
                ["fission.webContentIsolationStrategy"] = 0,

                // Allow the application to have focus even it runs in the background
                ["focusmanager.testmode"] = true,

                // Disable useragent updates
                ["general.useragent.updates.enabled"] = false,

                // Always use network provider for geolocation tests so we bypass the
                // macOS dialog raised by the corelocation provider
                ["geo.provider.testing"] = true,

                // Do not scan Wifi
                ["geo.wifi.scan"] = false,

                // No hang monitor
                ["hangmonitor.timeout"] = 0,

                // Show chrome errors and warnings in the error console
                ["javascript.options.showInConsole"] = true,

                // Disable download and usage of OpenH264: and Widevine plugins
                ["media.gmp-manager.updateEnabled"] = false,

                // Prevent various error message on the console
                // jest-puppeteer asserts that no error message is emitted by the console
                ["network.cookie.cookieBehavior"] = 0,

                // Disable experimental feature that is only available in Nightly
                ["network.cookie.sameSite.laxByDefault"] = false,

                // Do not prompt for temporary redirects
                ["network.http.prompt-temp-redirect"] = false,

                // Disable speculative connections so they are not reported as leaking
                // when they are hanging around
                ["network.http.speculative-parallel-limit"] = 0,

                // Do not automatically switch between offline and online
                ["network.manage-offline-status"] = false,

                // Make sure SNTP requests do not hit the network
                ["network.sntp.pools"] = server,

                // Disable Flash.
                ["plugin.state.flash"] = 0,

                ["privacy.trackingprotection.enabled"] = false,

                // Enable Remote Agent
                // https://bugzilla.mozilla.org/show_bug.cgi?id=1544393
                ["remote.enabled"] = true,

                // Don"t do network connections for mitm priming
                ["security.certerrors.mitm.priming.enabled"] = false,

                // Local documents have access to all other local documents,
                // including directory listings
                ["security.fileuri.strict_origin_policy"] = false,

                // Do not wait for the notification button security delay
                ["security.notification_enable_delay"] = 0,

                // Ensure blocklist updates do not hit the network
                ["services.settings.server"] = $"http://{server}/dummy/blocklist/",

                // Do not automatically fill sign-in forms with known usernames and
                // passwords
                ["signon.autofillForms"] = false,

                // Disable password capture, so that tests that include forms are not
                // influenced by the presence of the persistent doorhanger notification
                ["signon.rememberSignons"] = false,

                // Disable first-run welcome page
                ["startup.homepage_welcome_url"] = "about:blank",

                // Disable first-run welcome page
                ["startup.homepage_welcome_url.additional"] = string.Empty,

                // Disable browser animations (tabs, fullscreen, sliding alerts)
                ["toolkit.cosmeticAnimations.enabled"] = false,

                // Prevent starting into safe mode after application crashes
                ["toolkit.startup.max_resumed_crashes"] = -1,
            };

            File.WriteAllText(
                Path.Combine(tempUserDataDirectory.Path, "user.js"),
                string.Join("\n", defaultPreferences.Select(i => $"user_pref({JsonConvert.SerializeObject(i.Key)}, {JsonConvert.SerializeObject(i.Value)});").ToArray()));

            File.WriteAllText(Path.Combine(tempUserDataDirectory.Path, "prefs.js"), string.Empty);
        }

        internal static string[] GetDefaultArgs(LaunchOptions options)
        {
            var firefoxArguments = new List<string>(_defaultArgs);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                firefoxArguments.Add("--foreground");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                firefoxArguments.Add("--wait-for-browser");
            }

            if (!string.IsNullOrEmpty(options.UserDataDir))
            {
                firefoxArguments.Add("--profile");
                firefoxArguments.Add($"{options.UserDataDir.Quote()}");
            }

            if (options.Headless)
            {
                firefoxArguments.Add("--headless");
            }

            if (options.Devtools)
            {
                firefoxArguments.Add("--devtools");
            }

            if (options.Args.All(arg => arg.StartsWith("-", StringComparison.Ordinal)))
            {
                firefoxArguments.Add("about:blank");
            }

            firefoxArguments.AddRange(options.Args);
            return firefoxArguments.ToArray();
        }
    }
}
