(() => {
    const module = {};
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
        if (from && typeof from === "object" || typeof from === "function") {
            for (let key of __getOwnPropNames(from))
                if (!__hasOwnProp.call(to, key) && key !== except)
                    __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
        }
        return to;
    };
    var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);
    var __accessCheck = (obj, member, msg) => {
        if (!member.has(obj))
            throw TypeError("Cannot " + msg);
    };
    var __privateGet = (obj, member, getter) => {
        __accessCheck(obj, member, "read from private field");
        return getter ? getter.call(obj) : member.get(obj);
    };
    var __privateAdd = (obj, member, value) => {
        if (member.has(obj))
            throw TypeError("Cannot add the same private member more than once");
        member instanceof WeakSet ? member.add(obj) : member.set(obj, value);
    };
    var __privateSet = (obj, member, value, setter) => {
        __accessCheck(obj, member, "write to private field");
        setter ? setter.call(obj, value) : member.set(obj, value);
        return value;
    };

    // src/injected/injected.ts
    var injected_exports = {};
    __export(injected_exports, {
        default: () => injected_default
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
    var TimeoutError = class extends CustomError {
    };
    var ProtocolError = class extends CustomError {
        constructor() {
            super(...arguments);
            this.originalMessage = "";
        }
    };
    var errors = Object.freeze({
        TimeoutError,
        ProtocolError
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
        const timeoutId = opts && opts.timeout > 0 ? setTimeout(() => {
            isRejected = true;
            rejector(new TimeoutError(opts.message));
        }, opts.timeout) : void 0;
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
            }
        });
    }

    // src/injected/util.ts
    var util_exports = {};
    __export(util_exports, {
        checkVisibility: () => checkVisibility,
        createFunction: () => createFunction
    });
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
    var HIDDEN_VISIBILITY_VALUES = ["hidden", "collapse"];
    var checkVisibility = (node, visible) => {
        if (!node) {
            return visible === false;
        }
        if (visible === void 0) {
            return node;
        }
        const element = node.nodeType === Node.TEXT_NODE ? node.parentElement : node;
        const style = window.getComputedStyle(element);
        const isVisible = style && !HIDDEN_VISIBILITY_VALUES.includes(style.visibility) && isBoundingBoxVisible(element);
        return visible === isVisible ? node : false;
    };
    function isBoundingBoxVisible(element) {
        const rect = element.getBoundingClientRect();
        return rect.width > 0 && rect.height > 0 && rect.right > 0 && rect.bottom > 0 && rect.left < self.innerWidth && rect.top < self.innerHeight;
    }

    // src/injected/Poller.ts
    var Poller_exports = {};
    __export(Poller_exports, {
        IntervalPoller: () => IntervalPoller,
        MutationPoller: () => MutationPoller,
        RAFPoller: () => RAFPoller
    });

    // src/util/assert.ts
    var assert = (value, message) => {
        if (!value) {
            throw new Error(message);
        }
    };

    // src/injected/Poller.ts
    var _fn, _root, _observer, _promise;
    var MutationPoller = class {
        constructor(fn, root) {
            __privateAdd(this, _fn, void 0);
            __privateAdd(this, _root, void 0);
            __privateAdd(this, _observer, void 0);
            __privateAdd(this, _promise, void 0);
            __privateSet(this, _fn, fn);
            __privateSet(this, _root, root);
        }
        async start() {
            const promise = __privateSet(this, _promise, createDeferredPromise());
            const result = await __privateGet(this, _fn).call(this);
            if (result) {
                promise.resolve(result);
                return;
            }
            __privateSet(this, _observer, new MutationObserver(async () => {
                const result2 = await __privateGet(this, _fn).call(this);
                if (!result2) {
                    return;
                }
                promise.resolve(result2);
                await this.stop();
            }));
            __privateGet(this, _observer).observe(__privateGet(this, _root), {
                childList: true,
                subtree: true,
                attributes: true
            });
        }
        async stop() {
            assert(__privateGet(this, _promise), "Polling never started.");
            if (!__privateGet(this, _promise).finished()) {
                __privateGet(this, _promise).reject(new Error("Polling stopped"));
            }
            if (__privateGet(this, _observer)) {
                __privateGet(this, _observer).disconnect();
                __privateSet(this, _observer, void 0);
            }
        }
        result() {
            assert(__privateGet(this, _promise), "Polling never started.");
            return __privateGet(this, _promise);
        }
    };
    _fn = new WeakMap();
    _root = new WeakMap();
    _observer = new WeakMap();
    _promise = new WeakMap();
    var _fn2, _promise2;
    var RAFPoller = class {
        constructor(fn) {
            __privateAdd(this, _fn2, void 0);
            __privateAdd(this, _promise2, void 0);
            __privateSet(this, _fn2, fn);
        }
        async start() {
            const promise = __privateSet(this, _promise2, createDeferredPromise());
            const result = await __privateGet(this, _fn2).call(this);
            if (result) {
                promise.resolve(result);
                return;
            }
            const poll = async () => {
                if (promise.finished()) {
                    return;
                }
                const result2 = await __privateGet(this, _fn2).call(this);
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
            assert(__privateGet(this, _promise2), "Polling never started.");
            if (!__privateGet(this, _promise2).finished()) {
                __privateGet(this, _promise2).reject(new Error("Polling stopped"));
            }
        }
        result() {
            assert(__privateGet(this, _promise2), "Polling never started.");
            return __privateGet(this, _promise2);
        }
    };
    _fn2 = new WeakMap();
    _promise2 = new WeakMap();
    var _fn3, _ms, _interval, _promise3;
    var IntervalPoller = class {
        constructor(fn, ms) {
            __privateAdd(this, _fn3, void 0);
            __privateAdd(this, _ms, void 0);
            __privateAdd(this, _interval, void 0);
            __privateAdd(this, _promise3, void 0);
            __privateSet(this, _fn3, fn);
            __privateSet(this, _ms, ms);
        }
        async start() {
            const promise = __privateSet(this, _promise3, createDeferredPromise());
            const result = await __privateGet(this, _fn3).call(this);
            if (result) {
                promise.resolve(result);
                return;
            }
            __privateSet(this, _interval, setInterval(async () => {
                const result2 = await __privateGet(this, _fn3).call(this);
                if (!result2) {
                    return;
                }
                promise.resolve(result2);
                await this.stop();
            }, __privateGet(this, _ms)));
        }
        async stop() {
            assert(__privateGet(this, _promise3), "Polling never started.");
            if (!__privateGet(this, _promise3).finished()) {
                __privateGet(this, _promise3).reject(new Error("Polling stopped"));
            }
            if (__privateGet(this, _interval)) {
                clearInterval(__privateGet(this, _interval));
                __privateSet(this, _interval, void 0);
            }
        }
        result() {
            assert(__privateGet(this, _promise3), "Polling never started.");
            return __privateGet(this, _promise3);
        }
    };
    _fn3 = new WeakMap();
    _ms = new WeakMap();
    _interval = new WeakMap();
    _promise3 = new WeakMap();

    // src/injected/TextContent.ts
    var TextContent_exports = {};
    __export(TextContent_exports, {
        createTextContent: () => createTextContent
    });
    var TRIVIAL_VALUE_INPUT_TYPES = /* @__PURE__ */ new Set(["checkbox", "image", "radio"]);
    var isNonTrivialValueNode = (node) => {
        if (node instanceof HTMLSelectElement) {
            return true;
        }
        if (node instanceof HTMLTextAreaElement) {
            return true;
        }
        if (node instanceof HTMLInputElement && !TRIVIAL_VALUE_INPUT_TYPES.has(node.type)) {
            return true;
        }
        return false;
    };
    var UNSUITABLE_NODE_NAMES = /* @__PURE__ */ new Set(["SCRIPT", "STYLE"]);
    var isSuitableNodeForTextMatching = (node) => {
        var _a;
        return !UNSUITABLE_NODE_NAMES.has(node.nodeName) && !((_a = document.head) == null ? void 0 : _a.contains(node));
    };
    var textContentCache = /* @__PURE__ */ new Map();
    var createTextContent = (root) => {
        var _a, _b;
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
        } else {
            for (let child = root.firstChild; child; child = child.nextSibling) {
                if (child.nodeType === Node.TEXT_NODE) {
                    value.full += (_a = child.nodeValue) != null ? _a : "";
                    currentImmediate += (_b = child.nodeValue) != null ? _b : "";
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
        }
        textContentCache.set(root, value);
        return value;
    };

    // src/injected/injected.ts
    var PuppeteerUtil = Object.freeze({
        ...util_exports,
        ...Poller_exports,
        ...TextContent_exports,
        createDeferredPromise
    });
    var injected_default = PuppeteerUtil;

    return module.exports.default;
})()