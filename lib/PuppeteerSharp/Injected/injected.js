"use strict";
var __defProp = Object.defineProperty;
var __export = (target, all) => {
    for (var name in all)
        __defProp(target, name, { get: all[name], enumerable: true });
};
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
    let resolver = (_) => {
    };
    let rejector = (_) => {
    };
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
            return result;
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
        return __privateGet(this, _promise);
    }
    async stop() {
        assert(__privateGet(this, _promise), "Polling never started.");
        if (!__privateGet(this, _promise).finished()) {
            __privateGet(this, _promise).reject(new Error("Polling stopped"));
        }
        if (__privateGet(this, _observer)) {
            __privateGet(this, _observer).disconnect();
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
            return result;
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
        return __privateGet(this, _promise2);
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
            return result;
        }
        __privateSet(this, _interval, setInterval(async () => {
            const result2 = await __privateGet(this, _fn3).call(this);
            if (!result2) {
                return;
            }
            promise.resolve(result2);
            await this.stop();
        }, __privateGet(this, _ms)));
        return __privateGet(this, _promise3);
    }
    async stop() {
        assert(__privateGet(this, _promise3), "Polling never started.");
        if (!__privateGet(this, _promise3).finished()) {
            __privateGet(this, _promise3).reject(new Error("Polling stopped"));
        }
        if (__privateGet(this, _interval)) {
            clearInterval(__privateGet(this, _interval));
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

// src/injected/util.ts
var util_exports = {};
__export(util_exports, {
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

// src/injected/injected.ts
Object.assign(
    self,
    Object.freeze({
        InjectedUtil: {
            ...Poller_exports,
            ...util_exports,
            createDeferredPromise
        }
    })
);