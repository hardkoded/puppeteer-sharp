using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PuppeteerSharp.BrowserData
{
    /// <summary>
    /// Chrome info.
    /// </summary>
    public static class Firefox
    {
        /// <summary>
        /// Default firefox build.
        /// </summary>
        public const string DefaultBuildId = "FIREFOX_NIGHTLY";

        private static readonly Dictionary<string, string> _cachedBuildIds = [];

        internal static Task<string> GetDefaultBuildIdAsync() => ResolveBuildIdAsync(DefaultBuildId);

        internal static string ResolveDownloadUrl(Platform platform, string buildId, string baseUrl)
            =>
                $"{baseUrl ?? "https://archive.mozilla.org/pub/firefox/nightly/latest-mozilla-central"}/{string.Join("/", ResolveDownloadPath(platform, buildId))}";

        internal static async Task<string> ResolveBuildIdAsync(string channel)
        {
            if (_cachedBuildIds.TryGetValue(channel, out var build))
            {
                return build;
            }

            var version = await JsonUtils
                .GetAsync<Dictionary<string, string>>("https://product-details.mozilla.org/1.0/firefox_versions.json")
                .ConfigureAwait(false);

            if (!version.TryGetValue(channel, out var buildId))
            {
                throw new PuppeteerException($"Channel {channel} not found.");
            }

            _cachedBuildIds[channel] = buildId;
            return buildId;
        }

        internal static string RelativeExecutablePath(Platform platform, string buildId)
            => platform switch
            {
                Platform.MacOS or Platform.MacOSArm64 => Path.Combine(
                    "Firefox Nightly.app",
                    "Contents",
                    "MacOS",
                    "firefox"),
                Platform.Linux => Path.Combine("firefox", "firefox"),
                Platform.Win32 or Platform.Win64 => Path.Combine("firefox", "firefox.exe"),
                _ => throw new ArgumentException("Invalid platform", nameof(platform)),
            };

        internal static void CreateProfile(string tempUserDataDirectory, Dictionary<string, object> preferences)
        {
            var defaultPreferences = GetDefaultPreferences(preferences);

            File.WriteAllText(
                Path.Combine(tempUserDataDirectory, "user.js"),
                string.Join(
                    "\n",
                    defaultPreferences.Select(i =>
                            $"user_pref({JsonConvert.SerializeObject(i.Key)}, {JsonConvert.SerializeObject(i.Value)});")
                        .ToArray()));

            File.WriteAllText(Path.Combine(tempUserDataDirectory, "prefs.js"), string.Empty);
        }

        private static string[] ResolveDownloadPath(Platform platform, string buildId)
            => new string[] { GetArchive(platform, buildId), };

        private static string GetArchive(Platform platform, string buildId)
            => platform switch
            {
                Platform.Linux => $"firefox-{buildId}.en-US.{platform.ToString().ToLowerInvariant()}-x86_64.tar.bz2",
                Platform.MacOS or Platform.MacOSArm64 => $"firefox-{buildId}.en-US.mac.dmg",

                // Windows archive name changed at r591479.
                Platform.Win32 or Platform.Win64 =>
                    $"firefox-{buildId}.en-US.{platform.ToString().ToLowerInvariant()}.zip",
                _ => throw new PuppeteerException($"Unknown platform: {platform}"),
            };

        private static Dictionary<string, object> GetDefaultPreferences(Dictionary<string, object> preferences)
        {
            const string server = "dummy.test";
            var prefs = new Dictionary<string, object>()
            {
                // Make sure Shield doesn't hit the network.
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

                // Disable the GFX sanity window
                ["media.sanity-test.disabled"] = true,

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

            if (preferences != null)
            {
                foreach (var kv in preferences)
                {
                    prefs[kv.Key] = kv.Value;
                }
            }

            return prefs;
        }
    }
}
