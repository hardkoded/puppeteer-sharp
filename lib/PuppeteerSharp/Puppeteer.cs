using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Mobile;

namespace PuppeteerSharp
{
    /// <summary>
    /// Provides a method to launch a Chromium instance.
    /// </summary>
    /// <example>
    /// The following is a typical example of using a Puppeteer to drive automation:
    /// <code>
    /// var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
    /// var page = await browser.NewPageAsync();
    /// await page.GoToAsync("https://www.google.com");
    /// await Browser.CloseAsync();
    /// </code>
    /// </example>
    public static class Puppeteer
    {
        private static readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private static readonly Regex _customQueryHandlerParserRegex = new("(?<query>^[a-zA-Z]+)\\/(?<selector>.*)", RegexOptions.Compiled);
        private static readonly InternalQueryHandler _defaultHandler = MakeQueryHandler(new CustomQueryHandler
        {
            QueryOne = "(element, selector) => element.querySelector(selector)",
            QueryAll = "(element, selector) => element.querySelectorAll(selector)",
        });

        internal const int DefaultTimeout = 30_000;
        private static readonly Dictionary<string, InternalQueryHandler> _queryHandlers = new();

        /// <summary>
        /// The default flags that Chromium will be launched with.
        /// </summary>
        internal static string[] DefaultArgs => ChromiumLauncher.DefaultArgs;

        /// <summary>
        /// Returns a list of devices to be used with <seealso cref="Page.EmulateAsync(DeviceDescriptor)"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateAsync(iPhone);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> Devices => DeviceDescriptors.ToReadOnly();

        /// <summary>
        /// Returns a list of network conditions to be used with <seealso cref="Page.EmulateNetworkConditionsAsync(NetworkConditions)"/>.
        /// Actual list of conditions can be found in <seealso cref="PredefinedNetworkConditions.Conditions"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var slow3G = Puppeteer.NetworkConditions["Slow 3G"];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateNetworkConditionsAsync(slow3G);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<string, NetworkConditions> NetworkConditions => PredefinedNetworkConditions.ToReadOnly();

        /// <summary>
        /// Returns an array of argument based on the options provided and the platform where the library is running
        /// </summary>
        /// <returns>Chromium arguments.</returns>
        /// <param name="options">Options.</param>
        public static string[] GetDefaultArgs(LaunchOptions options = null)
            => (options?.Product ?? Product.Chrome) == Product.Chrome
                ? ChromiumLauncher.GetDefaultArgs(options ?? new LaunchOptions())
                : FirefoxLauncher.GetDefaultArgs(options ?? new LaunchOptions());

        /// <summary>
        /// The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for launching Chrome</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <returns>A connected browser.</returns>
        /// <remarks>
        /// See <a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/">this article</a>
        /// for a description of the differences between Chromium and Chrome.
        /// <a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md">This article</a> describes some differences for Linux users.
        ///
        /// Environment Variables
        /// Puppeteer looks for certain <see href="https://en.wikipedia.org/wiki/Environment_variable">environment variables</see>() to aid its operations.
        /// - <c>PUPPETEER_CHROMIUM_REVISION</c> - specify a certain version of Chromium you'd like Puppeteer to use. See <see cref="Puppeteer.LaunchAsync(LaunchOptions, ILoggerFactory)"/> on how executable path is inferred.
        ///   **BEWARE**: Puppeteer is only <see href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy">guaranteed to work</see> with the bundled Chromium, use at your own risk.
        /// - <c>PUPPETEER_EXECUTABLE_PATH</c> - specify an executable path to be used in <see cref="Puppeteer.LaunchAsync(LaunchOptions, ILoggerFactory)"/>.
        ///   **BEWARE**: Puppeteer is only <see href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy">guaranteed to work</see> with the bundled Chromium, use at your own risk.
        /// </remarks>
        public static Task<Browser> LaunchAsync(LaunchOptions options, ILoggerFactory loggerFactory = null)
            => new Launcher(loggerFactory).LaunchAsync(options);

        /// <summary>
        /// Attaches Puppeteer to an existing Chromium instance. The browser will be closed when the Browser is disposed.
        /// </summary>
        /// <param name="options">Options for connecting.</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <returns>A connected browser.</returns>
        public static Task<Browser> ConnectAsync(ConnectOptions options, ILoggerFactory loggerFactory = null)
            => new Launcher(loggerFactory).ConnectAsync(options);

