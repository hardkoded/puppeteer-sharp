using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Input;

namespace PuppeteerSharp.Locators
{
    /// <summary>
    /// Locators describe a strategy of locating elements and performing an action on
    /// them. If the action fails because the element is not ready for the action,
    /// the whole operation is retried. Various preconditions for a successful action
    /// are checked automatically.
    /// </summary>
    public abstract class Locator
    {
        /// <summary>
        /// For observables coming from promises, a delay is needed, otherwise the retry will
        /// never yield in a permanent failure for a promise.
        /// </summary>
        internal const int RetryDelay = 100;

        private bool _ensureElementIsInTheViewport = true;
        private bool _waitForEnabled = true;
        private bool _waitForStableBoundingBox = true;

        /// <summary>
        /// Gets or sets the visibility option.
        /// </summary>
        protected internal VisibilityOption? Visibility { get; set; }

        /// <summary>
        /// Gets or sets the timeout in milliseconds.
        /// </summary>
        protected internal int Timeout { get; set; } = 30_000;

        /// <summary>
        /// Gets or sets a value indicating whether to ensure the element is in the viewport.
        /// </summary>
        protected internal bool EnsureElementIsInTheViewport
        {
            get => _ensureElementIsInTheViewport;
            set => _ensureElementIsInTheViewport = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the element to be enabled.
        /// </summary>
        protected internal bool WaitForEnabled
        {
            get => _waitForEnabled;
            set => _waitForEnabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for a stable bounding box.
        /// </summary>
        protected internal bool WaitForStableBoundingBox
        {
            get => _waitForStableBoundingBox;
            set => _waitForStableBoundingBox = value;
        }

        /// <summary>
        /// Creates a new locator that races multiple locators, returning the first one to resolve.
        /// </summary>
        /// <param name="locators">The locators to race.</param>
        /// <returns>A new locator that resolves to the first matching locator's result.</returns>
        public static Locator Race(params Locator[] locators)
        {
            return new RaceLocator(locators);
        }

        /// <summary>
        /// Sets the timeout for the locator.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <returns>The locator instance for chaining.</returns>
        public Locator SetTimeout(int timeout)
        {
            Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the visibility option for the locator.
        /// </summary>
        /// <param name="visibility">The visibility option.</param>
        /// <returns>The locator instance for chaining.</returns>
        public Locator SetVisibility(VisibilityOption? visibility)
        {
            Visibility = visibility;
            return this;
        }

        /// <summary>
        /// Sets whether to wait for the element to be enabled before acting.
        /// </summary>
        /// <param name="value">Whether to wait for enabled.</param>
        /// <returns>The locator instance for chaining.</returns>
        public Locator SetWaitForEnabled(bool value)
        {
            _waitForEnabled = value;
            return this;
        }

        /// <summary>
        /// Sets whether to ensure the element is scrolled into the viewport.
        /// </summary>
        /// <param name="value">Whether to ensure the element is in the viewport.</param>
        /// <returns>The locator instance for chaining.</returns>
        public Locator SetEnsureElementIsInTheViewport(bool value)
        {
            _ensureElementIsInTheViewport = value;
            return this;
        }

        /// <summary>
        /// Sets whether to wait for a stable bounding box before acting.
        /// </summary>
        /// <param name="value">Whether to wait for a stable bounding box.</param>
        /// <returns>The locator instance for chaining.</returns>
        public Locator SetWaitForStableBoundingBox(bool value)
        {
            _waitForStableBoundingBox = value;
            return this;
        }

        /// <summary>
        /// Creates a new locator that filters elements using the provided predicate.
        /// If the predicate does not match, the locator will retry.
        /// </summary>
        /// <param name="predicate">A JavaScript function expression that takes an element and returns a boolean.</param>
        /// <returns>A new locator that filters based on the predicate.</returns>
        public Locator Filter(string predicate)
        {
            return new FilteredLocator(this, predicate);
        }

        /// <summary>
        /// Maps the locator using the provided mapper.
        /// </summary>
        /// <param name="mapper">A JavaScript function expression that transforms the element.</param>
        /// <returns>A new locator that maps the element.</returns>
        public Locator Map(string mapper)
        {
            return new MappedLocator(this, mapper);
        }

        /// <summary>
        /// Waits for the locator to get a handle from the page.
        /// </summary>
        /// <param name="options">Optional action options.</param>
        /// <returns>A task that resolves to a handle for the located element.</returns>
        public async Task<IJSHandle> WaitHandleAsync(LocatorActionOptions options = null)
        {
            return await RunWithRetryAsync(
                ct => WaitHandleCoreAsync(options, ct),
                options?.CancellationToken ?? default).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits for the locator to get a value from the page.
        /// Note this requires the value to be JSON-serializable.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="options">Optional action options.</param>
        /// <returns>A task that resolves to the serialized value.</returns>
        public async Task<T> WaitAsync<T>(LocatorActionOptions options = null)
        {
            var handle = await WaitHandleAsync(options).ConfigureAwait(false);

            try
            {
                return await handle.JsonValueAsync<T>().ConfigureAwait(false);
            }
            finally
            {
                await handle.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Waits for the locator to locate an element.
        /// </summary>
        /// <param name="options">Optional action options.</param>
        /// <returns>A task that completes when the element is located.</returns>
        public async Task WaitAsync(LocatorActionOptions options = null)
        {
            var handle = await WaitHandleAsync(options).ConfigureAwait(false);

            await handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Clicks the located element.
        /// </summary>
        /// <param name="options">Optional click options.</param>
        /// <returns>A task that completes when the click is performed.</returns>
        public Task ClickAsync(LocatorClickOptions options = null)
        {
            return PerformActionAsync(
                async (handle, ct) =>
                {
                    var clickOptions = new ClickOptions
                    {
                        Button = options?.Button ?? MouseButton.Left,
                        Count = options?.Count ?? 1,
                        Delay = options?.Delay ?? 0,
                        OffSet = options?.OffSet,
                    };
                    await handle.ClickAsync(clickOptions).ConfigureAwait(false);
                },
                true,
                options?.CancellationToken ?? default);
        }

        /// <summary>
        /// Hovers over the located element.
        /// </summary>
        /// <param name="options">Optional action options.</param>
        /// <returns>A task that completes when the hover is performed.</returns>
        public Task HoverAsync(LocatorActionOptions options = null)
        {
            return PerformActionAsync(
                async (handle, ct) => await handle.HoverAsync().ConfigureAwait(false),
                false,
                options?.CancellationToken ?? default);
        }

        /// <summary>
        /// Scrolls the located element.
        /// </summary>
        /// <param name="options">Optional scroll options.</param>
        /// <returns>A task that completes when the scroll is performed.</returns>
        public Task ScrollAsync(LocatorScrollOptions options = null)
        {
            return PerformActionAsync(
                async (handle, ct) =>
                {
                    await handle.EvaluateFunctionAsync(
                        @"(el, scrollTop, scrollLeft) => {
                            if (scrollTop !== undefined) { el.scrollTop = scrollTop; }
                            if (scrollLeft !== undefined) { el.scrollLeft = scrollLeft; }
                        }",
                        options?.ScrollTop,
                        options?.ScrollLeft).ConfigureAwait(false);
                },
                false,
                options?.CancellationToken ?? default);
        }

        /// <summary>
        /// Fills the located element with the provided value.
        /// </summary>
        /// <param name="value">The value to fill.</param>
        /// <param name="options">Optional action options.</param>
        /// <returns>A task that completes when the fill is performed.</returns>
        public Task FillAsync(string value, LocatorActionOptions options = null)
        {
            return PerformActionAsync(
                async (handle, ct) =>
                {
                    var inputType = await handle.EvaluateFunctionAsync<string>(
                        @"(el) => {
                            if (el instanceof HTMLSelectElement) return 'select';
                            if (el instanceof HTMLInputElement) {
                                if (new Set(['textarea','text','url','tel','search','password','number','email']).has(el.type))
                                    return 'typeable-input';
                                else return 'other-input';
                            }
                            if (el.isContentEditable) return 'contenteditable';
                            return 'unknown';
                        }").ConfigureAwait(false);

                    switch (inputType)
                    {
                        case "select":
                            await handle.SelectAsync(value).ConfigureAwait(false);
                            break;
                        case "contenteditable":
                        case "typeable-input":
                            var textToType = await handle.EvaluateFunctionAsync<string>(
                                @"(input, newValue) => {
                                    const currentValue = input.isContentEditable ? input.innerText : input.value;
                                    if (newValue.length <= currentValue.length || !newValue.startsWith(input.value)) {
                                        if (input.isContentEditable) { input.innerText = ''; } else { input.value = ''; }
                                        return newValue;
                                    }
                                    const originalValue = input.isContentEditable ? input.innerText : input.value;
                                    if (input.isContentEditable) { input.innerText = ''; input.innerText = originalValue; }
                                    else { input.value = ''; input.value = originalValue; }
                                    return newValue.substring(originalValue.length);
                                }",
                                value).ConfigureAwait(false);
                            await handle.TypeAsync(textToType).ConfigureAwait(false);
                            break;
                        case "other-input":
                            await handle.FocusAsync().ConfigureAwait(false);
                            await handle.EvaluateFunctionAsync(
                                @"(input, value) => {
                                    input.value = value;
                                    input.dispatchEvent(new Event('input', {bubbles: true}));
                                    input.dispatchEvent(new Event('change', {bubbles: true}));
                                }",
                                value).ConfigureAwait(false);
                            break;
                        default:
                            throw new PuppeteerException("Element cannot be filled out.");
                    }
                },
                true,
                options?.CancellationToken ?? default);
        }

        /// <summary>
        /// Copies options from another locator.
        /// </summary>
        /// <param name="other">The locator to copy options from.</param>
        internal void CopyOptions(Locator other)
        {
            Timeout = other.Timeout;
            Visibility = other.Visibility;
            _waitForEnabled = other._waitForEnabled;
            _ensureElementIsInTheViewport = other._ensureElementIsInTheViewport;
            _waitForStableBoundingBox = other._waitForStableBoundingBox;
        }

        /// <summary>
        /// Waits for the element and returns a handle.
        /// </summary>
        /// <param name="options">Optional action options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that resolves to a handle for the located element.</returns>
        internal abstract Task<IJSHandle> WaitHandleCoreAsync(LocatorActionOptions options, CancellationToken cancellationToken);

        private static async Task WaitForStableBoundingBoxAsync(IElementHandle handle)
        {
            var result = await handle.EvaluateFunctionAsync<bool>(
                @"(element) => {
                    return new Promise(resolve => {
                        window.requestAnimationFrame(() => {
                            const rect1 = element.getBoundingClientRect();
                            window.requestAnimationFrame(() => {
                                const rect2 = element.getBoundingClientRect();
                                resolve(
                                    rect1.x === rect2.x && rect1.y === rect2.y &&
                                    rect1.width === rect2.width && rect1.height === rect2.height
                                );
                            });
                        });
                    });
                }").ConfigureAwait(false);

            if (!result)
            {
                throw new PuppeteerException("Bounding box is not stable.");
            }
        }

        private async Task PerformActionAsync(
            Func<IElementHandle, CancellationToken, Task> action,
            bool waitForEnabled,
            CancellationToken cancellationToken)
        {
            await RunWithRetryAsync(
                async ct =>
                {
                    var handle = await WaitHandleCoreAsync(null, ct).ConfigureAwait(false);
                    var elementHandle = handle as IElementHandle;

                    if (elementHandle == null)
                    {
                        await handle.DisposeAsync().ConfigureAwait(false);
                        throw new PuppeteerException("Locator did not resolve to an element.");
                    }

                    try
                    {
                        if (EnsureElementIsInTheViewport)
                        {
                            var isInViewport = await elementHandle.IsIntersectingViewportAsync().ConfigureAwait(false);
                            if (!isInViewport)
                            {
                                await elementHandle.ScrollIntoViewAsync().ConfigureAwait(false);
                            }
                        }

                        if (WaitForStableBoundingBox)
                        {
                            await WaitForStableBoundingBoxAsync(elementHandle).ConfigureAwait(false);
                        }

                        if (waitForEnabled && WaitForEnabled)
                        {
                            await WaitForEnabledAsync(elementHandle).ConfigureAwait(false);
                        }

                        await action(elementHandle, ct).ConfigureAwait(false);
                        return handle;
                    }
                    catch
                    {
                        await handle.DisposeAsync().ConfigureAwait(false);
                        throw;
                    }
                },
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<IJSHandle> RunWithRetryAsync(
            Func<CancellationToken, Task<IJSHandle>> operation,
            CancellationToken cancellationToken)
        {
            using var timeoutCts = Timeout > 0
                ? new CancellationTokenSource(Timeout)
                : new CancellationTokenSource();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                timeoutCts.Token, cancellationToken);

            var linkedToken = linkedCts.Token;

            while (true)
            {
                try
                {
                    return await operation(linkedToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException($"Timed out after waiting {Timeout}ms");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception) when (!linkedToken.IsCancellationRequested)
                {
                    await Task.Delay(RetryDelay, linkedToken).ConfigureAwait(false);
                }
            }
        }

        private async Task WaitForEnabledAsync(IElementHandle handle)
        {
            var isEnabled = await handle.EvaluateFunctionAsync<bool>(
                @"(element) => {
                    if ('disabled' in element && typeof element.disabled === 'boolean') {
                        return !element.disabled;
                    }
                    return true;
                }").ConfigureAwait(false);

            if (!isEnabled)
            {
                throw new PuppeteerException("Element is disabled.");
            }
        }
    }
}
