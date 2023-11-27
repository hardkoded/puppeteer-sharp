"use strict";
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
    for (var name in all)
        __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
    if ((from && typeof from === "object") || typeof from === "function") {
        for (let key of __getOwnPropNames(from))
            if (!__hasOwnProp.call(to, key) && key !== except)
                __defProp(to, key, {
                    get: () => from[key],
                    enumerable:
                        !(desc = __getOwnPropDesc(from, key)) ||
                        desc.enumerable,
                });
    }
    return to;
};
var __toCommonJS = (mod) =>
    __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/injected/injected.ts
var injected_exports = {};
__export(injected_exports, {
    default: () => injected_default,
});
module.exports = __toCommonJS(injected_exports);

// src/common/Errors.ts
var CustomError = class extends Error {
    constructor(message) {
        super(message);
        this.name = this.constructor.name;
        Error.captureStackTrace(this, this.constructor);
    }
};
var TimeoutError = class extends CustomError {};
var ProtocolError = class extends CustomError {
    #code;
    #originalMessage = "";
    set code(code) {
        this.#code = code;
    }
    get code() {
        return this.#code;
    }
    set originalMessage(originalMessage) {
        this.#originalMessage = originalMessage;
    }
    get originalMessage() {
        return this.#originalMessage;
    }
};
var errors = Object.freeze({
    TimeoutError,
    ProtocolError,
});

// src/util/DeferredPromise.ts
function createDeferredPromise(opts) {
    let isResolved = false;
    let isRejected = false;
    let resolver;
    let rejector;
    const taskPromise = new Promise((resolve, reject) => {
        resolver = resolve;
        rejector = reject;
    });
    const timeoutId =
        opts && opts.timeout > 0
            ? setTimeout(() => {
                  isRejected = true;
                  rejector(new TimeoutError(opts.message));
              }, opts.timeout)
            : void 0;
    return Object.assign(taskPromise, {
        resolved: () => {
            return isResolved;
        },
        finished: () => {
            return isResolved || isRejected;
        },
        resolve: (value) => {
            if (timeoutId) {
                clearTimeout(timeoutId);
            }
            isResolved = true;
            resolver(value);
        },
        reject: (err) => {
            clearTimeout(timeoutId);
            isRejected = true;
            rejector(err);
        },
    });
}

// src/util/Function.ts
var createdFunctions = /* @__PURE__ */ new Map();
var createFunction = (functionValue) => {
    let fn = createdFunctions.get(functionValue);
    if (fn) {
        return fn;
    }
    fn = new Function(`return ${functionValue}`)();
    createdFunctions.set(functionValue, fn);
    return fn;
};

// src/injected/ARIAQuerySelector.ts
var ARIAQuerySelector_exports = {};
__export(ARIAQuerySelector_exports, {
    ariaQuerySelector: () => ariaQuerySelector,
});
var ariaQuerySelector = (root, selector) => {
    return window.__ariaQuerySelector(root, selector);
};

// src/injected/CustomQuerySelector.ts
var CustomQuerySelector_exports = {};
__export(CustomQuerySelector_exports, {
    customQuerySelectors: () => customQuerySelectors,
});
var CustomQuerySelectorRegistry = class {
    #selectors = /* @__PURE__ */ new Map();
    register(name, handler) {
        if (!handler.queryOne && handler.queryAll) {
            const querySelectorAll = handler.queryAll;
            handler.queryOne = (node, selector) => {
                for (const result of querySelectorAll(node, selector)) {
                    return result;
                }
                return null;
            };
        } else if (handler.queryOne && !handler.queryAll) {
            const querySelector = handler.queryOne;
            handler.queryAll = (node, selector) => {
                const result = querySelector(node, selector);
                return result ? [result] : [];
            };
        } else if (!handler.queryOne || !handler.queryAll) {
            throw new Error("At least one query method must be defined.");
        }
        this.#selectors.set(name, {
            querySelector: handler.queryOne,
            querySelectorAll: handler.queryAll,
        });
    }
    unregister(name) {
        this.#selectors.delete(name);
    }
    get(name) {
        return this.#selectors.get(name);
    }
    clear() {
        this.#selectors.clear();
    }
};
var customQuerySelectors = new CustomQuerySelectorRegistry();

