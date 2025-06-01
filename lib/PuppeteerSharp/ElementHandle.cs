using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CefSharp.Dom.Helpers;
using CefSharp.Dom.Helpers.Json;
using CefSharp.Dom.Input;
using CefSharp.Dom.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CefSharp.Dom
{
    /// <summary>
    /// Inherits from <see cref="JSHandle"/>. It represents an in-page DOM element.
    /// ElementHandles can be created by <see cref="IDevToolsContext.QuerySelectorAsync(string)"/> or <see cref="IDevToolsContext.QuerySelectorAllAsync(string)"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ElementHandle : JSHandle
    {
        private readonly FrameManager _frameManager;
        private readonly ILogger<ElementHandle> _logger;

        internal ElementHandle(
            ExecutionContext context,
            DevToolsConnection client,
            RemoteObject remoteObject,
            IDevToolsContext devToolsContext,
            FrameManager frameManager) : base(context, client, remoteObject)
        {
            DevToolsContext = devToolsContext;
            _frameManager = frameManager;
            _logger = client.LoggerFactory.CreateLogger<ElementHandle>();
        }

        internal IDevToolsContext DevToolsContext { get; }

        private string DebuggerDisplay =>
            string.IsNullOrEmpty(RemoteObject.ClassName) ? ToString() : $"{RemoteObject.ClassName}@{RemoteObject.Description}";

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        /// <param name="options">Screenshot options.</param>
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

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IDevToolsContext.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        /// <param name="options">Screenshot options.</param>
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

            var viewport = DevToolsContext.Viewport;
            if (viewport != null && (boundingBox.Width > viewport.Width || boundingBox.Height > viewport.Height))
            {
                var newRawViewport = JObject.FromObject(viewport);
                newRawViewport.Merge(new ViewPortOptions
                {
                    Width = (int)Math.Max(viewport.Width, Math.Ceiling(boundingBox.Width)),
                    Height = (int)Math.Max(viewport.Height, Math.Ceiling(boundingBox.Height))
                });
                await DevToolsContext.SetViewportAsync(newRawViewport.ToObject<ViewPortOptions>(true)).ConfigureAwait(false);
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
            var getLayoutMetricsResponse = await Connection.SendAsync<GetLayoutMetricsResponse>("Page.getLayoutMetrics").ConfigureAwait(false);

            var clip = boundingBox;
            clip.X += getLayoutMetricsResponse.LayoutViewport.PageX;
            clip.Y += getLayoutMetricsResponse.LayoutViewport.PageY;

            options.Clip = boundingBox.ToClip();
            var imageData = await DevToolsContext.ScreenshotBase64Async(options).ConfigureAwait(false);

            if (needsViewportReset)
            {
                await DevToolsContext.SetViewportAsync(viewport).ConfigureAwait(false);
            }

            return imageData;
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="IDevToolsContext.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <returns>Task which resolves when the element is successfully hovered</returns>
        public async Task HoverAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await DevToolsContext.Mouse.MoveAsync(point.X, point.Y).ConfigureAwait(false);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="IDevToolsContext.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="options">click options</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully clicked</returns>
        public async Task ClickAsync(ClickOptions options = null)
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync(options?.OffSet).ConfigureAwait(false);
            await DevToolsContext.Mouse.ClickAsync(point.X, point.Y, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads files
        /// </summary>
        /// <param name="filePaths">Sets the value of the file input to these paths. Paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/> </remarks>
        /// <returns>Task</returns>
        public Task UploadFileAsync(params string[] filePaths) => UploadFileAsync(true, filePaths);

        /// <summary>
        /// Uploads files
        /// </summary>
        /// <param name="resolveFilePaths">Set to true to resolve paths using <see cref="Path.GetFullPath(string)"/></param>
        /// <param name="filePaths">Sets the value of the file input to these paths. Paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/> </remarks>
        /// <returns>Task</returns>
        public async Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }

            var isMultiple = await EvaluateFunctionAsync<bool>("element => element.multiple").ConfigureAwait(false);

            if (!isMultiple && filePaths.Length > 1)
            {
                throw new PuppeteerException("Multiple file uploads only work with <input type=file multiple>");
            }

            var objectId = RemoteObject.ObjectId;
            var node = await Connection.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
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
                await Connection.SendAsync("DOM.setFileInputFiles", new DomSetFileInputFilesRequest
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
                    System.IO.File.Open(file, FileMode.Open).Dispose();
                }
                catch (Exception ex)
                {
                    throw new PuppeteerException($"{files} does not exist or is not readable", ex);
                }
            }
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap the element
        /// at the specified <see cref="TapOptions.OffSet"/> or in the center of the element if OffSet is null.
        /// </summary>
        /// <param name="options">tap options</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully tapped</returns>
        public async Task TapAsync(TapOptions options)
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync(options?.OffSet).ConfigureAwait(false);
            await DevToolsContext.Touchscreen.TapAsync(point.X, point.Y).ConfigureAwait(false);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap in the center of the element.
        /// </summary>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully tapped</returns>
        public async Task TapAsync()
        {
            await TapAsync(null).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls <c>focus</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"/> on the element.
        /// </summary>
        /// <returns>Task</returns>
        public Task FocusAsync() => EvaluateFunctionAsync("element => element.focus()");

        /// <summary>
        /// Get element attribute value
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="attribute">attribute</param>
        /// <returns>Task which resolves to the attributes value.</returns>
        public async Task<T> GetAttributeValueAsync<T>(string attribute)
        {
            var attr = await EvaluateFunctionHandleAsync("(element, attr) => element.getAttribute(attr)", attribute).ConfigureAwait(false);

            return await attr.GetValueAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Set element attribute value
        /// </summary>
        /// <param name="attribute">attribute name</param>
        /// <param name="value">attribute value</param>
        /// <returns>Task which resolves when the attribute value has been set.</returns>
        public Task SetAttributeValueAsync(string attribute, object value)
        {
            return EvaluateFunctionHandleAsync("(element, attr, val) => element.setAttribute(attr, val)", attribute, value);
        }

        /// <summary>
        /// Invokes a member function (method).
        /// </summary>
        /// <param name="memberFunctionName">case sensitive member function name</param>
        /// <returns>Task which resolves when member (method).</returns>
        public Task InvokeMemberAsync(string memberFunctionName)
        {
            return EvaluateFunctionHandleAsync($"(element) => element.{memberFunctionName}()");
        }

        /// <summary>
        /// Focuses the element, and sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="text">A text to type into a focused element</param>
        /// <param name="options">type options</param>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="ElementHandle.PressAsync(string, PressOptions)"/>
        /// </remarks>
        /// <example>
        /// <code>
        /// elementHandle.TypeAsync("#mytextarea", "Hello"); // Types instantly
        /// elementHandle.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
        /// </code>
        /// An example of typing into a text field and then submitting the form:
        /// <code>
        /// var elementHandle = await devToolsContext.GetElementAsync("input");
        /// await elementHandle.TypeAsync("some text");
        /// await elementHandle.PressAsync("Enter");
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public async Task TypeAsync(string text, TypeOptions options = null)
        {
            await FocusAsync().ConfigureAwait(false);
            await DevToolsContext.Keyboard.TypeAsync(text, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Focuses the element, and then uses <see cref="Keyboard.DownAsync(string, DownOptions)"/> and <see cref="Keyboard.UpAsync(string)"/>.
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">press options</param>
        /// <remarks>
        /// If <c>key</c> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also be generated. The <see cref="DownOptions.Text"/> option can be specified to force an input event to be generated.
        /// </remarks>
        /// <returns></returns>
        public async Task PressAsync(string key, PressOptions options = null)
        {
            await FocusAsync().ConfigureAwait(false);
            await DevToolsContext.Keyboard.PressAsync(key, options).ConfigureAwait(false);
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        public async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var handle = await EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelector(selector)",
                selector).ConfigureAwait(false);

            if (handle is ElementHandle element)
            {
                return element;
            }

            await handle.DisposeAsync().ConfigureAwait(false);
            return null;
        }

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public async Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var arrayHandle = await EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelectorAll(selector)",
                selector).ConfigureAwait(false);

            var properties = await arrayHandle.GetPropertiesAsync().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.Values.OfType<ElementHandle>().ToArray();
        }

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{JSHandle}, string, object[])"/>
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to a <see cref="JSHandle"/> of <c>document.querySelectorAll</c> result</returns>
        public Task<JSHandle> QuerySelectorAllHandleAsync(string selector)
            => ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => Array.from(element.querySelectorAll(selector))", this, selector);

        /// <summary>
        /// Evaluates the XPath expression relative to the elementHandle. If there's no such element, the method will resolve to <c>null</c>.
        /// </summary>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <see cref="ElementHandle"/></returns>
        public async Task<ElementHandle[]> XPathAsync(string expression)
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

            return properties.Values.OfType<ElementHandle>().ToArray();
        }

        /// <summary>
        /// This method returns the bounding box of the element (relative to the main frame),
        /// or null if the element is not visible.
        /// </summary>
        /// <returns>The BoundingBox task.</returns>
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

        /// <summary>
        /// returns boxes of the element, or <c>null</c> if the element is not visible. Box points are sorted clock-wise.
        /// </summary>
        /// <returns>Task BoxModel task.</returns>
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

        /// <summary>
        ///Content frame for element handles referencing iframe nodes, or null otherwise.
        /// </summary>
        /// <returns>Resolves to the content frame</returns>
        public async Task<Frame> ContentFrameAsync()
        {
            var nodeInfo = await Connection.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest
            {
                ObjectId = RemoteObject.ObjectId
            }).ConfigureAwait(false);

            if (string.IsNullOrEmpty(nodeInfo.Node.FrameId))
            {
                return null;
            }

            var frame = await _frameManager.GetFrameAsync(nodeInfo.Node.FrameId).ConfigureAwait(false);

            return frame;
        }

        /// <summary>
        /// Evaluates if the element is visible in the current viewport.
        /// </summary>
        /// <returns>A task which resolves to true if the element is visible in the current viewport.</returns>
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

        /// <summary>
        /// Triggers a `change` and `input` event once all the provided options have been selected.
        /// If there's no `select` element matching `selector`, the method throws an exception.
        /// </summary>
        /// <example>
        /// <code>
        /// await handle.SelectAsync("blue"); // single selection
        /// await handle.SelectAsync("red", "green", "blue"); // multiple selections
        /// </code>
        /// </example>
        /// <param name="values">Values of options to select. If the `select` has the `multiple` attribute, all values are considered, otherwise only the first one is taken into account.</param>
        /// <returns>A task that resolves to an array of option values that have been successfully selected.</returns>
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

        /// <summary>
        /// Set DOM Element Property. e.g innerText
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="val">value</param>
        /// <returns>Task</returns>
        public Task SetPropertyValueAsync(string propertyName, object val)
        {
            return EvaluateFunctionAsync("(element, v) => { element." + propertyName + " = v; }", val);
        }

        /// <summary>
        /// This method creates and captures a dragevent from the element.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser with the drag data</returns>
        public async Task<DragData> DragAsync(decimal x, decimal y)
        {
            if (!DevToolsContext.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var start = await ClickablePointAsync().ConfigureAwait(false);
            return await DevToolsContext.Mouse.DragAsync(start.X, start.Y, x, y).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispatches a `dragenter` event.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public async Task DragEnterAsync(DragData data)
        {
            if (!DevToolsContext.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await DevToolsContext.Mouse.DragEnterAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispatches a `dragover` event.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public async Task DragOverAsync(DragData data)
        {
            if (!DevToolsContext.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await DevToolsContext.Mouse.DragOverAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs a dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public async Task DropAsync(DragData data)
        {
            if (!DevToolsContext.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            await DevToolsContext.Mouse.DropAsync(point.X, point.Y, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs a drag, dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="target">Target element</param>
        /// <param name="delay">If specified, is the time to wait between `dragover` and `drop` in milliseconds.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
        public async Task DragAndDropAsync(ElementHandle target, int delay = 0)
        {
            if (target == null)
            {
                throw new ArgumentException("Target cannot be null", nameof(target));
            }

            if (!DevToolsContext.IsDragInterceptionEnabled)
            {
                throw new PuppeteerException("Drag Interception is not enabled!");
            }

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var point = await ClickablePointAsync().ConfigureAwait(false);
            var targetPoint = await target.ClickablePointAsync().ConfigureAwait(false);
            await DevToolsContext.Mouse.DragAndDropAsync(point.X, point.Y, targetPoint.X, targetPoint.Y, delay).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the middle point within an element unless a specific offset is provided.
        /// </summary>
        /// <param name="offset">Optional offset.</param>
        /// <exception cref="PuppeteerException">When the node is not visible or not an HTMLElement.</exception>
        /// <returns>A <see cref="Task"/> that resolves to the clickable point.</returns>
        public async Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
        {
            var box = await ClickableBoxAsync().ConfigureAwait(false) ?? throw new PuppeteerException("Node is either not clickable or not an Element");

            if (offset != null)
            {
                return new BoxModelPoint() { X = box.X + offset.Value.X, Y = box.Y + offset.Value.Y, };
            }

            return new BoxModelPoint() { X = box.X + (box.Width / 2), Y = box.Y + (box.Height / 2), };
        }

        /// <summary>
        /// Gets a clickable point for the current element (currently the mid point).
        /// </summary>
        /// <returns>Task that resolves to the x, y point that describes the element's position.</returns>
        public async Task<BoxModelPoint> ClickablePointAsync()
        {
            GetContentQuadsResponse result = null;

            var contentQuadsTask = Connection.SendAsync<GetContentQuadsResponse>("DOM.getContentQuads", new DomGetContentQuadsRequest
            {
                ObjectId = RemoteObject.ObjectId
            });
            var layoutTask = Connection.SendAsync<PageGetLayoutMetricsResponse>("Page.getLayoutMetrics");

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

            return new BoxModelPoint
            {
                X = x / 4,
                Y = y / 4
            };
        }

        private IEnumerable<BoxModelPoint> IntersectQuadWithViewport(IEnumerable<BoxModelPoint> quad, PageGetLayoutMetricsResponse viewport)
            => quad.Select(point => new BoxModelPoint
            {
                X = Math.Min(Math.Max(point.X, 0), viewport.ContentSize.Width),
                Y = Math.Min(Math.Max(point.Y, 0), viewport.ContentSize.Height),
            });

        /// <summary>
        /// If the element is not already fully visible then scrolls the element's parent container such that the element on
        /// which ScrollIntoViewIfNeededAsync() is called is visible to the user.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser</returns>
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
                DevToolsContext.JavascriptEnabled).ConfigureAwait(false);

            if (errorMessage != null)
            {
                throw new PuppeteerException(errorMessage);
            }
        }

        private async Task<BoxModelResponse> GetBoxModelAsync()
        {
            try
            {
                return await Connection.SendAsync<BoxModelResponse>("DOM.getBoxModel", new DomGetBoxModelRequest
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

        private async Task<BoundingBox> ClickableBoxAsync()
        {
            var boxes = await EvaluateFunctionAsync<BoundingBox[]>(@"element => {
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

            var frame = ExecutionContext.Frame;
            var parentFrame = frame.ParentFrame;
            while (parentFrame != null)
            {
                var handle = await frame.FrameElementAsync().ConfigureAwait(false)
                    ?? throw new PuppeteerException("Unsupported frame type");

                var parentBox = await handle.EvaluateFunctionAsync<BoundingBox>(@"element => {
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
            var documentBox = await EvaluateFunctionAsync<BoundingBox>(@"() => {
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
