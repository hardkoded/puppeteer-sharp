using PuppeteerSharp.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class ElementHandle : JSHandle
    {
        internal Page Page { get; }

        public ElementHandle(ExecutionContext context, Session client, object remoteObject, Page page) :
            base(context, client, remoteObject)
        {
            Page = page;
        }

        public override ElementHandle AsElement() => this;

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
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see cref="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/> </remarks>
        /// <returns>Task</returns>
        public async Task UploadFileAsync(params string[] filePaths)
        {
            var files = filePaths.Select(Path.GetFullPath).ToArray();
            var objectId = RemoteObject.objectId.ToString();
            await _client.SendAsync("DOM.setFileInputFiles", new { objectId, files });
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
        /// Calls <c>focus</c> <see cref="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"/> on the element.
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

        internal async Task<ElementHandle> QuerySelectorAsync(string selector)
        {
            var handle = await ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelector(selector)",
                this, selector);

            var element = handle.AsElement();
            if (element != null)
            {
                return element;
            }

            await handle.DisposeAsync();
            return null;
        }

        internal async Task<ElementHandle[]> QuerySelectorAllAsync(string selector)
        {
            var arrayHandle = await ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelectorAll(selector)",
                this, selector);

            var properties = await arrayHandle.GetPropertiesAsync();
            await arrayHandle.DisposeAsync();
            var result = new List<ElementHandle>();
            foreach(var property in properties.Values)
            {
                var elementHandle = property.AsElement();
                if(elementHandle != null)
                {
                    result.Add(elementHandle);
                }
            }
            return result.ToArray();
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
                throw new PuppeteerException(errorMessage);
        }

        private async Task<BoundingBox> BoundingBoxAsync()
        {
            var result = await _client.SendAsync("DOM.getBoxModel", new { objectId = RemoteObject.objectId.ToString() });

            if (result == null)
                return null;

            var quad = result.model.border.ToObject<decimal[]>();

            var x = new[] { quad[0], quad[2], quad[4], quad[6] }.Min();
            var y = new[] { quad[1], quad[3], quad[5], quad[7] }.Min();
            var width = new[] { quad[0], quad[2], quad[4], quad[6] }.Max() - x;
            var height = new[] { quad[1], quad[3], quad[5], quad[7] }.Max() - y;

            return new BoundingBox(x, y, width, height);

        }

        private class BoundingBox
        {
            public decimal X { get; }
            public decimal Y { get; }
            public decimal Width { get; }
            public decimal Height { get; }

            public BoundingBox(decimal x, decimal y, decimal width, decimal height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
    }
}