// src/injected/PierceQuerySelector.ts
var PierceQuerySelector_exports = {};
__export(PierceQuerySelector_exports, {
    pierceQuerySelector: () => pierceQuerySelector,
    pierceQuerySelectorAll: () => pierceQuerySelectorAll,
});
var pierceQuerySelector = (root, selector) => {
    let found = null;
    const search = (root2) => {
        const iter = document.createTreeWalker(root2, NodeFilter.SHOW_ELEMENT);
        do {
            const currentNode = iter.currentNode;
            if (currentNode.shadowRoot) {
                search(currentNode.shadowRoot);
            }
            if (currentNode instanceof ShadowRoot) {
                continue;
            }
            if (
                currentNode !== root2 &&
                !found &&
                currentNode.matches(selector)
            ) {
                found = currentNode;
            }
        } while (!found && iter.nextNode());
    };
    if (root instanceof Document) {
        root = root.documentElement;
    }
    search(root);
    return found;
};
var pierceQuerySelectorAll = (element, selector) => {
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
};

// src/util/assert.ts
var assert = (value, message) => {
    if (!value) {
        throw new Error(message);
    }
};

// src/injected/Poller.ts
var MutationPoller = class {
    #fn;
    #root;
    #observer;
    #promise;
    constructor(fn, root) {
        this.#fn = fn;
        this.#root = root;
    }
    async start() {
        const promise = (this.#promise = createDeferredPromise());
        const result = await this.#fn();
        if (result) {
            promise.resolve(result);
            return;
        }
        this.#observer = new MutationObserver(async () => {
            const result2 = await this.#fn();
            if (!result2) {
                return;
            }
            promise.resolve(result2);
            await this.stop();
        });
        this.#observer.observe(this.#root, {
            childList: true,
            subtree: true,
            attributes: true,
        });
    }
    async stop() {
        assert(this.#promise, "Polling never started.");
        if (!this.#promise.finished()) {
            this.#promise.reject(new Error("Polling stopped"));
        }
        if (this.#observer) {
            this.#observer.disconnect();
            this.#observer = void 0;
        }
    }
    result() {
        assert(this.#promise, "Polling never started.");
        return this.#promise;
    }
};
var RAFPoller = class {
    #fn;
    #promise;
    constructor(fn) {
        this.#fn = fn;
    }
    async start() {
        const promise = (this.#promise = createDeferredPromise());
        const result = await this.#fn();
        if (result) {
            promise.resolve(result);
            return;
        }
        const poll = async () => {
            if (promise.finished()) {
                return;
            }
            const result2 = await this.#fn();
            if (!result2) {
                window.requestAnimationFrame(poll);
                return;
            }
            promise.resolve(result2);
            await this.stop();
        };
        window.requestAnimationFrame(poll);
    }
    async stop() {
        assert(this.#promise, "Polling never started.");
        if (!this.#promise.finished()) {
            this.#promise.reject(new Error("Polling stopped"));
        }
    }
    result() {
        assert(this.#promise, "Polling never started.");
        return this.#promise;
    }
};
var IntervalPoller = class {
    #fn;
    #ms;
    #interval;
    #promise;
    constructor(fn, ms) {
        this.#fn = fn;
        this.#ms = ms;
    }
    async start() {
        const promise = (this.#promise = createDeferredPromise());
        const result = await this.#fn();
        if (result) {
            promise.resolve(result);
            return;
        }
        this.#interval = setInterval(async () => {
            const result2 = await this.#fn();
            if (!result2) {
                return;
            }
            promise.resolve(result2);
            await this.stop();
        }, this.#ms);
    }
    async stop() {
        assert(this.#promise, "Polling never started.");
        if (!this.#promise.finished()) {
            this.#promise.reject(new Error("Polling stopped"));
        }
        if (this.#interval) {
            clearInterval(this.#interval);
            this.#interval = void 0;
        }
    }
    result() {
        assert(this.#promise, "Polling never started.");
        return this.#promise;
    }
};

// src/injected/TextContent.ts
var TRIVIAL_VALUE_INPUT_TYPES = /* @__PURE__ */ new Set([
    "checkbox",
    "image",
    "radio",
]);
var isNonTrivialValueNode = (node) => {
    if (node instanceof HTMLSelectElement) {
        return true;
    }
    if (node instanceof HTMLTextAreaElement) {
        return true;
    }
    if (
        node instanceof HTMLInputElement &&
        !TRIVIAL_VALUE_INPUT_TYPES.has(node.type)
    ) {
        return true;
    }
    return false;
};
var UNSUITABLE_NODE_NAMES = /* @__PURE__ */ new Set(["SCRIPT", "STYLE"]);
var isSuitableNodeForTextMatching = (node) => {
    return (
        !UNSUITABLE_NODE_NAMES.has(node.nodeName) &&
        !document.head?.contains(node)
    );
};
var textContentCache = /* @__PURE__ */ new WeakMap();
var eraseFromCache = (node) => {
    while (node) {
        textContentCache.delete(node);
        if (node instanceof ShadowRoot) {
            node = node.host;
        } else {
            node = node.parentNode;
        }
    }
};
var observedNodes = /* @__PURE__ */ new WeakSet();
var textChangeObserver = new MutationObserver((mutations) => {
    for (const mutation of mutations) {
        eraseFromCache(mutation.target);
    }
});
var createTextContent = (root) => {
    let value = textContentCache.get(root);
    if (value) {
        return value;
    }
    value = { full: "", immediate: [] };
    if (!isSuitableNodeForTextMatching(root)) {
        return value;
    }
    let currentImmediate = "";
    if (isNonTrivialValueNode(root)) {
        value.full = root.value;
        value.immediate.push(root.value);
        root.addEventListener(
            "input",
            (event) => {
                eraseFromCache(event.target);
            },
            { once: true, capture: true }
        );
    } else {
        for (let child = root.firstChild; child; child = child.nextSibling) {
            if (child.nodeType === Node.TEXT_NODE) {
                value.full += child.nodeValue ?? "";
                currentImmediate += child.nodeValue ?? "";
                continue;
            }
            if (currentImmediate) {
                value.immediate.push(currentImmediate);
            }
            currentImmediate = "";
            if (child.nodeType === Node.ELEMENT_NODE) {
                value.full += createTextContent(child).full;
            }
        }
        if (currentImmediate) {
            value.immediate.push(currentImmediate);
        }
        if (root instanceof Element && root.shadowRoot) {
            value.full += createTextContent(root.shadowRoot).full;
        }
        if (!observedNodes.has(root)) {
            textChangeObserver.observe(root, {
                childList: true,
                characterData: true,
                subtree: true,
            });
            observedNodes.add(root);
        }
    }
    textContentCache.set(root, value);
    return value;
};

// src/injected/TextQuerySelector.ts
var TextQuerySelector_exports = {};
__export(TextQuerySelector_exports, {
    textQuerySelectorAll: () => textQuerySelectorAll,
});
var textQuerySelectorAll = function* (root, selector) {
    let yielded = false;
    for (const node of root.childNodes) {
        if (node instanceof Element && isSuitableNodeForTextMatching(node)) {
            let matches;
            if (!node.shadowRoot) {
                matches = textQuerySelectorAll(node, selector);
            } else {
                matches = textQuerySelectorAll(node.shadowRoot, selector);
            }
            for (const match of matches) {
                yield match;
                yielded = true;
            }
        }
    }
    if (yielded) {
        return;
    }
    if (root instanceof Element && isSuitableNodeForTextMatching(root)) {
        const textContent = createTextContent(root);
        if (textContent.full.includes(selector)) {
            yield root;
        }
    }
};

// src/injected/util.ts
var util_exports = {};
__export(util_exports, {
    checkVisibility: () => checkVisibility,
});
var HIDDEN_VISIBILITY_VALUES = ["hidden", "collapse"];
var checkVisibility = (node, visible) => {
    if (!node) {
        return visible === false;
    }
    if (visible === void 0) {
        return node;
    }
    const element =
        node.nodeType === Node.TEXT_NODE ? node.parentElement : node;
    const style = window.getComputedStyle(element);
    const isVisible =
        style &&
        !HIDDEN_VISIBILITY_VALUES.includes(style.visibility) &&
        !isBoundingBoxEmpty(element);
    return visible === isVisible ? node : false;
};
function isBoundingBoxEmpty(element) {
    const rect = element.getBoundingClientRect();
    return rect.width === 0 || rect.height === 0;
}

// src/injected/XPathQuerySelector.ts
var XPathQuerySelector_exports = {};
__export(XPathQuerySelector_exports, {
    xpathQuerySelectorAll: () => xpathQuerySelectorAll,
});
var xpathQuerySelectorAll = function* (root, selector) {
    const doc = root.ownerDocument || document;
    const iterator = doc.evaluate(
        selector,
        root,
        null,
        XPathResult.ORDERED_NODE_ITERATOR_TYPE
    );
    let item;
    while ((item = iterator.iterateNext())) {
        yield item;
    }
};

// src/injected/injected.ts
var PuppeteerUtil = Object.freeze({
    ...ARIAQuerySelector_exports,
    ...CustomQuerySelector_exports,
    ...PierceQuerySelector_exports,
    ...TextQuerySelector_exports,
    ...util_exports,
    ...XPathQuerySelector_exports,
    createDeferredPromise,
    createFunction,
    createTextContent,
    IntervalPoller,
    isSuitableNodeForTextMatching,
    MutationPoller,
    RAFPoller,
});
var injected_default = PuppeteerUtil;
