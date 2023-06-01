using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <inheritdoc/>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ElementHandle : JSHandle, IElementHandle
    {
        private readonly FrameManager _frameManager;
        private readonly ILogger<ElementHandle> _logger;

        internal ElementHandle(
            ExecutionContext context,
            CDPSession client,
            RemoteObject remoteObject,
            IFrame frame,
            IPage page,
            FrameManager frameManager) : base(context, client, remoteObject)
        {
            Page = page;
            Frame = frame as Frame;
            _frameManager = frameManager;
            _logger = client.LoggerFactory.CreateLogger<ElementHandle>();
        }

        internal IPage Page { get; }

        internal Frame Frame { get; }

        internal CustomQueriesManager CustomQueriesManager => ((Browser)Page.Browser).CustomQueriesManager;

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
        public async Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
        {
            var customQueriesManager = ((Browser)Frame.FrameManager.Page.Browser).CustomQueriesManager;
            var (updatedSelector, queryHandler) = customQueriesManager.GetQueryHandlerAndSelector(selector);
            return await queryHandler.WaitFor(null, this, updatedSelector, options).ConfigureAwait(false);
        }

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
                newRawViewport["Width"] = (int)Math.Max(viewport.Width, Math.Ceiling(boundingBox.Width));
                newRawViewport["Height"] = (int)Math.Max(viewport.Height, Math.Ceiling(boundingBox.Height));
                await Page.SetViewportAsync(newRawViewport.ToObject<ViewPortOptions>(true)).ConfigureAwait(false);
                needsViewportReset = true;
            }

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

            var getLayoutMetricsResponse = await Client.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics").ConfigureAwait(false);

            var clip = boundingBox;
            var metricsViewport = getLayoutMetricsResponse.CssVisualViewport ?? getLayoutMetricsResponse.LayoutViewport;
            clip.X += metricsViewport.PageX;
            clip.Y += metricsViewport.PageY;

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
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.MoveAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task ClickAsync(ClickOptions options = null)
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var clickablePoint = await ClickablePointAsync(options?.OffSet).ConfigureAwait(false);
            await Page.Mouse.ClickAsync(clickablePoint.X, clickablePoint.Y, options).ConfigureAwait(false);
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
                ObjectId = RemoteObject.ObjectId,
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
                    BackendNodeId = backendNodeId,
                }).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task TapAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Touchscreen.TapAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
        {
            var (updatedSelector, queryHandler) = CustomQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.QueryOne(this, updatedSelector);
        }

        /// <inheritdoc/>
        public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var (updatedSelector, queryHandler) = CustomQueriesManager.GetQueryHandlerAndSelector(selector);
            return queryHandler.QueryAll(this, updatedSelector);
        }

        /// <inheritdoc/>
        public async Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
        {
            var (updatedSelector, queryHandler) = CustomQueriesManager.GetQueryHandlerAndSelector(selector);
            var handles = await queryHandler.QueryAll(this, updatedSelector).ConfigureAwait(false);

            var elements = await EvaluateFunctionHandleAsync(
                @"(_, ...elements) => {
                    return elements;
                }",
                handles).ConfigureAwait(false) as JSHandle;

            elements.DisposeAction = async () =>
            {
                // We can't use Task.WhenAll with ValueTask :(
                foreach (var handle in handles)
                {
                    await handle.DisposeAsync().ConfigureAwait(false);
                }
            };

            return elements;
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

            var (offsetX, offsetY) = await GetOOPIFOffsetsAsync(Frame).ConfigureAwait(false);
            var quad = result.Model.Border;

            var x = new[] { quad[0], quad[2], quad[4], quad[6] }.Min();
            var y = new[] { quad[1], quad[3], quad[5], quad[7] }.Min();
            var width = new[] { quad[0], quad[2], quad[4], quad[6] }.Max() - x;
            var height = new[] { quad[1], quad[3], quad[5], quad[7] }.Max() - y;

            return new BoundingBox(x + offsetX, y + offsetY, width, height);
        }

        /// <inheritdoc/>
        public async Task<BoxModel> BoxModelAsync()
        {
            var result = await GetBoxModelAsync().ConfigureAwait(false);
            var (offsetX, offsetY) = await GetOOPIFOffsetsAsync(Frame).ConfigureAwait(false);

            return result == null
                ? null
                : new BoxModel
                {
                    Content = ApplyOffsetsToQuad(FromProtocolQuad(result.Model.Content), offsetX, offsetY).ToArray(),
                    Padding = ApplyOffsetsToQuad(FromProtocolQuad(result.Model.Padding), offsetX, offsetY).ToArray(),
                    Border = ApplyOffsetsToQuad(FromProtocolQuad(result.Model.Border), offsetX, offsetY).ToArray(),
                    Margin = ApplyOffsetsToQuad(FromProtocolQuad(result.Model.Margin), offsetX, offsetY).ToArray(),
                    Width = result.Model.Width,
                    Height = result.Model.Height,
                };
        }

        /// <inheritdoc/>
        public async Task<IFrame> ContentFrameAsync()
        {
            var nodeInfo = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = RemoteObject.ObjectId,
            }).ConfigureAwait(false);

            return string.IsNullOrEmpty(nodeInfo.Node.FrameId) ? null : await _frameManager.FrameTree.GetFrameAsync(nodeInfo.Node.FrameId).ConfigureAwait(false);
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
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragEnterAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DragOverAsync(DragData data)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragOverAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DropAsync(DragData data)
        {
            if (!Page.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DropAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
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
            var clickablePoint = await ClickablePointAsync().ConfigureAwait(false);
            var targetPoint = await target.ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.DragAndDropAsync(clickablePoint.X, clickablePoint.Y, targetPoint.X, targetPoint.Y, delay).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
        {
            GetContentQuadsResponse result = null;

            var contentQuadsTask = Client.SendAsync<GetContentQuadsResponse>("DOM.getContentQuads", new DomGetContentQuadsRequest
            {
                ObjectId = RemoteObject.ObjectId,
            });
            var layoutTask = Page.Client.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics");

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

            var (offsetX, offsetY) = await GetOOPIFOffsetsAsync(Frame).ConfigureAwait(false);

            // Filter out quads that have too small area to click into.
            var quads = result.Quads
                .Select(FromProtocolQuad)
                .Select(quad => ApplyOffsetsToQuad(quad, offsetX, offsetY))
                .Select(q => IntersectQuadWithViewport(q, layoutTask.Result))
                .Where(q => ComputeQuadArea(q.ToArray()) > 1);

            if (!quads.Any())
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

            // Return the middle point of the first quad.
            var quad = quads.First();
            if (offset != null)
            {
                // Return the point of the first quad identified by offset.
                var minX = decimal.MaxValue;
                var minY = decimal.MaxValue;
                foreach (var point in quad)
                {
                    if (point.X < minX)
                    {
                        minX = point.X;
                    }

                    if (point.Y < minY)
                    {
                        minY = point.Y;
                    }
                }

                if (
                  minX != decimal.MaxValue &&
                  minY != decimal.MaxValue)
                {
                    return new BoxModelPoint()
                    {
                        X = minX + offset.Value.X,
                        Y = minY + offset.Value.Y,
                    };
                }
            }

            var x = 0m;
            var y = 0m;

            foreach (var point in quad)
            {
                x += point.X;
                y += point.Y;
            }

            return new BoxModelPoint()
            {
                X = x / 4,
                Y = y / 4,
            };
        }

        /// <summary>
        /// Scroll into view if needed.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        /// <exception cref="PuppeteerException">Puppeteer exception.</exception>
        public async Task ScrollIntoViewIfNeededAsync()
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
                ((Page)Page).JavascriptEnabled).ConfigureAwait(false);

            if (errorMessage != null)
            {
                throw new PuppeteerException(errorMessage);
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

        private IEnumerable<BoxModelPoint> ApplyOffsetsToQuad(BoxModelPoint[] quad, decimal offsetX, decimal offsetY)
            => quad.Select((part) => new BoxModelPoint() { X = part.X + offsetX, Y = part.Y + offsetY });

        private async Task<(decimal OffsetX, decimal OffsetY)> GetOOPIFOffsetsAsync(IFrame frame)
        {
            decimal offsetX = 0;
            decimal offsetY = 0;

            while (frame.ParentFrame != null)
            {
                var parent = (Frame)frame.ParentFrame;
                if (!frame.IsOopFrame)
                {
                    frame = parent;
                    continue;
                }

                var frameOwner = await parent.Client.SendAsync<DomGetFrameOwnerResponse>(
                        "DOM.getFrameOwner",
                        new DomGetFrameOwnerRequest
                        {
                            FrameId = frame.Id,
                        }).ConfigureAwait(false);

                var result = await parent.Client.SendAsync<DomGetBoxModelResponse>(
                    "DOM.getBoxModel",
                    new DomGetBoxModelRequest
                    {
                        BackendNodeId = frameOwner.BackendNodeId,
                    }).ConfigureAwait(false);

                if (result == null)
                {
                    break;
                }

                var contentBoxQuad = result.Model.Content;
                var topLeftCorner = FromProtocolQuad(contentBoxQuad)[0];
                offsetX += topLeftCorner.X;
                offsetY += topLeftCorner.Y;
                frame = parent;
            }

            return (offsetX, offsetY);
        }

        private IEnumerable<BoxModelPoint> IntersectQuadWithViewport(IEnumerable<BoxModelPoint> quad, PageGetLayoutMetricsResponse viewport)
        {
            var size = viewport.CssVisualViewport ?? viewport.LayoutViewport;
            return quad.Select(point => new BoxModelPoint
            {
                X = Math.Min(Math.Max(point.X, 0), size.ClientWidth),
                Y = Math.Min(Math.Max(point.Y, 0), size.ClientHeight),
            });
        }

        private async Task<DomGetBoxModelResponse> GetBoxModelAsync()
        {
            try
            {
                return await Client.SendAsync<DomGetBoxModelResponse>("DOM.getBoxModel", new DomGetBoxModelRequest
                {
                    ObjectId = RemoteObject.ObjectId,
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
            new BoxModelPoint { X = quad[6], Y = quad[7] },
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