        /// <summary>
        /// Creates the browser fetcher.
        /// </summary>
        /// <returns>The browser fetcher.</returns>
        /// <param name="options">Options.</param>
        public static BrowserFetcher CreateBrowserFetcher(BrowserFetcherOptions options) => new(options);

        /// <summary>
        /// Registers a custom query handler.
        /// After registration, the handler can be used everywhere where a selector is
        /// expected by prepending the selection string with `name/`. The name is
        /// only allowed to consist of lower- and upper case latin letters.
        /// </summary>
        /// <example>
        /// Puppeteer.RegisterCustomQueryHandler("text", "{ … }");
        /// var aHandle = await page.QuerySelectorAsync("text/…");
        /// </example>
        /// <param name="name">The name that the custom query handler will be registered under.</param>
        /// <param name="queryHandler">The query handler to register</param>
        internal static void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
        {
            if (_queryHandlers.ContainsKey(name))
            {
                throw new PuppeteerException($"A custom query handler named \"{name}\" already exists");
            }

            var isValidName = _customQueryHandlerNameRegex.IsMatch(name);
            if (!isValidName)
            {
                throw new PuppeteerException($"Custom query handler names may only contain[a-zA-Z]");
            }
            var internalHandler = MakeQueryHandler(queryHandler);

            _queryHandlers.Add(name, internalHandler);
        }

        private static InternalQueryHandler MakeQueryHandler(CustomQueryHandler handler)
        {
            var internalHandler = new InternalQueryHandler();

            if (!string.IsNullOrEmpty(handler.QueryOne))
            {
                internalHandler.QueryOne = async (ElementHandle element, string selector) =>
                {
                    var jsHandle = await element.EvaluateFunctionHandleAsync(handler.QueryOne, selector).ConfigureAwait(false);
                    if (jsHandle is ElementHandle elementHandle)
                    {
                        return elementHandle;
                    }

                    await jsHandle.DisposeAsync().ConfigureAwait(false);
                    return null;
                };

                internalHandler.WaitFor = (DOMWorld domWorld, string selector, WaitForSelectorOptions options)
                    => domWorld.WaitForSelectorInPageAsync(handler.QueryOne, selector, options);
            }

            if (!string.IsNullOrEmpty(handler.QueryAll))
            {
                internalHandler.QueryAll = async (ElementHandle element, string selector) =>
                {
                    var jsHandle = await element.EvaluateFunctionHandleAsync(handler.QueryOne, selector).ConfigureAwait(false);
                    var properties = await jsHandle.GetPropertiesAsync().ConfigureAwait(false);
                    var result = new List<ElementHandle>();

                    foreach (var property in properties.Values)
                    {
                        if (property is ElementHandle elementHandle)
                        {
                            result.Add(elementHandle);
                        }
                    }

                    return result.ToArray();
                };

                internalHandler.QueryAllArray = async (ElementHandle element, string selector) => {
                    var resultHandle = await element.EvaluateFunctionHandleAsync(
                      handler.QueryAll,
                      selector).ConfigureAwait(false);
                    var arrayHandle = await resultHandle.EvaluateFunctionHandleAsync("(res) => Array.from(res)").ConfigureAwait(false);
                    return arrayHandle as ElementHandle;
                };
            }

            return internalHandler;
        }

        internal static (string UpdatedSelector, InternalQueryHandler QueryHandler) GetQueryHandlerAndSelector(string selector)
        {
            var customQueryHandlerMatch = _customQueryHandlerParserRegex.Match(selector);
            if (!customQueryHandlerMatch.Success)
            {
                return (selector, _defaultHandler);
            }

            var name = customQueryHandlerMatch.Groups["query"].Value;
            var updatedSelector = customQueryHandlerMatch.Groups["selector"].Value;

            if (!_queryHandlers.TryGetValue(name, out var queryHandler))
            {
                throw new PuppeteerException($"Query set to use \"{name}\", but no query handler of that name was found");
            }

            return (updatedSelector, queryHandler);
        }
    }
}
