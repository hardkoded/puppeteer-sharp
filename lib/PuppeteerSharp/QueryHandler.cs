using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class QueryHandler<T>
    {
        private string _querySelector;
        private string _querySelectorAll;

        private static readonly Lazy<QueryHandler<T>> _instance = new(() => new QueryHandler<T>());

        public static QueryHandler Instance => _instance.Value;

        internal string QuerySelectorAll
        {
            get
            {
                if (!string.IsNullOrEmpty(_querySelectorAll))
                {
                    return _querySelectorAll;
                }

                var querySelectorAll = @"async (node, selector, PuppeteerUtil) => {
                    const querySelectorAll = 'FUNCTION_DEFINITION';
                    const result = await querySelector(node, selector, PuppeteerUtil);
                    if (result) {
                        yield result;
                    }
                }";

                // Upstream uses a CreateFunction util. We don't need that because we always use strings instead of node functions.
                _querySelectorAll = querySelectorAll.Replace("FUNCTION_DEFINITION", QuerySelector);
                return _querySelectorAll;
            }
            protected set
            {
                _querySelectorAll = value;
            }
        }

        internal string QuerySelector
        {
            get
            {
                if (!string.IsNullOrEmpty(_querySelector))
                {
                    return _querySelector;
                }

                if (string.IsNullOrEmpty(QuerySelectorAll))
                {
                    throw new PuppeteerException("Cannot create default query selector");
                }

                var querySelector = @"async (node, selector, PuppeteerUtil) => {
                    const querySelectorAll = 'FUNCTION_DEFINITION';
                    const results = querySelectorAll(node, selector, PuppeteerUtil);
                    for await (const result of results) {
                        return result;
                    }
                    return null;
                }";

                // Upstream uses a CreateFunction util. We don't need that because we always use strings instead of node functions.
                _querySelector = querySelector.Replace("FUNCTION_DEFINITION", QuerySelectorAll);
                return _querySelector;
            }
            protected set
            {
                _querySelector = value;
            }
        }

        public Func<IElementHandle, string, Task<IElementHandle>> QueryOne { get; set; }

        public Func<IFrame, IElementHandle, string, WaitForSelectorOptions, Task<IElementHandle>> WaitFor { get; set; }

        public ICollection Func<IElementHandle, string, Task<IElementHandle[]>> QueryAll { get; set; }
    }
}
