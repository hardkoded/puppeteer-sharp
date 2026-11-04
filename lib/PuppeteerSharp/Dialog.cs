using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="Dialog"/> objects are dispatched by page via the 'dialog' event.
    /// </summary>
    /// <example>
    /// An example of using Dialog class:
    /// <code>
    /// <![CDATA[
    /// Page.Dialog += async (sender, e) =>
    /// {
    ///     await e.Dialog.Accept();
    /// }
    /// await Page.EvaluateExpressionAsync("alert('yo');");
    /// ]]>
    /// </code>
    /// </example>
    public abstract class Dialog
    {
        private bool _handled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="message">Message.</param>
        /// <param name="defaultValue">Default value.</param>
        public Dialog(DialogType type, string message, string defaultValue)
        {
            DialogType = type;
            Message = message;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Dialog's type, can be one of alert, beforeunload, confirm or prompt.
        /// </summary>
        /// <value>The type of the dialog.</value>
        public DialogType DialogType { get; set; }

        /// <summary>
        /// If dialog is prompt, returns default prompt value. Otherwise, returns empty string.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; set; }

        /// <summary>
        /// A message displayed in the dialog.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets a value indicating whether the dialog has been handled.
        /// </summary>
        /// <value><c>true</c> if the dialog has already been accepted or dismissed; otherwise, <c>false</c>.</value>
        public bool Handled => _handled;

        private protected bool IsHandled
        {
            get => _handled;
            set => _handled = value;
        }

        /// <summary>
        /// Accept the Dialog.
        /// </summary>
        /// <returns>Task which resolves when the dialog has been accepted.</returns>
        /// <param name="promptText">A text to enter in prompt. Does not cause any effects if the dialog's type is not prompt.</param>
        public Task Accept(string promptText = "")
        {
            if (IsHandled)
            {
                throw new InvalidOperationException("Cannot accept dialog which is already handled!");
            }

            IsHandled = true;
            return HandleAsync(true, promptText);
        }

        /// <summary>
        /// Dismiss the dialog.
        /// </summary>
        /// <returns>Task which resolves when the dialog has been dismissed.</returns>
        public Task Dismiss()
        {
            if (IsHandled)
            {
                throw new InvalidOperationException("Cannot dismiss dialog which is already handled!");
            }

            IsHandled = true;
            return HandleAsync(false, null);
        }

        internal abstract Task HandleAsync(bool accept, string text);
    }
}
