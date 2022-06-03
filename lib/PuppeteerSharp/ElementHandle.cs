using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Inherits from <see cref="JSHandle"/>. It represents an in-page DOM element.
    /// ElementHandles can be created by <see cref="PuppeteerSharp.IPage.QuerySelectorAsync(string)"/> or <see cref="PuppeteerSharp.IPage.QuerySelectorAllAsync(string)"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ElementHandle : JSHandle, IElementHandle
    {
        private readonly FrameManager _frameManager;
        private readonly CustomQueriesManager _customQueriesManager;
        private readonly ILogger<ElementHandle> _logger;

        internal ElementHandle(
            ExecutionContext context,
            CDPSession client,
            RemoteObject remoteObject,
            IFrame frame) : base(context, client, remoteObject)
        {
            _frameManager = ((Frame)frame).FrameManager;
            _customQueriesManager = ((Browser)Page.Browser).CustomQueriesManager;
            _logger = client.LoggerFactory.CreateLogger<ElementHandle>();
        }

        internal Page Page => _frameManager.Page;

        private string DebuggerDisplay =>
            string.IsNullOrEmpty(RemoteObject.ClassName) ? ToString() : $"{RemoteObject.ClassName}@{RemoteObject.Description}";

        /// <inheritdoc/>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task ScreenshotAsync(string file, ScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.Type.HasValue)
            {
                options.Type = ScreenshotOptions.GetScreenshotTypeFromFile(file);
            }

            var data = await ScreenshotDataAsync(options).ConfigureAwait(false);

            using (var fs = AsyncFileHelper.CreateStream(file, FileMode.Create))
            {
                await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <inheritdoc/>
        public async Task<string> ScreenshotBase64Async(ScreenshotOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var needsViewportReset = false;
            var boundingBox = await BoundingBoxAsync().ConfigureAwait(false);

            if (boundingBox == null)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

            var viewport = Page.Viewport;
            if (viewport != null && (boundingBox.Width > viewport.Width || boundingBox.Height > viewport.Height))
            {
                var newRawViewport = JObject.FromObject(viewport);
                newRawViewport.Merge(new ViewPortOptions
                {
                    Width = (int)Math.Max(viewport.Width, Math.Ceiling(boundingBox.Width)),
                    Height = (int)Math.Max(viewport.Height, Math.Ceiling(boundingBox.Height))
                });
                await Page.SetViewportAsync(newRawViewport.ToObject<ViewPortOptions>(true)).ConfigureAwait(false);
                needsViewportReset = true;
            }
            await ExecutionContext.EvaluateFunctionAsync(
                @"function(element) {
                    element.scrollIntoView({ block: 'center', inline: 'center', behavior: 'instant'});
                }",
                this).ConfigureAwait(false);

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            boundingBox = await BoundingBoxAsync().ConfigureAwait(false);

            if (boundingBox == null)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }
            if (boundingBox.Width == 0)
            {
                throw new PuppeteerException("Node has 0 width.");
            }
            if (boundingBox.Height == 0)
            {
                throw new PuppeteerException("Node has 0 height.");
            }
            var getLayoutMetricsResponse = await Client.SendAsync<GetLayoutMetricsResponse>("Page.getLayoutMetrics").ConfigureAwait(false);

            var clip = boundingBox;
            clip.X += getLayoutMetricsResponse.LayoutViewport.PageX;
            clip.Y += getLayoutMetricsResponse.LayoutViewport.PageY;

            options.Clip = boundingBox.ToClip();
            var imageData = await Page.ScreenshotBase64Async(options).ConfigureAwait(false);

            if (needsViewportReset)
            {
                await Page.SetViewportAsync(viewport).ConfigureAwait(false);
            }

            return imageData;
        }

        /// <inheritdoc/>
        public async Task HoverAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.MoveAsync(point.X, point.Y).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task ClickAsync(ClickOptions options = null)
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.ClickAsync(point.X, point.Y, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task UploadFileAsync(params string[] filePaths) => UploadFileAsync(true, filePaths);

        /// <inheritdoc/>
        public async Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
        {
            var isMultiple = await EvaluateFunctionAsync<bool>("element => element.multiple").ConfigureAwait(false);

            if (!isMultiple && filePaths.Length > 1)
            {
                throw new PuppeteerException("Multiple file uploads only work with <input type=file multiple>");
            }

            var objectId = RemoteObject.ObjectId;
            var node = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = RemoteObject.ObjectId
            }).ConfigureAwait(false);
            var backendNodeId = node.Node.BackendNodeId;

            if (!filePaths.Any() || filePaths == null)
            {
                await EvaluateFunctionAsync(@"(element) => {
                    element.files = new DataTransfer().files;

                    // Dispatch events for this case because it should behave akin to a user action.
                    element.dispatchEvent(new Event('input', { bubbles: true }));
                    element.dispatchEvent(new Event('change', { bubbles: true }));
                }").ConfigureAwait(false);
            }
            else
            {
                var files = resolveFilePaths ? filePaths.Select(Path.GetFullPath).ToArray() : filePaths;
                CheckForFileAccess(files);
                await Client.SendAsync("DOM.setFileInputFiles", new DomSetFileInputFilesRequest
                {
                    ObjectId = objectId,
                    Files = files,
                    BackendNodeId = backendNodeId
                }).ConfigureAwait(false);
            }
        }

        private void CheckForFileAccess(string[] files)
        {
            foreach (var file in files)
            {
                try
                {
                    File.Open(file, FileMode.Open).Dispose();
                }
                catch (Exception ex)
                {
                    throw new PuppeteerException($"{files} does not exist or is not readable", ex);
                }
            }
        }

        /// <inheritdoc/>
        public async Task TapAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Touchscreen.TapAsync(point.X, point.Y).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task FocusAsync() => EvaluateFunctionAsync("element => element.focus()");

        /// <inheritdoc/>
        public async Task TypeAsync(string text, TypeOptions options = null)
        {
            await FocusAsync().ConfigureAwait(false);
            await Page.Keyboard.TypeAsync(text, options).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task PressAsync(string key, PressOptions options = null)
        {
            await FocusAsync().ConfigureAwait(false);
            await Page.Keyboard.PressAsync(key, options).ConfigureAwait(false);
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
        {
            var (updatedSelector, queryHandler) = _customQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.QueryOne(this, updatedSelector);
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var (updatedSelector, queryHandler) = _customQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.QueryAll(this, updatedSelector);
        }

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{IJSHandle}, string, object[])"/>
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to a <see cref="IJSHandle"/> of <c>document.querySelectorAll</c> result</returns>
        public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var (updatedSelector, queryHandler) = _customQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.QueryAllArray(this, updatedSelector);
        }

        /// <inheritdoc/>
        public async Task<IElementHandle[]> XPathAsync(string expression)
        {
            var arrayHandle = await ExecutionContext.EvaluateFunctionHandleAsync(
                @"(element, expression) => {
                    const document = element.ownerDocument || element;
                    const iterator = document.evaluate(expression, element, null, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
                    const array = [];
                    let item;
                    while ((item = iterator.iterateNext()))
                        array.push(item);
                    return array;
                }",
                this,
                expression).ConfigureAwait(false);
            var properties = await arrayHandle.GetPropertiesAsync().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.Values.OfType<IElementHandle>().ToArray();
        }

        /// <inheritdoc/>
        public async Task<BoundingBox> BoundingBoxAsync()
        {
            var result = await GetBoxModelAsync().ConfigureAwait(false);

            if (result == null)
            {
                return null;
            }

            var quad = result.Model.Border;

            var x = new[] { quad[0], quad[2], quad[4], quad[6] }.Min();
            var y = new[] { quad[1], quad[3], quad[5], quad[7] }.Min();
            var width = new[] { quad[0], quad[2], quad[4], quad[6] }.Max() - x;
            var height = new[] { quad[1], quad[3], quad[5], quad[7] }.Max() - y;

            return new BoundingBox(x, y, width, height);
        }

        /// <inheritdoc/>
        public async Task<BoxModel> BoxModelAsync()
        {
            var result = await GetBoxModelAsync().ConfigureAwait(false);

            return result == null
                ? null
                : new BoxModel
                {
                    Content = FromProtocolQuad(result.Model.Content),
                    Padding = FromProtocolQuad(result.Model.Padding),
                    Border = FromProtocolQuad(result.Model.Border),
                    Margin = FromProtocolQuad(result.Model.Margin),
                    Width = result.Model.Width,
                    Height = result.Model.Height
                };
        }

        /// <inheritdoc/>
        public async Task<IFrame> ContentFrameAsync()
        {
            var nodeInfo = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = RemoteObject.ObjectId
            }).ConfigureAwait(false);

            return string.IsNullOrEmpty(nodeInfo.Node.FrameId) ? null : await _frameManager.GetFrameAsync(nodeInfo.Node.FrameId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsIntersectingViewportAsync()
            => ExecutionContext.EvaluateFunctionAsync<bool>(
                @"async element =>
                {
                    const visibleRatio = await new Promise(resolve =>
                    {
                        const observer = new IntersectionObserver(entries =>
                        {
                            resolve(entries[0].intersectionRatio);
                            observer.disconnect();
                        });
                        observer.observe(element);
                    });
                    return visibleRatio > 0;
                }",
                this);

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(params string[] values)
            => EvaluateFunctionAsync<string[]>(
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
                new[] { values });

        /// <inheritdoc/>
        public async Task<DragData> DragAsync(decimal x, decimal y)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var start = await ClickablePointAsync().ConfigureAwait(false);
            return await Page.Mouse.DragAsync(start.X, start.Y, x, y).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DragEnterAsync(DragData data)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragEnterAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DragOverAsync(DragData data)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragOverAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DropAsync(DragData data)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DropAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DragAndDropAsync(IElementHandle target, int delay = 0)
        {
            if (target == null)
            {
                throw new ArgumentException("Target cannot be null", nameof(target));
            }

            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            var targetPoint = await target.ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragAndDropAsync(point.X, point.Y, targetPoint.X, targetPoint.Y, delay).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Point> ClickablePointAsync()
        {
            GetContentQuadsResponse result = null;

            var contentQuadsTask = Client.SendAsync<GetContentQuadsResponse>("DOM.getContentQuads", new DomGetContentQuadsRequest
            {
                ObjectId = RemoteObject.ObjectId
            });
            var layoutTask = Client.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics");

            try
            {
                await Task.WhenAll(contentQuadsTask, layoutTask).ConfigureAwait(false);
                result = contentQuadsTask.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get content quads");
            }

            if (result == null || result.Quads.Length == 0)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

            // Filter out quads that have too small area to click into.
            var quads = result.Quads
                .Select(FromProtocolQuad)
                .Select(q => IntersectQuadWithViewport(q, layoutTask.Result))
                .Where(q => ComputeQuadArea(q.ToArray()) > 1);

            if (!quads.Any())
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

            // Return the middle point of the first quad.
            var quad = quads.First();
            var x = 0m;
            var y = 0m;

            foreach (var point in quad)
            {
                x += point.X;
                y += point.Y;
            }

            return new Point { X = x / 4, Y = y / 4 };
        }

        /// <inheritdoc/>
        public async Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            var frame = (Frame)ExecutionContext.Frame;
            var secondaryContext = await frame.SecondaryWorld.GetExecutionContextAsync().ConfigureAwait(false);
            var adoptedRoot = await secondaryContext.AdoptElementHandleAsync(this).ConfigureAwait(false);
            options ??= new WaitForSelectorOptions();
            options.Root = adoptedRoot;

            var handle = await frame.SecondaryWorld.WaitForSelectorAsync(selector, options).ConfigureAwait(false);
            await adoptedRoot.DisposeAsync().ConfigureAwait(false);
            if (handle == null)
            {
                return null;
            }
            var mainExecutionContext = await frame.MainWorld.GetExecutionContextAsync().ConfigureAwait(false);
            var result = await mainExecutionContext.AdoptElementHandleAsync(handle).ConfigureAwait(false);
            await handle.DisposeAsync().ConfigureAwait(false);
            return result;
        }

        private IEnumerable<BoxModelPoint> IntersectQuadWithViewport(IEnumerable<BoxModelPoint> quad, PageGetLayoutMetricsResponse viewport)
            => quad.Select(point => new BoxModelPoint
            {
                X = Math.Min(Math.Max(point.X, 0), viewport.ContentSize.Width),
                Y = Math.Min(Math.Max(point.Y, 0), viewport.ContentSize.Height),
            });

        private async Task ScrollIntoViewIfNeededAsync()
        {
            var errorMessage = await EvaluateFunctionAsync<string>(
                @"async(element, pageJavascriptEnabled) => {
                    if (!element.isConnected)
                        return 'Node is detached from document';
                    if (element.nodeType !== Node.ELEMENT_NODE)
                        return 'Node is not of type HTMLElement';
                    // force-scroll if page's javascript is disabled.
                    if (!pageJavascriptEnabled) {
                        element.scrollIntoView({block: 'center', inline: 'center', behavior: 'instant'});
                        return null;
                    }
                    const visibleRatio = await new Promise(resolve => {
                    const observer = new IntersectionObserver(entries => {
                        resolve(entries[0].intersectionRatio);
                        observer.disconnect();
                    });
                    observer.observe(element);
                    });
                    if (visibleRatio !== 1.0)
                        element.scrollIntoView({block: 'center', inline: 'center', behavior: 'instant'});
                    return null;
                }",
                Page.JavascriptEnabled).ConfigureAwait(false);

            if (errorMessage != null)
            {
                throw new PuppeteerException(errorMessage);
            }
        }

        private async Task<BoxModelResponse> GetBoxModelAsync()
        {
            try
            {
                return await Client.SendAsync<BoxModelResponse>("DOM.getBoxModel", new DomGetBoxModelRequest
                {
                    ObjectId = RemoteObject.ObjectId
                }).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        private BoxModelPoint[] FromProtocolQuad(decimal[] quad) => new[]
        {
            new BoxModelPoint { X = quad[0], Y = quad[1] },
            new BoxModelPoint { X = quad[2], Y = quad[3] },
            new BoxModelPoint { X = quad[4], Y = quad[5] },
            new BoxModelPoint { X = quad[6], Y = quad[7] }
        };

        private decimal ComputeQuadArea(BoxModelPoint[] quad)
        {
            var area = 0m;
            for (var i = 0; i < quad.Length; ++i)
            {
                var p1 = quad[i];
                var p2 = quad[(i + 1) % quad.Length];
                area += ((p1.X * p2.Y) - (p2.X * p1.Y)) / 2;
            }
            return Math.Abs(area);
        }
    }
}
