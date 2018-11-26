using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using PuppeteerSharp.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Inherits from <see cref="JSHandle"/>. It represents an in-page DOM element. 
    /// ElementHandles can be created by <see cref="Page.QuerySelectorAsync(string)"/> or <see cref="Page.QuerySelectorAllAsync(string)"/>.
    /// </summary>
    public class ElementHandle : JSHandle
    {
        private readonly FrameManager _frameManager;
        private readonly ILogger<ElementHandle> _logger;

        internal ElementHandle(
            ExecutionContext context,
            CDPSession client,
            JToken remoteObject,
            Page page,
            FrameManager frameManager) :
            base(context, client, remoteObject)
        {
            Page = page;
            _frameManager = frameManager;
            _logger = client.LoggerFactory.CreateLogger<ElementHandle>();
        }

        internal Page Page { get; }

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension. 
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided, 
        /// the image won't be saved to the disk.</param>
        public Task ScreenshotAsync(string file) => ScreenshotAsync(file, new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension. 
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided, 
        /// the image won't be saved to the disk.</param>
        /// <param name="options">Screenshot options.</param>
        public async Task ScreenshotAsync(string file, ScreenshotOptions options)
        {
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
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        public Task<Stream> ScreenshotStreamAsync() => ScreenshotStreamAsync(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
            => new MemoryStream(await ScreenshotDataAsync(options).ConfigureAwait(false));

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        public Task<byte[]> ScreenshotDataAsync() => ScreenshotDataAsync(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
            => Convert.FromBase64String(await ScreenshotBase64Async(options).ConfigureAwait(false));

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        public Task<string> ScreenshotBase64Async() => ScreenshotBase64Async(new ScreenshotOptions());

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="Page.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element. 
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        /// <param name="options">Screenshot options.</param>
        public async Task<string> ScreenshotBase64Async(ScreenshotOptions options)
        {
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
            await ExecutionContext.EvaluateFunctionAsync(@"function(element) {
                element.scrollIntoView({ block: 'center', inline: 'center', behavior: 'instant'});
            }", this).ConfigureAwait(false);

            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            boundingBox = await BoundingBoxAsync().ConfigureAwait(false);

            if (boundingBox == null)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
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

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Page.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <returns>Task which resolves when the element is successfully hovered</returns>
        public async Task HoverAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var (x, y) = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.MoveAsync(x, y).ConfigureAwait(false);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Page.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="options">click options</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully clicked</returns>
        public async Task ClickAsync(ClickOptions options = null)
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var (x, y) = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Mouse.ClickAsync(x, y, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads files
        /// </summary>
        /// <param name="filePaths">Sets the value of the file input these paths. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/> </remarks>
        /// <returns>Task</returns>
        public Task UploadFileAsync(params string[] filePaths)
        {
            var files = filePaths.Select(Path.GetFullPath).ToArray();
            var objectId = RemoteObject[MessageKeys.ObjectId].AsString();
            return Client.SendAsync("DOM.setFileInputFiles", new { objectId, files });
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap in the center of the element.
        /// </summary>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully tapped</returns>
        public async Task TapAsync()
        {
            await ScrollIntoViewIfNeededAsync().ConfigureAwait(false);
            var (x, y) = await ClickablePointAsync().ConfigureAwait(false);
            await Page.Touchscreen.TapAsync(x, y).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls <c>focus</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"/> on the element.
        /// </summary>
        /// <returns>Task</returns>
        public Task FocusAsync() => ExecutionContext.EvaluateFunctionAsync("element => element.focus()", this);

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
        /// var elementHandle = await page.GetElementAsync("input");
        /// await elementHandle.TypeAsync("some text");
        /// await elementHandle.PressAsync("Enter");
        /// </code>
        /// </example>
        /// <returns>Task</returns>
        public async Task TypeAsync(string text, TypeOptions options = null)
        {
            await FocusAsync().ConfigureAwait(false);
            await Page.Keyboard.TypeAsync(text, options).ConfigureAwait(false);
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
            await Page.Keyboard.PressAsync(key, options).ConfigureAwait(false);
        }

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for</param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        public async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var handle = await ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelector(selector)",
                this, selector).ConfigureAwait(false);

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
            var arrayHandle = await ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelectorAll(selector)",
                this, selector).ConfigureAwait(false);

            var properties = await arrayHandle.GetPropertiesAsync().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.Values.OfType<ElementHandle>().ToArray();
        }

        /// <summary>
        /// A utility function to be used with <see cref="Extensions.EvaluateFunctionAsync{T}(Task{JSHandle}, string, object[])"/>
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
                this, expression
            ).ConfigureAwait(false);
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
            var nodeInfo = await Client.SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new Dictionary<string, object>
            {
                { MessageKeys.ObjectId, RemoteObject[MessageKeys.ObjectId] }
            }).ConfigureAwait(false);

            return string.IsNullOrEmpty(nodeInfo.Node.FrameId) ? null : await _frameManager.GetFrameAsync(nodeInfo.Node.FrameId);
        }

        /// <summary>
        /// Evaluates if the element is visible in the current viewport.
        /// </summary>
        /// <returns>A task which resolves to true if the element is visible in the current viewport.</returns>
        public Task<bool> IsIntersectingViewportAsync()
            => ExecutionContext.EvaluateFunctionAsync<bool>(@"async element =>
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
            }", this);

        private async Task<(decimal x, decimal y)> ClickablePointAsync()
        {
            GetContentQuadsResponse result = null;

            try
            {
                result = await Client.SendAsync<GetContentQuadsResponse>("DOM.getContentQuads", new Dictionary<string, object>
                {
                    { MessageKeys.ObjectId, RemoteObject[MessageKeys.ObjectId] }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            if (result == null || result.Quads.Length == 0)
            {
                throw new PuppeteerException("Node is either not visible or not an HTMLElement");
            }

            // Filter out quads that have too small area to click into.
            var quads = result.Quads.Select(FromProtocolQuad).Where(q => ComputeQuadArea(q) > 1);
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

            return (
                x: x / 4,
                y: y / 4
            );
        }

        private async Task ScrollIntoViewIfNeededAsync()
        {
            var errorMessage = await ExecutionContext.EvaluateFunctionAsync<string>(@"async(element, pageJavascriptEnabled) => {
              if (!element.isConnected)
                return 'Node is detached from document';
              if (element.nodeType !== Node.ELEMENT_NODE)
                return 'Node is not of type HTMLElement';
              // force-scroll if page's javascript is disabled.
              if (!pageJavascriptEnabled) {
                element.scrollIntoView({block: 'center', inline: 'center', behavior: 'instant'});
                return false;
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
              return false;
            }", this, Page.JavascriptEnabled).ConfigureAwait(false);

            if (errorMessage != null)
            {
                throw new PuppeteerException(errorMessage);
            }
        }

        private async Task<BoxModelResponse> GetBoxModelAsync()
        {
            try
            {
                return await Client.SendAsync<BoxModelResponse>("DOM.getBoxModel", new
                {
                    objectId = RemoteObject[MessageKeys.ObjectId].AsString()
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
            new BoxModelPoint{ X = quad[0], Y = quad[1] },
            new BoxModelPoint{ X = quad[2], Y = quad[3] },
            new BoxModelPoint{ X = quad[4], Y = quad[5] },
            new BoxModelPoint{ X = quad[6], Y = quad[7] }
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