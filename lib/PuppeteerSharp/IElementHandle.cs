using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Input;

namespace PuppeteerSharp
{
    /// <summary>
    /// Inherits from <see cref="IJSHandle"/>. It represents an in-page DOM element.
    /// ElementHandles can be created by <see cref="IPage.QuerySelectorAsync(string)"/> or <see cref="IPage.QuerySelectorAllAsync(string)"/>.
    /// </summary>
    public interface IElementHandle : IJSHandle
    {
        /// <summary>
        /// Parent frame.
        /// </summary>
        IFrame Frame { get; }

        /// <summary>
        /// This method returns the bounding box of the element (relative to the main frame),
        /// or null if the element is not visible.
        /// </summary>
        /// <returns>The BoundingBox task.</returns>
        Task<BoundingBox> BoundingBoxAsync();

        /// <summary>
        /// returns boxes of the element, or <c>null</c> if the element is not visible. Box points are sorted clock-wise.
        /// </summary>
        /// <returns>Task BoxModel task.</returns>
        Task<BoxModel> BoxModelAsync();

        /// <summary>
        /// Returns the middle point within an element unless a specific offset is provided.
        /// </summary>
        /// <param name="offset">Optional offset.</param>
        /// <exception cref="PuppeteerException">When the node is not visible or not an HTMLElement.</exception>
        /// <returns>A <see cref="Task"/> that resolves to the clickable point.</returns>
        public Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null);

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="PuppeteerSharp.IPage.Mouse"/> to click in the center of the element.
        /// </summary>
        /// <param name="options">click options.</param>
        /// <exception cref="PuppeteerException">if the element is detached from DOM.</exception>
        /// <returns>Task which resolves when the element is successfully clicked.</returns>
        Task ClickAsync(ClickOptions options = null);

        /// <summary>
        /// Resolves the frame associated with the element..
        /// </summary>
        /// <returns>Task which resolves to the frame associated with the element.</returns>
        Task<IFrame> ContentFrameAsync();

        /// <summary>
        /// Performs a drag, dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="target">Target element.</param>
        /// <param name="delay">If specified, is the time to wait between `dragover` and `drop` in milliseconds.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DragAndDropAsync(IElementHandle target, int delay = 0);

        /// <summary>
        /// This method creates and captures a dragevent from the element.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser with the drag data.</returns>
        [Obsolete("Just use " + nameof(DropAsync) + " instead")]
        Task<DragData> DragAsync(decimal x, decimal y);

        /// <summary>
        /// This method creates and captures a dragevent from the element.
        /// </summary>
        /// <param name="target">Target Element.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser with the drag data.</returns>
        [Obsolete("Just use " + nameof(DropAsync) + " instead")]
        Task<DragData> DragAsync(IElementHandle target);

        /// <summary>
        /// Dispatches a `dragenter` event.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        [Obsolete("Don't use" + nameof(DragEnterAsync) + ". `dragenter` will automatically be performed during dragging. ")]
        Task DragEnterAsync(DragData data);

        /// <summary>
        /// Dispatches a `dragover` event.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        [Obsolete("Don't use" + nameof(DragOverAsync) + ". `dragover` will automatically be performed during dragging. ")]
        Task DragOverAsync(DragData data);

        /// <summary>
        /// Performs a dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="data">Drag data containing items and operations mask.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DropAsync(DragData data);

        /// <summary>
        /// Performs a dragenter, dragover, and drop in sequence.
        /// </summary>
        /// <param name="element">Element to drop.</param>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task DropAsync(IElementHandle element);

        /// <summary>
        /// Calls <c>focus</c> <see href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"/> on the element.
        /// </summary>
        /// <returns>Task.</returns>
        Task FocusAsync();

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="IPage.Mouse"/> to hover over the center of the element.
        /// </summary>
        /// <returns>Task which resolves when the element is successfully hovered.</returns>
        Task HoverAsync();

        /// <summary>
        /// Evaluates if the element is visible in the current viewport. If an element is an SVG, we check if the svg owner element is in the viewport instead.
        /// </summary>
        /// <param name="threshold">A number between 0 and 1 specifying the fraction of the element's area that must be visible to pass the check.</param>
        /// <returns>A task which resolves to true if the element is visible in the current viewport.</returns>
        Task<bool> IsIntersectingViewportAsync(decimal threshold = 0);

        /// <summary>
        /// Focuses the element, and then uses <see cref="IKeyboard.DownAsync(string, DownOptions)"/> and <see cref="IKeyboard.UpAsync(string)"/>.
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">press options.</param>
        /// <remarks>
        /// If <c>key</c> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also be generated. The <see cref="DownOptions.Text"/> option can be specified to force an input event to be generated.
        /// </remarks>
        /// <returns>Task which resolves when the key is successfully pressed.</returns>
        Task PressAsync(string key, PressOptions options = null);

