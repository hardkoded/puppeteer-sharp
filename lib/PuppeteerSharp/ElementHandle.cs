using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Input;
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
        internal Page Page { get; }

        internal ElementHandle(ExecutionContext context, CDPSession client, object remoteObject, Page page) :
            base(context, client, remoteObject)
        {
            Page = page;
        }

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
            var fileInfo = new FileInfo(file);
            options.Type = fileInfo.Extension.Replace(".", string.Empty);

            var data = await ScreenshotDataAsync(options);

            using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                await fs.WriteAsync(data, 0, data.Length);
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
            => new MemoryStream(await ScreenshotDataAsync(options));

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
        {
            await ScrollIntoViewIfNeededAsync();
            dynamic metrics = await Client.SendAsync("Page.getLayoutMetrics") as JObject;

            var boundingBox = await BoundingBoxAsync();
            if (boundingBox == null)
            {
                throw new PuppeteerException("Node is not visible");
            }

            boundingBox.X += metrics.layoutViewport.pageX.ToObject<decimal>();
            boundingBox.Y += metrics.layoutViewport.pageY.ToObject<decimal>();
            options.Clip = boundingBox.ToClip();
            return await Page.ScreenshotDataAsync(options);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Page.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <returns>Task which resolves when the element is successfully hovered</returns>
        public async Task HoverAsync()
        {
            var (x, y) = await VisibleCenterAsync();
            await Page.Mouse.MoveAsync(x, y);
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Page.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="options">click options</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully clicked</returns>
        public async Task ClickAsync(ClickOptions options = null)
        {
            var (x, y) = await VisibleCenterAsync();
            await Page.Mouse.ClickAsync(x, y, options);
        }

        /// <summary>
        /// Uploads files
        /// </summary>
        /// <param name="filePaths">Sets the value of the file input these paths. paths are resolved using <see cref="Path.GetFullPath(string)"/></param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/> </remarks>
        /// <returns>Task</returns>
        public async Task UploadFileAsync(params string[] filePaths)
        {
            var files = filePaths.Select(Path.GetFullPath).ToArray();
            var objectId = RemoteObject.objectId.ToString();
            await Client.SendAsync("DOM.setFileInputFiles", new { objectId, files });
        }

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap in the center of the element.
        /// </summary>
        /// <exception cref="PuppeteerException">if the element is detached from DOM</exception>
        /// <returns>Task which resolves when the element is successfully tapped</returns>
        public async Task TapAsync()
        {
            var (x, y) = await VisibleCenterAsync();
            await Page.Touchscreen.TapAsync(x, y);
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
            await FocusAsync();
            await Page.Keyboard.TypeAsync(text, options);
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
            await FocusAsync();
            await Page.Keyboard.PressAsync(key, options);
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
                this, selector);

            if (handle is ElementHandle element)
            {
                return element;
            }

            await handle.DisposeAsync();
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
                this, selector);

            var properties = await arrayHandle.GetPropertiesAsync();
            await arrayHandle.DisposeAsync();

            return properties.Values.OfType<ElementHandle>().ToArray();
        }

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
            );
            var properties = await arrayHandle.GetPropertiesAsync();
            await arrayHandle.DisposeAsync();

            return properties.Values.OfType<ElementHandle>().ToArray();
        }

        private async Task<(decimal x, decimal y)> VisibleCenterAsync()
        {
            await ScrollIntoViewIfNeededAsync();
            var box = await BoundingBoxAsync();
            if (box == null)
            {
                throw new PuppeteerException("Node is not visible");
            }

            return (
                x: box.X + (box.Width / 2),
                y: box.Y + (box.Height / 2)
            );
        }

        private async Task ScrollIntoViewIfNeededAsync()
        {
            var errorMessage = await ExecutionContext.EvaluateFunctionAsync<string>(@"element => {
                if (!element.isConnected)
                    return 'Node is detached from document';
                if (element.nodeType !== Node.ELEMENT_NODE)
                    return 'Node is not of type HTMLElement';
                element.scrollIntoViewIfNeeded();
                return null;
            }", this);

            if (errorMessage != null)
            {
                throw new PuppeteerException(errorMessage);
            }
        }

        /// <summary>
        /// This method returns the bounding box of the element (relative to the main frame), 
        /// or null if the element is not visible.
        /// </summary>
        /// <returns>The BoundingBox task.</returns>
        public async Task<BoundingBox> BoundingBoxAsync()
        {
            dynamic result = null;

            try
            {
                result = await Client.SendAsync("DOM.getBoxModel", new
                {
                    objectId = RemoteObject.objectId.ToString()
                });
            }
            catch (PuppeteerException ex)
            {
                Logger.LogError(ex.Message);
            }

            if (result == null)
            {
                return null;
            }

            var quad = result.model.border.ToObject<decimal[]>();

            var x = new[] { quad[0], quad[2], quad[4], quad[6] }.Min();
            var y = new[] { quad[1], quad[3], quad[5], quad[7] }.Min();
            var width = new[] { quad[0], quad[2], quad[4], quad[6] }.Max() - x;
            var height = new[] { quad[1], quad[3], quad[5], quad[7] }.Max() - y;

            return new BoundingBox(x, y, width, height);
        }
    }
}