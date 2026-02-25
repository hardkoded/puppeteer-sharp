using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.QueryHandlers
{
    internal class CustomQuerySelectorRegistry
    {
        private static readonly string[] _customQuerySeparators = new[] { "=", "/" };

        private readonly object _lock = new();
        private readonly Dictionary<string, (string RegisterScript, QueryHandler Handler)> _queryHandlers = new();

        private readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private readonly QueryHandler _defaultHandler = new CssQueryHandler();

        // The connection is a good place to keep the state of custom queries and injectors.
        // Although I consider that the Browser class would be a better place for this,
        // The connection is being shared between all the components involved in one browser instance
        internal static CustomQuerySelectorRegistry Default { get; } = new();

        internal Dictionary<string, QueryHandler> InternalQueryHandlers => new()
        {
            ["aria"] = new AriaQueryHandler(),
            ["pierce"] = new PierceQueryHandler(),
            ["text"] = new TextQueryHandler(),
            ["xpath"] = new XPathQueryHandler(),
        };

        internal void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
        {
            if (InternalQueryHandlers.ContainsKey(name))
            {
                throw new PuppeteerException($"A query handler named \"{name}\" already exists");
            }

            var isValidName = _customQueryHandlerNameRegex.IsMatch(name);
            if (!isValidName)
            {
                throw new PuppeteerException($"Custom query handler names may only contain [a-zA-Z]");
            }

            if (string.IsNullOrEmpty(queryHandler.QueryAll) && string.IsNullOrEmpty(queryHandler.QueryOne))
            {
                throw new PuppeteerException("At least one query method must be implemented.");
            }

            // Create a QueryHandler that calls into PuppeteerUtil.customQuerySelectors
            var jsonName = JsonSerializer.Serialize(name);
            var internalHandler = new QueryHandler
            {
                QuerySelector = $@"(node, selector, PuppeteerUtil) => {{
                    return PuppeteerUtil.customQuerySelectors
                        .get({jsonName})
                        .querySelector(node, selector);
                }}",
                QuerySelectorAll = $@"(node, selector, PuppeteerUtil) => {{
                    return PuppeteerUtil.customQuerySelectors
                        .get({jsonName})
                        .querySelectorAll(node, selector);
                }}",
            };

            // Generate the registration script that will be injected
            var queryAll = string.IsNullOrEmpty(queryHandler.QueryAll) ? "undefined" : queryHandler.QueryAll;
            var queryOne = string.IsNullOrEmpty(queryHandler.QueryOne) ? "undefined" : queryHandler.QueryOne;

            var registerScript = $@"(PuppeteerUtil) => {{
                PuppeteerUtil.customQuerySelectors.register({jsonName}, {{
                    queryAll: {queryAll},
                    queryOne: {queryOne},
                }});
            }}";

            lock (_lock)
            {
                if (_queryHandlers.ContainsKey(name))
                {
                    throw new PuppeteerException($"A custom query handler named \"{name}\" already exists");
                }

                _queryHandlers.Add(name, (registerScript, internalHandler));
            }

            ScriptInjector.Default.Append(registerScript);
        }

        internal (string UpdatedSelector, QueryHandler QueryHandler, WaitForFunctionPollingOption Polling) GetQueryHandlerAndSelector(string selector)
        {
            // Take a snapshot of custom handlers to avoid holding lock during iteration
            KeyValuePair<string, (string RegisterScript, QueryHandler Handler)>[] customHandlers;
            lock (_lock)
            {
                customHandlers = _queryHandlers.ToArray();
            }

            // Check custom handlers first, then internal handlers (matching upstream order)
            foreach (var kv in customHandlers)
            {
                foreach (var separator in _customQuerySeparators)
                {
                    var prefix = $"{kv.Key}{separator}";

                    if (selector.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        selector = selector.Substring(prefix.Length);
                        return (selector, kv.Value.Handler, WaitForFunctionPollingOption.Mutation);
                    }
                }
            }

            foreach (var kv in InternalQueryHandlers)
            {
                foreach (var separator in _customQuerySeparators)
                {
                    var prefix = $"{kv.Key}{separator}";

                    if (selector.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        selector = selector.Substring(prefix.Length);

                        // Use RAF-based polling for ARIA selectors
                        var polling = kv.Key == "aria"
                            ? WaitForFunctionPollingOption.Raf
                            : WaitForFunctionPollingOption.Mutation;
                        return (selector, kv.Value, polling);
                    }
                }
            }

            try
            {
                var (jsonSelector, isPureCSS, hasPseudoClasses, hasAria) = PSelectorParser.Parse(selector);

                if (isPureCSS)
                {
                    return (selector, _defaultHandler, SelectorHelper.HasPseudoClasses(selector) ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation);
                }

                return (jsonSelector, new PQueryHandler(), hasAria ? WaitForFunctionPollingOption.Raf : WaitForFunctionPollingOption.Mutation);
            }
            catch
            {
                return (selector, _defaultHandler, WaitForFunctionPollingOption.Mutation);
            }
        }

        internal IEnumerable<string> GetCustomQueryHandlerNames()
        {
            lock (_lock)
            {
                return _queryHandlers.Keys.ToArray();
            }
        }

        internal void UnregisterCustomQueryHandler(string name)
        {
            string registerScript = null;
            lock (_lock)
            {
                if (_queryHandlers.TryGetValue(name, out var handler))
                {
                    registerScript = handler.RegisterScript;
                    _queryHandlers.Remove(name);
                }
            }

            if (registerScript != null)
            {
                ScriptInjector.Default.Pop(registerScript);
            }
        }

        internal void ClearCustomQueryHandlers()
        {
            string[] names;
            lock (_lock)
            {
                names = _queryHandlers.Keys.ToArray();
            }

            foreach (var name in names)
            {
                UnregisterCustomQueryHandler(name);
            }
        }
    }
}
