using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp
{
    /// <inheritdoc cref="PuppeteerSharp.IElementHandle" />
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ElementHandle : JSHandle, IElementHandle
    {
        private readonly ILogger<ElementHandle> _logger;

        internal ElementHandle(
            IsolatedWorld world,
            RemoteObject remoteObject) : base(world, remoteObject)
        {
            Handle = new JSHandle(world, remoteObject);
            _logger = world.Client.LoggerFactory.CreateLogger<ElementHandle>();
        }

        /// <inheritdoc/>
        IFrame IElementHandle.Frame => Frame;

        private JSHandle Handle { get; }

        private CustomQuerySelectorRegistry CustomQuerySelectorRegistry => Client.Connection.CustomQuerySelectorRegistry;

        private FrameManager FrameManager => Frame.FrameManager;

        private Page Page => Frame.FrameManager.Page;

        private string DebuggerDisplay =>
            string.IsNullOrEmpty(RemoteObject.ClassName) ? ToString() : $"{RemoteObject.ClassName}@{RemoteObject.Description}";

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
            return await BindIsolatedHandleAsync(handle => queryHandler.WaitForAsync(null, handle, updatedSelector, options)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<byte[]> ScreenshotDataAsync(ElementScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ElementScreenshotOptions());

        /// <inheritdoc/>
        public Task<string> ScreenshotBase64Async(ElementScreenshotOptions options)
            => BindIsolatedHandleAsync(async handle =>
            {
                if (options == null)
                {
                    throw new ArgumentNullException(nameof(options));
                }

                var clip = await handle.NonEmptyVisibleBoundingBoxAsync().ConfigureAwait(false);
                var page = handle.FrameManager.Page;

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
                    ] as const;
                }").ConfigureAwait(false);

                clip.X += points[0];
                clip.Y += points[1];

                options.Clip = clip.ToClip();

                return await page.ScreenshotBase64Async(options).ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public Task HoverAsync()
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.MoveAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task ClickAsync(ClickOptions options = null)
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync(options?.OffSet).ConfigureAwait(false);
                await Page.Mouse.ClickAsync(clickablePoint.X, clickablePoint.Y, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task UploadFileAsync(params string[] filePaths) => UploadFileAsync(true, filePaths);

        /// <inheritdoc/>
        public Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
            => BindIsolatedHandleAsync(async handle =>
            {
                var isMultiple = await EvaluateFunctionAsync<bool>("element => element.multiple").ConfigureAwait(false);

                if (!isMultiple && filePaths.Length > 1)
                {
                    throw new PuppeteerException("Multiple file uploads only work with <input type=file multiple>");
                }

                // The zero-length array is a special case, it seems that
                // DOM.setFileInputFiles does not actually update the files in that case, so
                // the solution is to eval the element value to a new FileList directly.
                if (!filePaths.Any())
                {
                    await handle.EvaluateFunctionAsync(@"(element) => {
                        element.files = new DataTransfer().files;

                        // Dispatch events for this case because it should behave akin to a user action.
                        element.dispatchEvent(
                            new Event('input', {bubbles: true, composed: true})
                        );
                        element.dispatchEvent(new Event('change', { bubbles: true }));
                    }").ConfigureAwait(false);

                    return handle;
                }

                var objectId = RemoteObject.ObjectId;
                var node = await handle.Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
                {
                    ObjectId = RemoteObject.ObjectId,
                }).ConfigureAwait(false);
                var backendNodeId = node.Node.BackendNodeId;

                var files = resolveFilePaths ? filePaths.Select(Path.GetFullPath).ToArray() : filePaths;
                CheckForFileAccess(files);
                await handle.Client.SendAsync("DOM.setFileInputFiles", new DomSetFileInputFilesRequest
                {
                    ObjectId = objectId,
                    Files = files,
                    BackendNodeId = backendNodeId,
                }).ConfigureAwait(false);

                return handle;
            });

        /// <inheritdoc/>
        public Task TapAsync()
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Touchscreen.TapAsync(clickablePoint.X, clickablePoint.Y).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task FocusAsync() => BindIsolatedHandleAsync(handle => handle.EvaluateFunctionAsync("element => element.focus()"));

        /// <inheritdoc/>
        public Task TypeAsync(string text, TypeOptions options = null)
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.FocusAsync().ConfigureAwait(false);
                await Page.Keyboard.TypeAsync(text, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task PressAsync(string key, PressOptions options = null)
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.FocusAsync().ConfigureAwait(false);
                await Page.Keyboard.PressAsync(key, options).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task<IElementHandle> QuerySelectorAsync(string selector)
            => BindIsolatedHandleAsync(handle =>
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
            => BindIsolatedHandleAsync(async handle =>
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

            return BindIsolatedHandleAsync(handle => handle.QuerySelectorAllAsync($"xpath/{expression}"));
        }

        /// <inheritdoc/>
        public async Task<BoundingBox> BoundingBoxAsync()
        {
            var result = await GetBoxModelAsync().ConfigureAwait(false);

            if (result == null)
            {
                return null;
            }

            var (offsetX, offsetY) = await GetOopifOffsetsAsync(Frame).ConfigureAwait(false);
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
            var (offsetX, offsetY) = await GetOopifOffsetsAsync(Frame).ConfigureAwait(false);

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

            return string.IsNullOrEmpty(nodeInfo.Node.FrameId) ? null : await FrameManager.FrameTree.GetFrameAsync(nodeInfo.Node.FrameId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsIntersectingViewportAsync(int threshold)
            => BindIsolatedHandleAsync(handle =>
                handle.Realm.EvaluateFunctionAsync<bool>(
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
                    handle,
                    threshold));

        /// <inheritdoc/>
        public Task<string[]> SelectAsync(params string[] values)
            => BindIsolatedHandleAsync(handle => handle.EvaluateFunctionAsync<string[]>(
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
            => BindIsolatedHandleAsync(async handle =>
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
            => BindIsolatedHandleAsync(async handle =>
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
                    return await Page.Mouse.DragAsync(start.X, start.Y, targetPoint.X, targetPoint.Y).ConfigureAwait(false);
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
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.DragEnterAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task DragOverAsync(DragData data)
            => BindIsolatedHandleAsync(async handle =>
            {
                await handle.ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
                var clickablePoint = await handle.ClickablePointAsync().ConfigureAwait(false);
                await Page.Mouse.DragOverAsync(clickablePoint.X, clickablePoint.Y, data).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task DropAsync(DragData data)
            => BindIsolatedHandleAsync(async handle =>
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
            => BindIsolatedHandleAsync(async handle =>
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
                await Page.Mouse.DragAndDropAsync(clickablePoint.X, clickablePoint.Y, targetPoint.X, targetPoint.Y, delay).ConfigureAwait(false);
                return handle;
            });

        /// <inheritdoc/>
        public Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
            => BindIsolatedHandleAsync(async handle =>
            {
                GetContentQuadsResponse result = null;

                var contentQuadsTask = handle.Client.SendAsync<GetContentQuadsResponse>("DOM.getContentQuads", new DomGetContentQuadsRequest
                {
                    ObjectId = handle.RemoteObject.ObjectId,
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

                var (offsetX, offsetY) = await GetOopifOffsetsAsync(Frame).ConfigureAwait(false);

                // Filter out quads that have too small area to click into.
                var quads = result.Quads
                    .Select(FromProtocolQuad)
                    .Select(quad => ApplyOffsetsToQuad(quad, offsetX, offsetY))
                    .Select(q => IntersectQuadWithViewport(q, layoutTask.Result))
                    .Where(q => ComputeQuadArea(q.ToArray()) > 1);

                var quadsArray = quads as IEnumerable<BoxModelPoint>[] ?? quads.ToArray();
                if (!quadsArray.Any())
                {
                    throw new PuppeteerException("Node is either not visible or not an HTMLElement");
                }

                // Return the middle point of the first quad.
                var quad = quadsArray.First().ToArray();
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
            });

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync() => BindIsolatedHandleAsync(handle => CheckVisibilityAsync(handle, true));

        /// <inheritdoc/>
        public Task<bool> IsHiddenAsync() => BindIsolatedHandleAsync(handle => CheckVisibilityAsync(handle, false));

        /// <inheritdoc/>
        public override Task<IJSHandle> GetPropertyAsync(string propertyName)
            => BindIsolatedHandleAsync(element => element.Handle.GetPropertyAsync(propertyName));

        /// <inheritdoc/>
        public override Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
            => BindIsolatedHandleAsync(element => element.Handle.GetPropertiesAsync());

        /// <inheritdoc/>
        public override Task<T> JsonValueAsync<T>()
            => BindIsolatedHandleAsync(element => element.Handle.JsonValueAsync<T>());

        private async Task<BoundingBox> NonEmptyVisibleBoundingBoxAsync()
        {
            var box = await BoundingBoxAsync().ConfigureAwait(false);

            if (box == null)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

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

        private async Task ScrollIntoViewIfNeededAsync()
        {
            if (await IsIntersectingViewportAsync(1).ConfigureAwait(false))
            {
                return;
            }

            await ScrollIntoViewAsync().ConfigureAwait(false);
        }

        private Task ScrollIntoViewAsync()
            => BindIsolatedHandleAsync(async handle =>
            {
                try
                {
                    await handle.Client.SendAsync("DOM.scrollIntoViewIfNeeded", new DomScrollIntoViewIfNeededRequest
                    {
                        ObjectId = Id,
                    }).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "DOM.scrollIntoViewIfNeeded is not supported");
                    await handle.EvaluateFunctionAsync(
                        @"element => {
                            element.scrollIntoView({
                                block: 'center',
                                inline: 'center',
                                behavior: 'instant',
                            });
                        }").ConfigureAwait(false);
                }

                return handle;
            });

        private Task<bool> CheckVisibilityAsync(IElementHandle handle, bool visibility)
            => handle.EvaluateFunctionAsync<bool>(
                @"async (element, PuppeteerUtil, visibility) =>
                {
                    return Boolean(PuppeteerUtil.checkVisibility(element, visibility));
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                visibility);

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

        private async Task<(decimal OffsetX, decimal OffsetY)> GetOopifOffsetsAsync(IFrame frame)
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

        private async Task<T> BindIsolatedHandleAsync<T>(Func<ElementHandle, Task<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (Realm == Frame.IsolatedRealm)
            {
                return await action(this).ConfigureAwait(false);
            }

            var adoptedThis = await Frame.IsolatedRealm.AdoptHandleAsync(this).ConfigureAwait(false) as ElementHandle;
            var result = await action(adoptedThis).ConfigureAwait(false);

            if (result is IJSHandle jsHandleResult)
            {
                // If the function returns `adoptedThis`, then we return `this` and T is a IJSHandle.
                if (jsHandleResult == adoptedThis)
                {
                    return (T)(object)this;
                }

                return (T)await Realm.TransferHandleAsync(jsHandleResult).ConfigureAwait(false);
            }

            // If the function returns an array of handlers, transfer them into the current realm.
            // Dynamic arrays using generics can be hard to translate
            if (typeof(T).IsArray && result is IEnumerable enumerable)
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

                var elementType = typeof(T).GetElementType();
                if (elementType != null)
                {
                    var output = Array.CreateInstance(elementType, resultArray.Count);

                    for (var i = 0; i < resultArray.Count; i++)
                    {
                        // You can use Convert.ChangeType to convert values to the desired type
                        output.SetValue(resultArray[i], i);
                    }

                    return (T)(object)output;
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
    }
}
