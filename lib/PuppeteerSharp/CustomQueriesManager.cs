using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PuppeteerSharp
{
    internal static class CustomQueriesManager
    {
        private static readonly Dictionary<string, InternalQueryHandler> _queryHandlers = new();
        private static readonly Dictionary<string, InternalQueryHandler> _builtInHandlers = new();
        private static readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private static readonly Regex _customQueryHandlerParserRegex = new("(?<query>^[a-zA-Z]+)\\/(?<selector>.*)", RegexOptions.Compiled);
        private static readonly InternalQueryHandler _defaultHandler = MakeQueryHandler(new CustomQueryHandler
        {
            QueryOne = "(element, selector) => element.querySelector(selector)",
            QueryAll = "(element, selector) => element.querySelectorAll(selector)",
        });

        internal static void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
        {
            if (_queryHandlers.ContainsKey(name))
            {
                throw new PuppeteerException($"A custom query handler named \"{name}\" already exists");
            }

            var isValidName = _customQueryHandlerNameRegex.IsMatch(name);
            if (!isValidName)
            {
                throw new PuppeteerException($"Custom query handler names may only contain [a-zA-Z]");
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
                    var jsHandle = await element.EvaluateFunctionHandleAsync(handler.QueryAll, selector).ConfigureAwait(false);
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
                    return await resultHandle.EvaluateFunctionHandleAsync("(res) => Array.from(res)").ConfigureAwait(false);
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

        internal static IEnumerable<string> GetCustomQueryHandlerNames()
            => _queryHandlers.Keys;

        internal static void UnregisterCustomQueryHandler(string name)
            => _queryHandlers.Remove(name);

        internal static void ClearCustomQueryHandlers()
        {
            foreach (var name in CustomQueryHandlerNames())
            {
                UnregisterCustomQueryHandler(name);
            }
        }

        private static IEnumerable<string> CustomQueryHandlerNames()
            => _queryHandlers.Keys.Where(k => !_builtInHandlers.ContainsKey(k));
    }
}