        /// <summary>
        /// Runs <c>element.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for.</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements.</returns>
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);

        /// <summary>
        /// A utility function to be used with <see cref="PuppeteerHandleExtensions.EvaluateFunctionAsync{T}(Task{IJSHandle}, string, object[])"/>.
        /// </summary>
        /// <param name="selector">A selector to query element for.</param>
        /// <returns>Task which resolves to a <see cref="IJSHandle"/> of <c>document.querySelectorAll</c> result.</returns>
        Task<IJSHandle> QuerySelectorAllHandleAsync(string selector);

        /// <summary>
        /// The method runs <c>element.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="selector">A selector to query element for.</param>
        /// <returns>Task which resolves to <see cref="IElementHandle"/> pointing to the frame element.</returns>
        Task<IElementHandle> QuerySelectorAsync(string selector);

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <see cref="IPage.ScreenshotAsync(string)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        Task ScreenshotAsync(string file);

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="IPage.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="file">The file path to save the image to. The screenshot type will be inferred from file extension.
        /// If path is a relative path, then it is resolved relative to current working directory. If no path is provided,
        /// the image won't be saved to the disk.</param>
        /// <param name="options">Screenshot options.</param>
        Task ScreenshotAsync(string file, ElementScreenshotOptions options);

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        Task<string> ScreenshotBase64Async();

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotBase64Async(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="string"/> containing the image data as base64.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<string> ScreenshotBase64Async(ElementScreenshotOptions options);

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        Task<byte[]> ScreenshotDataAsync();

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="byte"/>[] containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<byte[]> ScreenshotDataAsync(ElementScreenshotOptions options);

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        Task<Stream> ScreenshotStreamAsync();

        /// <summary>
        /// This method scrolls element into view if needed, and then uses <seealso cref="PuppeteerSharp.IPage.ScreenshotDataAsync(ScreenshotOptions)"/> to take a screenshot of the element.
        /// If the element is detached from DOM, the method throws an error.
        /// </summary>
        /// <returns>Task which resolves to a <see cref="Stream"/> containing the image data.</returns>
        /// <param name="options">Screenshot options.</param>
        Task<Stream> ScreenshotStreamAsync(ElementScreenshotOptions options);

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
        Task<string[]> SelectAsync(params string[] values);

        /// <summary>
        /// Scrolls element into view if needed, and then uses <see cref="Touchscreen.TapAsync(decimal, decimal)"/> to tap in the center of the element.
        /// </summary>
        /// <exception cref="PuppeteerException">if the element is detached from DOM.</exception>
        /// <returns>Task which resolves when the element is successfully tapped.</returns>
        Task TapAsync();

        /// <summary>
        /// Focuses the element, and sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="text">A text to type into a focused element.</param>
        /// <param name="options">type options.</param>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c> use <see cref="IElementHandle.PressAsync(string, PressOptions)"/>.
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
        /// <returns>Task.</returns>
        Task TypeAsync(string text, TypeOptions options = null);

        /// <summary>
        /// Uploads files.
        /// </summary>
        /// <param name="resolveFilePaths">Set to true to resolve paths using <see cref="Path.GetFullPath(string)"/>.</param>
        /// <param name="filePaths">Sets the value of the file input to these paths. Paths are resolved using <see cref="Path.GetFullPath(string)"/>.</param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/>. </remarks>
        /// <returns>Task.</returns>
        Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths);

        /// <summary>
        /// Uploads files.
        /// </summary>
        /// <param name="filePaths">Sets the value of the file input to these paths. Paths are resolved using <see cref="Path.GetFullPath(string)"/>.</param>
        /// <remarks>This method expects <c>elementHandle</c> to point to an <c>input element</c> <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"/>. </remarks>
        /// <returns>Task.</returns>
        Task UploadFileAsync(params string[] filePaths);

        /// <summary>
        /// Waits for a selector to be added to the DOM.
        /// </summary>
        /// <param name="selector">A selector of an element to wait for.</param>
        /// <param name="options">Optional waiting parameters.</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.</returns>
        Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null);

        /// <summary>
        /// Checks if an element is visible using the same mechanism as <see cref="WaitForSelectorAsync"/>.
        /// </summary>
        /// <returns>Task which resolves to true if the element is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Checks if an element is hidden using the same mechanism as <see cref="WaitForSelectorAsync"/>.
        /// </summary>
        /// <returns>Task which resolves to true if the element is hidden.</returns>
        Task<bool> IsHiddenAsync();

        /// <summary>
        /// Dispatches a <c>touchstart</c> event.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchStartAsync();

        /// <summary>
        /// Dispatches a <c>touchmove</c> event.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchMoveAsync();

        /// <summary>
        /// /// Dispatches a <c>touchendt</c> event.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task TouchEndAsync();

        /// <summary>
        /// Scrolls the element into view using either the automation protocol client or by calling element.scrollIntoView.
        /// </summary>
        /// <returns>A Task that resolves when the message was confirmed by the browser.</returns>
        Task ScrollIntoViewAsync();
    }
}
