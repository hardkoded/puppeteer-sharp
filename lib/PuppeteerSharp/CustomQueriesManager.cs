using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    internal class CustomQueriesManager
    {
        private readonly Dictionary<string, PuppeteerQueryHandler> _queryHandlers = new();
        private readonly PuppeteerQueryHandler _pierceHandler = CreatePuppeteerQueryHandler(new CustomQueryHandler
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

        private readonly PuppeteerQueryHandler _ariaHandler = AriaQueryHandlerFactory.Create();
        private readonly Dictionary<string, PuppeteerQueryHandler> _builtInHandlers;
        private readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private readonly Regex _customQueryHandlerParserRegex = new("(?<query>^[a-zA-Z]+)\\/(?<selector>.*)", RegexOptions.Compiled);
        private readonly PuppeteerQueryHandler _defaultHandler = CreatePuppeteerQueryHandler(new CustomQueryHandler
        {
            QueryOne = "(element, selector) => element.querySelector(selector)",
            QueryAll = "(element, selector) => element.querySelectorAll(selector)",
        });

        private readonly PuppeteerQueryHandler _textQueryHandler = CreatePuppeteerQueryHandler(new CustomQueryHandler
        {
            QueryOne = @"(element, selector, {createTextContent}) => {
                const search = (root)=> {
                  for (const node of root.childNodes) {
                    if (node instanceof Element) {
                      let matchedNode;
                      if (node.shadowRoot) {
                        matchedNode = search(node.shadowRoot);
                      } else {
                        matchedNode = search(node);
                      }
                      if (matchedNode) {
                        return matchedNode;
                      }
                    }
                  }
                  const textContent = createTextContent(root);
                  if (textContent.full.includes(selector)) {
                    return root;
                  }
                  return null;
                };
                return search(element);
              }",
            QueryAll = @"(element, selector, {createTextContent}) => {
                const search = (root) => {
                  let results = [];
                  for (const node of root.childNodes) {
                    if (node instanceof Element) {
                      let matchedNodes;
                      if (node.shadowRoot) {
                        matchedNodes = search(node.shadowRoot);
                      } else {
                        matchedNodes = search(node);
                      }
                      results = results.concat(matchedNodes);
                    }
                  }
                  if (results.length > 0) {
                    return results;
                  }

                  const textContent = createTextContent(root);
                  if (textContent.full.includes(selector)) {
                    return [root];
                  }
                  return [];
                };
                return search(element);
              }",
        });

        public CustomQueriesManager()
        {
            _builtInHandlers = new()
            {
                ["aria"] = _ariaHandler,
                ["pierce"] = _pierceHandler,
                ["text"] = _textQueryHandler,
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

            var internalHandler = CreatePuppeteerQueryHandler(queryHandler);

            _queryHandlers.Add(name, internalHandler);
        }

        internal (string UpdatedSelector, PuppeteerQueryHandler QueryHandler) GetQueryHandlerAndSelector(string selector)
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

        private static PuppeteerQueryHandler CreatePuppeteerQueryHandler(CustomQueryHandler handler)
        {
            var internalHandler = new PuppeteerQueryHandler();

            if (!string.IsNullOrEmpty(handler.QueryOne))
            {
                internalHandler.QueryOne = async (IElementHandle element, string selector) =>
                {
                    var handle = element as JSHandle;
                    var jsHandle = await element.EvaluateFunctionHandleAsync(
                        handler.QueryOne,
                        selector,
                        await handle.ExecutionContext.World.GetPuppeteerUtilAsync().ConfigureAwait(false))
                        .ConfigureAwait(false);
                    if (jsHandle is ElementHandle elementHandle)
                    {
                        return elementHandle;
                    }

                    await jsHandle.DisposeAsync().ConfigureAwait(false);
                    return null;
                };
            }

            if (!string.IsNullOrEmpty(handler.QueryAll))
            {
                internalHandler.QueryAll = async (IElementHandle element, string selector) =>
                {
                    var handle = element as JSHandle;
                    var jsHandle = await element.EvaluateFunctionHandleAsync(
                        handler.QueryAll,
                        selector,
                        await handle.ExecutionContext.World.GetPuppeteerUtilAsync().ConfigureAwait(false))
                        .ConfigureAwait(false);
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
            }

            return internalHandler;
        }

        private IEnumerable<string> CustomQueryHandlerNames()
            => _queryHandlers.Keys.ToArray().Where(k => !_builtInHandlers.ContainsKey(k));
    }
}
