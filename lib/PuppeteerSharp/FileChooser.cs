using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="FileChooser"/> objects are returned via the <seealso cref="IPage.WaitForFileChooserAsync(WaitForOptions)"/> method.
    /// File choosers let you react to the page requesting for a file.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// var waitTask = page.WaitForFileChooserAsync();
    /// await Task.WhenAll(
    ///     waitTask,
    ///     page.ClickAsync("#upload-file-button")); // some button that triggers file selection
    ///
    /// await waitTask.Result.AcceptAsync('/tmp/myfile.pdf');
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// In browsers, only one file chooser can be opened at a time.
    /// All file choosers must be accepted or canceled. Not doing so will prevent subsequent file choosers from appearing.
    /// </remarks>
    public class FileChooser
    {
        private readonly IElementHandle _element;
        private bool _handled;

        internal FileChooser(IElementHandle element, PageFileChooserOpenedResponse e)
        {
            _element = element;
            IsMultiple = e.Mode != "selectSingle";
            _handled = false;
        }

        /// <summary>
        /// Whether file chooser allow for <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#attr-multiple">multiple</see> file selection.
        /// </summary>
        public bool IsMultiple { get; }

        /// <summary>
        /// Accept the file chooser request with given paths.
        /// If some of the filePaths are relative paths, then they are resolved relative to the current working directory.
        /// </summary>
        /// <param name="filePaths">File paths to send to the file chooser.</param>
        /// <returns>A task that resolves after the accept message is processed by the browser.</returns>
        public Task AcceptAsync(params string[] filePaths)
        {
            if (_handled)
            {
                throw new PuppeteerException("Cannot accept FileChooser which is already handled!");
            }

            _handled = true;
            var files = filePaths.Select(Path.GetFullPath);
            return _element.UploadFileAsync(files.ToArray());
        }

        /// <summary>
        /// Closes the file chooser without selecting any files.
        /// </summary>
        /// <returns>A task that resolves after the cancel event is sent to the browser.</returns>
        public Task CancelAsync()
        {
            if (_handled)
            {
                throw new PuppeteerException("Cannot accept FileChooser which is already handled!");
            }

            _handled = true;
            return _element.EvaluateFunctionAsync("element => element.dispatchEvent(new Event('cancel', {bubbles: true}))");
        }
    }
}
