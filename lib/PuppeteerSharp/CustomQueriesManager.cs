using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PuppeteerSharp
{
    internal class CustomQueriesManager
    {
        private readonly Dictionary<string, InternalQueryHandler> _queryHandlers = new();
        private readonly InternalQueryHandler _pierceHandler = MakeQueryHandler(new CustomQueryHandler
        {
            QueryOne = @"(element, selector) => {
                let found = null;
                const search = (root) => {
                  const iter = document.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
                  do {
                    const currentNode = iter.currentNode;
                    if (currentNode.shadowRoot) {
                      search(currentNode.shadowRoot);
                    }
                    if (currentNode instanceof ShadowRoot) {
                      continue;
                    }
                    if (currentNode !== root && !found && currentNode.matches(selector)) {
                      found = currentNode;
                    }
                  } while (!found && iter.nextNode());
                };
                if (element instanceof Document) {
                  element = element.documentElement;
                }
                search(element);
                return found;
              }",
            QueryAll = @"(element, selector) => {
                const result = [];
                const collect = (root) => {
                  const iter = document.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
                  do {
                    const currentNode = iter.currentNode;
                    if (currentNode.shadowRoot) {
                      collect(currentNode.shadowRoot);
                    }
                    if (currentNode instanceof ShadowRoot) {
                      continue;
                    }
                    if (currentNode !== root && currentNode.matches(selector)) {
                      result.push(currentNode);
                    }
                  } while (iter.nextNode());
                };
                if (element instanceof Document) {
                  element = element.documentElement;
                }
                collect(element);
                return result;
              }",
        });

        private readonly Dictionary<string, InternalQueryHandler> _builtInHandlers;

        private readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private readonly Regex _customQueryHandlerParserRegex = new("(?<query>^[a-zA-Z]+)\\/(?<selector>.*)", RegexOptions.Compiled);
        private readonly InternalQueryHandler _defaultHandler = MakeQueryHandler(new CustomQueryHandler
        {
            QueryOne = "(element, selector) => element.querySelector(selector)",
            QueryAll = "(element, selector) => element.querySelectorAll(selector)",
        });

        public CustomQueriesManager()
        {
            _builtInHandlers = new()
            {
                ["pierce"] = _pierceHandler,
            };
            _queryHandlers = _builtInHandlers.ToDictionary(
                entry => entry.Key,
                entry => entry.Value);
        }

        internal void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
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

                internalHandler.QueryAllArray = async (ElementHandle element, string selector) =>
                {
                    var resultHandle = await element.EvaluateFunctionHandleAsync(
                      handler.QueryAll,
                      selector).ConfigureAwait(false);
                    return await resultHandle.EvaluateFunctionHandleAsync("(res) => Array.from(res)").ConfigureAwait(false);
                };
            }

            return internalHandler;
        }

        internal (string UpdatedSelector, InternalQueryHandler QueryHandler) GetQueryHandlerAndSelector(string selector)
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

        internal IEnumerable<string> GetCustomQueryHandlerNames()
            => _queryHandlers.Keys;

        internal void UnregisterCustomQueryHandler(string name)
            => _queryHandlers.Remove(name);

        internal void ClearCustomQueryHandlers()
        {
            foreach (var name in CustomQueryHandlerNames())
            {
                UnregisterCustomQueryHandler(name);
            }
        }

        private IEnumerable<string> CustomQueryHandlerNames()
            => _queryHandlers.Keys.ToArray().Where(k => !_builtInHandlers.ContainsKey(k));
    }
}
