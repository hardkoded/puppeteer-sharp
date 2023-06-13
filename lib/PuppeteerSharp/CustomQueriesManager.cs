using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PuppeteerSharp
{
    internal class CustomQueriesManager
    {
        private static readonly string[] _customQuerySeparators = new[] { "=", "/" };
        private readonly Dictionary<string, PuppeteerQueryHandler> _internalQueryHandlers = new()
        {
            ["aria"] = AriaQueryHandlerFactory.Create(),
            ["pierce"] = CreatePierceHandler(),
            ["text"] = CreateTextQueryHandler(),
            ["xpath"] = CreateXpathHandler(),
        };

        private readonly Dictionary<string, PuppeteerQueryHandler> _queryHandlers = new();

        private readonly Regex _customQueryHandlerNameRegex = new("[a-zA-Z]+$", RegexOptions.Compiled);
        private readonly PuppeteerQueryHandler _defaultHandler = CreatePuppeteerQueryHandler(new CustomQueryHandler
        {
            QueryOne = "(element, selector) => element.querySelector(selector)",
            QueryAll = "(element, selector) => element.querySelectorAll(selector)",
        });

        internal void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
        {
            if (_internalQueryHandlers.ContainsKey(name))
            {
                throw new PuppeteerException($"A query handler named \"{name}\" already exists");
            }

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
            var handlers = _internalQueryHandlers.Concat(_queryHandlers);

            foreach (var kv in handlers)
            {
                foreach (var separator in _customQuerySeparators)
                {
                    var prefix = $"{kv.Key}{separator}";

                    if (selector.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        selector = selector.Substring(prefix.Length);
                        return (selector, kv.Value);
                    }
                }
            }

            return (selector, _defaultHandler);
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

        private static PuppeteerQueryHandler CreatePierceHandler() =>
            CreatePuppeteerQueryHandler(new CustomQueryHandler
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

        private static PuppeteerQueryHandler CreateTextQueryHandler() =>
            CreatePuppeteerQueryHandler(new CustomQueryHandler
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

        private static PuppeteerQueryHandler CreateXpathHandler() =>
            CreatePuppeteerQueryHandler(new CustomQueryHandler
            {
                QueryOne = @"(element, selector) => {
                  const doc = element.ownerDocument || document;
                  const result = doc.evaluate(
                    selector,
                    element,
                    null,
                    XPathResult.FIRST_ORDERED_NODE_TYPE
                  );
                  return result.singleNodeValue;
                }",
                QueryAll = @"(element, selector) => {
                  const doc = element.ownerDocument || document;
                  const iterator = doc.evaluate(
                    selector,
                    element,
                    null,
                    XPathResult.ORDERED_NODE_ITERATOR_TYPE
                  );
                  const array = [];
                  let item;
                  while ((item = iterator.iterateNext())) {
                    array.push(item);
                  }
                  return array;
                },
              })",
            });

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

                internalHandler.WaitFor = async (IFrame frame, IElementHandle element, string selector, WaitForSelectorOptions options) =>
                {
                    var frameImpl = frame as Frame;

                    if (element != null)
                    {
                        frameImpl = ((ElementHandle)element).Frame;
                        element = (await frameImpl.PuppeteerWorld.AdoptHandleAsync(element).ConfigureAwait(false)) as IElementHandle;
                    }

                    var result = await frameImpl.PuppeteerWorld.WaitForSelectorInPageAsync(
                        handler.QueryOne,
                        element,
                        selector,
                        options).ConfigureAwait(false);

                    if (element != null)
                    {
                        await element.DisposeAsync().ConfigureAwait(false);
                    }

                    if (result == null)
                    {
                        return null;
                    }

                    return await frameImpl.MainWorld.TransferHandleAsync(result).ConfigureAwait(false) as IElementHandle;
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

        private IEnumerable<string> CustomQueryHandlerNames() => _queryHandlers.Keys.ToArray();
    }
}
