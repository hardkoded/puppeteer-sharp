using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp
{
    /// <inheritdoc cref="PuppeteerSharp.IElementHandle" />
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class ElementHandle : JSHandle, IElementHandle
    {
        internal ElementHandle(
            IsolatedWorld world,
            RemoteObject remoteObject) : base(world, remoteObject)
        {
        }

        /// <inheritdoc/>
        IFrame IElementHandle.Frame => Frame;

        internal abstract CustomQuerySelectorRegistry CustomQuerySelectorRegistry { get; }

        /// <summary>
        /// Base handle.
        /// </summary>
        protected JSHandle Handle { get; init; }

        /// <summary>
        /// Element's page.
        /// </summary>
        protected abstract Page Page { get; }

        private string DebuggerDisplay =>
            string.IsNullOrEmpty(RemoteObject.ClassName)
                ? ToString()
                : $"{RemoteObject.ClassName}@{RemoteObject.Description}";

        /// <inheritdoc/>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ElementScreenshotOptions());

        /// <inheritdoc/>
        public async Task ScreenshotAsync(string file, ElementScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Type ??= ScreenshotOptions.GetScreenshotTypeFromFile(file);

            var data = await ScreenshotDataAsync(options).ConfigureAwait(false);

            using var fs = AsyncFileHelper.CreateStream(file, FileMode.Create);
            await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ElementScreenshotOptions());

        /// <inheritdoc/>
        public async Task<Stream> ScreenshotStreamAsync(ElementScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ElementScreenshotOptions());

        /// <inheritdoc/>
        public async Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            if (string.IsNullOrEmpty(selector))
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var (updatedSelector, queryHandler) = CustomQuerySelectorRegistry.GetQueryHandlerAndSelector(selector);
            return await BindIsolatedHandleAsync<IElementHandle, ElementHandle>(handle =>
                queryHandler.WaitForAsync(null, handle, updatedSelector, options)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ElementScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ElementScreenshotOptions());

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async(ElementScreenshotOptions options)
            => BindIsolatedHandleAsync<string, ElementHandle>(async handle =>
            {
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                var clip = await handle.NonEmptyVisibleBoundingBoxAsync().ConfigureAwait(false);
                var page = handle.Page;

                if (options.ScrollIntoView)
                {
                    await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);

                    // We measure again just in case.
                    clip = await handle.NonEmptyVisibleBoundingBoxAsync().ConfigureAwait(false);
                }

                var points = await EvaluateFunctionAsync<int[]>(@"() => {
                    if (!window.visualViewport) {
                        throw new Error('window.visualViewport is not supported.');
                    }
                    return [
                        window.visualViewport.pageLeft,
                        window.visualViewport.pageTop,
                    ];
                }").ConfigureAwait(false);

                clip.X += points[0];
                clip.Y += points[1];

                options.Clip = clip.ToClip();

                return await page.ScreenshotBase64Async(options).ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public Task HoverAsync()
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.MoveAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task ClickAsync(ClickOptions options = null)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync(options?.OffSet).ConfigureAwait(false);
                await Page.Mouse.ClickAsync(clickablePoint.X, clickablePoint.Y, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task UploadFileAsync(params string[] filePaths) => UploadFileAsync(true, filePaths);

        /// <inheritdoc/>
        public abstract Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths);

        /// <inheritdoc/>
        public Task TapAsync()
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Touchscreen.TapAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task TouchStartAsync()
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Touchscreen.TouchStartAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task TouchMoveAsync()
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Touchscreen.TouchMoveAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task TouchEndAsync()
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                await Page.Touchscreen.TouchEndAsync().ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task FocusAsync() =>
            BindIsolatedHandleAsync<JToken, ElementHandle>(handle => handle.EvaluateFunctionAsync("element => element.focus()"));

        /// <inheritdoc/>
        public Task TypeAsync(string text, TypeOptions options = null)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.FocusAsync().ConfigureAwait(false);
                await Page.Keyboard.TypeAsync(text, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task PressAsync(string key, PressOptions options = null)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.FocusAsync().ConfigureAwait(false);
                await Page.Keyboard.PressAsync(key, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(handle =>
            {
                if (string.IsNullOrEmpty(selector))
                {
                    throw new ArgumentNullException(nameof(selector));
                }

                var (updatedSelector, queryHandler) = CustomQuerySelectorRegistry.GetQueryHandlerAndSelector(selector);
                return queryHandler.QueryOneAsync(handle, updatedSelector);
            });

        /// <inheritdoc/>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
            => BindIsolatedHandleAsync<IElementHandle[], ElementHandle>(async handle =>
            {
                if (string.IsNullOrEmpty(selector))
                {
                    throw new ArgumentNullException(nameof(selector));
                }

                var (updatedSelector, queryHandler) = CustomQuerySelectorRegistry.GetQueryHandlerAndSelector(selector);
                var result = new List<IElementHandle>();
                await foreach (var item in queryHandler.QueryAllAsync(handle, updatedSelector))
                {
                    result.Add(item);
                }

                return result.ToArray();
            });

        /// <inheritdoc/>
        public async Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            if (string.IsNullOrEmpty(selector))
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var handles = await QuerySelectorAllAsync(selector).ConfigureAwait(false);

            var elements = await EvaluateFunctionHandleAsync(
                @"(_, ...elements) => {
                    return elements;
                }",
                handles).ConfigureAwait(false) as JSHandle;

            elements!.DisposeAction = async () =>
            {
                // We can't use Task.WhenAll with ValueTask :(
                foreach (var item in handles)
                {
                    await item.DisposeAsync().ConfigureAwait(false);
                }
            };

            return elements;
        }

        /// <inheritdoc/>
        public Task<IElementHandle[]> XPathAsync(string expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.StartsWith("//", StringComparison.Ordinal))
            {
                expression = $".{expression}";
            }

            return BindIsolatedHandleAsync<IElementHandle[], ElementHandle>(handle => handle.QuerySelectorAllAsync($"xpath/{expression}"));
        }

        /// <inheritdoc/>
        public Task<BoundingBox> BoundingBoxAsync()
            => BindIsolatedHandleAsync<BoundingBox, ElementHandle>(async handle =>
            {
                var box = await handle.EvaluateFunctionAsync<BoundingBox>(@"element => {
                    if (!(element instanceof Element)) {
                        return null;
                    }
                    // Element is not visible.
                    if (element.getClientRects().length === 0) {
                        return null;
                    }
                    const rect = element.getBoundingClientRect();
                    return {x: rect.x, y: rect.y, width: rect.width, height: rect.height};
                }").ConfigureAwait(false);

                if (box == null)
                {
                    return null;
                }

                var offset = await handle.GetTopLeftCornerOfFrameAsync().ConfigureAwait(false);

                if (offset == null)
                {
                    return null;
                }

                return new BoundingBox()
                {
                    X = box.X + offset.Value.X,
                    Y = box.Y + offset.Value.Y,
                    Height = box.Height,
                    Width = box.Width,
                };
            });

        /// <inheritdoc/>
        public Task<BoxModel> BoxModelAsync()
            => BindIsolatedHandleAsync<BoxModel, ElementHandle>(async handle =>
            {
                var model = await handle.EvaluateFunctionAsync<BoxModel>(@"element => {
                    if (!(element instanceof Element)) {
                        return null;
                    }
                    // Element is not visible.
                    if (element.getClientRects().length === 0) {
                        return null;
                    }
                    const rect = element.getBoundingClientRect();
                    const style = window.getComputedStyle(element);
                    const offsets = {
                        padding: {
                          left: parseInt(style.paddingLeft, 10),
                          top: parseInt(style.paddingTop, 10),
                          right: parseInt(style.paddingRight, 10),
                          bottom: parseInt(style.paddingBottom, 10),
                        },
                        margin: {
                          left: -parseInt(style.marginLeft, 10),
                          top: -parseInt(style.marginTop, 10),
                          right: -parseInt(style.marginRight, 10),
                          bottom: -parseInt(style.marginBottom, 10),
                        },
                        border: {
                          left: parseInt(style.borderLeft, 10),
                          top: parseInt(style.borderTop, 10),
                          right: parseInt(style.borderRight, 10),
                          bottom: parseInt(style.borderBottom, 10),
                        },
                      };
                  const border = [
                    {x: rect.left, y: rect.top},
                    {x: rect.left + rect.width, y: rect.top},
                    {x: rect.left + rect.width, y: rect.top + rect.bottom},
                    {x: rect.left, y: rect.top + rect.bottom},
                  ];
                  const padding = transformQuadWithOffsets(border, offsets.border);
                  const content = transformQuadWithOffsets(padding, offsets.padding);
                  const margin = transformQuadWithOffsets(border, offsets.margin);
                  return {
                    content,
                    padding,
                    border,
                    margin,
                    width: rect.width,
                    height: rect.height,
                  };

                  function transformQuadWithOffsets(
                    quad,
                    offsets
                  ) {
                    return [
                      {
                        x: quad[0].x + offsets.left,
                        y: quad[0].y + offsets.top,
                      },
                      {
                        x: quad[1].x - offsets.right,
                        y: quad[1].y + offsets.top,
                      },
                      {
                        x: quad[2].x - offsets.right,
                        y: quad[2].y - offsets.bottom,
                      },
                      {
                        x: quad[3].x + offsets.left,
                        y: quad[3].y - offsets.bottom,
                      },
                    ];
                  }
                }").ConfigureAwait(false);

                if (model == null)
                {
                    return null;
                }

                var offset = await handle.GetTopLeftCornerOfFrameAsync().ConfigureAwait(false);

                if (offset == null)
                {
                    return null;
                }

                foreach (var point in model.Content)
                {
                    point.X += offset.Value.X;
                    point.Y += offset.Value.Y;
                }

                foreach (var point in model.Padding)
                {
                    point.X += offset.Value.X;
                    point.Y += offset.Value.Y;
                }

                foreach (var point in model.Border)
                {
                    point.X += offset.Value.X;
                    point.Y += offset.Value.Y;
                }

                foreach (var point in model.Margin)
                {
                    point.X += offset.Value.X;
                    point.Y += offset.Value.Y;
                }

                return model;
            });

        /// <inheritdoc/>
        public abstract Task<IFrame> ContentFrameAsync();

        /// <inheritdoc/>
        public Task<bool> IsIntersectingViewportAsync(decimal threshold)
            => BindIsolatedHandleAsync<bool, ElementHandle>(async handle =>
            {
                await handle.AssertConnectedElementAsync().ConfigureAwait(false);
                var svgHandle = await AsSVGElementHandleAsync(this).ConfigureAwait(false);
                var target = svgHandle == null ? handle : await svgHandle.GetOwnerSVGElementAsync().ConfigureAwait(false);

                return await target.Realm.EvaluateFunctionAsync<bool>(
                    @"async (element, threshold) => {
                        const visibleRatio = await new Promise(resolve => {
                            const observer = new IntersectionObserver(entries => {
                                resolve(entries[0].intersectionRatio);
                                observer.disconnect();
                            });
                            observer.observe(element);
                        });
                        return threshold === 1 ? visibleRatio === 1 : visibleRatio > threshold;
                    }",
                    target,
                    threshold).ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(params string[] values)
            => BindIsolatedHandleAsync<string[], ElementHandle>(handle => handle.EvaluateFunctionAsync<string[]>(
                @"(element, values) =>
                    {
                        if (element.nodeName.toLowerCase() !== 'select')
                            throw new Error('Element is not a <select> element.');

                        const options = Array.from(element.options);
                        element.value = undefined;
                        for (const option of options) {
                            option.selected = values.includes(option.value);
                            if (option.selected && !element.multiple)
                                break;
                        }
                        element.dispatchEvent(new Event('input', { 'bubbles': true }));
                        element.dispatchEvent(new Event('change', { 'bubbles': true }));
                        return options.filter(option => option.selected).map(option => option.value);
                    }",
                new object[] { values }));

        /// <inheritdoc/>
        public Task<DragData> DragAsync(decimal x, decimal y)
            => BindIsolatedHandleAsync<DragData, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);

#pragma warning disable CS0618 // Type or member is obsolete
                if (Page.IsDragInterceptionEnabled)
                {
                    var start = await handle.ClickablePointAsync().ConfigureAwait(false);
                    return await Page.Mouse.DragAsync(start.X, start.Y, x, y).ConfigureAwait(false);
                }
#pragma warning restore CS0618 // Type or member is obsolete

                try
                {
                    if (!Page.IsDragging)
                    {
                        Page.IsDragging = true;
                        await handle.HoverAsync().ConfigureAwait(false);
                        await Page.Mouse.DownAsync().ConfigureAwait(false);
                        await Page.Mouse.MoveAsync(x, y).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Page.IsDragging = false;
                    throw new PuppeteerException("Failed to process drag.", ex);
                }

                return null;
            });

        /// <inheritdoc/>
        public Task<DragData> DragAsync(IElementHandle target)
            => BindIsolatedHandleAsync<DragData, ElementHandle>(async handle =>
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(target), "Target cannot be null");
                }

                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);

                if (Page.IsDragInterceptionEnabled)
                {
                    var start = await handle.ClickablePointAsync().ConfigureAwait(false);
                    var targetPoint = await target.ClickablePointAsync().ConfigureAwait(false);
                    return await Page.Mouse.DragAsync(start.X, start.Y, targetPoint.X, targetPoint.Y)
                        .ConfigureAwait(false);
                }

                try
                {
                    if (!Page.IsDragging)
                    {
                        Page.IsDragging = true;
                        await handle.HoverAsync().ConfigureAwait(false);
                        await Page.Mouse.DownAsync().ConfigureAwait(false);
                        await target.HoverAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Page.IsDragging = false;
                    throw new PuppeteerException("Failed to process drag.", ex);
                }

                return null;
            });

        /// <inheritdoc/>
        public Task DragEnterAsync(DragData data)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.DragEnterAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task DragOverAsync(DragData data)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.DragOverAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task DropAsync(DragData data)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.DropAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public async Task DropAsync(IElementHandle target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
#pragma warning disable CS0618 // Type or member is obsolete
            await target.DragAsync(this).ConfigureAwait(false);
#pragma warning restore CS0618 // Type or member is obsolete
            Page.IsDragging = false;
            await Page.Mouse.UpAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task DragAndDropAsync(IElementHandle target, int delay = 0)
            => BindIsolatedHandleAsync<IElementHandle, ElementHandle>(async handle =>
            {
                if (target == null)
                {
                    throw new ArgumentException("Target cannot be null", nameof(target));
                }

                if (!Page.IsDragInterceptionEnabled)
                {
                    throw new PuppeteerException("Drag Interception is not enabled!");
                }

                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                var targetPoint = await target.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse
                    .DragAndDropAsync(clickablePoint.X, clickablePoint.Y, targetPoint.X, targetPoint.Y, delay)
                    .ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
            => BindIsolatedHandleAsync<BoxModelPoint, ElementHandle>(async handle =>
            {
                var box = await handle.ClickableBoxAsync().ConfigureAwait(false) ?? throw new PuppeteerException("Node is either not clickable or not an Element");

                if (offset != null)
                {
                    return new BoxModelPoint() { X = box.X + offset.Value.X, Y = box.Y + offset.Value.Y, };
                }

                return new BoxModelPoint() { X = box.X + (box.Width / 2), Y = box.Y + (box.Height / 2), };
            });

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync()
            => BindIsolatedHandleAsync<bool, ElementHandle>(handle => CheckVisibilityAsync(handle, true));

        /// <inheritdoc/>
        public Task<bool> IsHiddenAsync()
            => BindIsolatedHandleAsync<bool, ElementHandle>(handle => CheckVisibilityAsync(handle, false));

        /// <inheritdoc/>
        public override Task<IJSHandle> GetPropertyAsync(string propertyName)
            => BindIsolatedHandleAsync<IJSHandle, ElementHandle>(element => element.Handle.GetPropertyAsync(propertyName));

        /// <inheritdoc/>
        public override Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
            => BindIsolatedHandleAsync<Dictionary<string, IJSHandle>, ElementHandle>(element => element.Handle.GetPropertiesAsync());

        /// <inheritdoc/>
        public override Task<T> JsonValueAsync<T>()
            => BindIsolatedHandleAsync<T, ElementHandle>(element => element.Handle.JsonValueAsync<T>());

        /// <inheritdoc/>
        public virtual Task ScrollIntoViewAsync()
            => BindIsolatedHandleAsync<JToken, ElementHandle>(handle
                => handle.EvaluateFunctionAsync(
                        @"element => {
                            element.scrollIntoView({
                                block: 'center',
                                inline: 'center',
                                behavior: 'instant',
                            });
                        }"));

        /// <summary>
        /// Checks whether the element is still connected to the browser.
        /// </summary>
        /// <exception cref="PuppeteerException">The exception if the element is not connected.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task AssertConnectedElementAsync()
        {
            var error = await EvaluateFunctionAsync<string>(@"element => {
                if (!element.isConnected) {
                    return 'Node is detached from document';
                }

                if (element.nodeType !== Node.ELEMENT_NODE) {
                  return 'Node is not of type HTMLElement';
                }
                return;
            }").ConfigureAwait(false);

            if (!string.IsNullOrEmpty(error))
            {
                throw new PuppeteerException(error);
            }
        }

        /// <summary>
        /// Executes a function in the isolated context.
        /// </summary>
        /// <param name="action">Function to execute.</param>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <typeparam name="TElementHandle">Element handle type.</typeparam>
        /// <returns>Task which resolves to the result of the function.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <c>null</c>.</exception>
        protected async Task<TResult> BindIsolatedHandleAsync<TResult, TElementHandle>(Func<TElementHandle, Task<TResult>> action)
            where TElementHandle : ElementHandle
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (Realm == Frame.IsolatedRealm)
            {
                return await action((TElementHandle)this).ConfigureAwait(false);
            }

            var adoptedThis = await Frame.IsolatedRealm.AdoptHandleAsync(this).ConfigureAwait(false) as ElementHandle;
            var result = await action((TElementHandle)adoptedThis).ConfigureAwait(false);

            if (result is IJSHandle jsHandleResult)
            {
                // If the function returns `adoptedThis`, then we return `this` and T is a IJSHandle.
                if (jsHandleResult == adoptedThis)
                {
                    return (TResult)(object)this;
                }

                return (TResult)await Realm.TransferHandleAsync(jsHandleResult).ConfigureAwait(false);
            }

            // If the function returns an array of handlers, transfer them into the current realm.
            // Dynamic arrays using generics can be hard to translate
            if (typeof(TResult).IsArray && result is IEnumerable enumerable)
            {
                var resultArray = new List<object>();

                foreach (var item in enumerable)
                {
                    if (item is IJSHandle jsHandle)
                    {
                        resultArray.Add(await Realm.TransferHandleAsync(jsHandle).ConfigureAwait(false));
                    }
                    else
                    {
                        resultArray.Add(item);
                    }
                }

                var elementType = typeof(TResult).GetElementType();
                if (elementType != null)
                {
                    var output = Array.CreateInstance(elementType, resultArray.Count);

                    for (var i = 0; i < resultArray.Count; i++)
                    {
                        // You can use Convert.ChangeType to convert values to the desired type
                        output.SetValue(resultArray[i], i);
                    }

                    return (TResult)(object)output;
                }
            }

            if (result is not IDictionary<string, IJSHandle> dictionaryResult)
            {
                return result;
            }

            foreach (var key in dictionaryResult.Keys)
            {
                dictionaryResult[key] = await Realm.TransferHandleAsync(dictionaryResult[key]).ConfigureAwait(false);
            }

            return result;
        }

        private async Task<BoundingBox> NonEmptyVisibleBoundingBoxAsync()
        {
            var box = await BoundingBoxAsync().ConfigureAwait(false) ??
                      throw new PuppeteerException("Node is either not visible or not an HTMLElement");

            if (box.Width == 0)
            {
                throw new PuppeteerException("Node has 0 width.");
            }

            if (box.Height == 0)
            {
                throw new PuppeteerException("Node has 0 height.");
            }

            return box;
        }

        private async Task<ElementHandle> AsSVGElementHandleAsync(ElementHandle elementHandle)
        {
            if (await elementHandle.EvaluateFunctionAsync<bool>(@"element => element instanceof SVGElement").ConfigureAwait(false))
            {
                return elementHandle;
            }

            return null;
        }

        private async Task<ElementHandle> GetOwnerSVGElementAsync()
            => await EvaluateFunctionHandleAsync(@"element => {
                if (element instanceof SVGSVGElement) {
                    return element;
                }

                return element.ownerSVGElement;
            }").ConfigureAwait(false) as ElementHandle;

        private async Task ScrollIntoViewIfNeededAsync()
        {
            if (await IsIntersectingViewportAsync(1).ConfigureAwait(false))
            {
                return;
            }

            await ScrollIntoViewAsync().ConfigureAwait(false);
        }

        private Task<bool> CheckVisibilityAsync(IElementHandle handle, bool visibility)
            => handle.EvaluateFunctionAsync<bool>(
                @"async (element, PuppeteerUtil, visibility) =>
                {
                    return Boolean(PuppeteerUtil.checkVisibility(element, visibility));
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                visibility);

        private async Task<Point?> GetTopLeftCornerOfFrameAsync()
        {
            var point = default(Point);

            var frame = Frame;
            var parentFrame = frame.ParentFrame;

            while (parentFrame != null)
            {
                var handle = await frame.FrameElementAsync().ConfigureAwait(false);
                if (handle == null)
                {
                    throw new PuppeteerException("Unsupported frame type");
                }

                var parentBox = await handle.EvaluateFunctionAsync<Point?>(@"element => {
                    // Element is not visible.
                    if (element.getClientRects().length === 0) {
                        return null;
                    }
                    const rect = element.getBoundingClientRect();
                    const style = window.getComputedStyle(element);
                    return {
                        x:
                        rect.left +
                            parseInt(style.paddingLeft, 10) +
                            parseInt(style.borderLeftWidth, 10),
                        y:
                        rect.top +
                            parseInt(style.paddingTop, 10) +
                            parseInt(style.borderTopWidth, 10),
                    };
                }").ConfigureAwait(false);

                if (parentBox == null)
                {
                    return null;
                }

                point.X += parentBox.Value.X;
                point.Y += parentBox.Value.Y;
                frame = parentFrame;
                parentFrame = frame.ParentFrame;
            }

            return point;
        }

        private async Task<BoundingBox> ClickableBoxAsync()
        {
            var boxes = await this.EvaluateFunctionAsync<BoundingBox[]>(@"element => {
                if (!(element instanceof Element)) {
                    return null;
                }
                return [...element.getClientRects()].map(rect => {
                    return {x: rect.x, y: rect.y, width: rect.width, height: rect.height};
                });
            }").ConfigureAwait(false);

            if (boxes == null || boxes.Length == 0)
            {
                return null;
            }

            await IntersectBoundingBoxesWithFrameAsync(boxes).ConfigureAwait(false);

            var frame = Frame;
            var parentFrame = frame.ParentFrame;
            while (parentFrame != null)
            {
                var handle = await frame.FrameElementAsync().ConfigureAwait(false);
                if (handle == null)
                {
                    throw new PuppeteerException("Unsupported frame type");
                }

                var parentBox = await handle.EvaluateFunctionAsync<BoundingBox>(@"element => {
                    // Element is not visible.
                    if (element.getClientRects().length === 0) {
                        return null;
                    }
                    const rect = element.getBoundingClientRect();
                    const style = window.getComputedStyle(element);
                    return {
                        X:
                        rect.left +
                            parseInt(style.paddingLeft, 10) +
                            parseInt(style.borderLeftWidth, 10),
                        Y:
                        rect.top +
                            parseInt(style.paddingTop, 10) +
                            parseInt(style.borderTopWidth, 10),
                    };
                }").ConfigureAwait(false);

                if (parentBox == null)
                {
                    return null;
                }

                foreach (var box in boxes)
                {
                    box.X += parentBox.X;
                    box.Y += parentBox.Y;
                }

                await handle.IntersectBoundingBoxesWithFrameAsync(boxes).ConfigureAwait(false);
                frame = parentFrame;
                parentFrame = frame.ParentFrame;
            }

            var resultBox = boxes.FirstOrDefault(box => box.Width >= 1 && box.Height >= 1);

            return resultBox;
        }

        private async Task IntersectBoundingBoxesWithFrameAsync(BoundingBox[] boxes)
        {
            var documentBox = await Frame
                .IsolatedRealm
                .EvaluateFunctionAsync<BoundingBox>(@"() => {
                    return {
                        width: document.documentElement.clientWidth,
                        height: document.documentElement.clientHeight,
                    };
                }").ConfigureAwait(false);

            foreach (var box in boxes)
            {
                IntersectBoundingBox(box, documentBox.Width, documentBox.Height);
            }
        }

        private void IntersectBoundingBox(BoundingBox box, decimal width, decimal height)
        {
            box.Width = Math.Max(
                box.X >= 0
                    ? Math.Min(width - box.X, box.Width)
                    : Math.Min(width, box.Width + box.X),
                0);
            box.Height = Math.Max(
                box.Y >= 0
                    ? Math.Min(height - box.Y, box.Height)
                    : Math.Min(height, box.Height + box.Y),
                0);
        }
    }
}
