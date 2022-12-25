using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <summary>
    /// Keyboard provides an api for managing a virtual keyboard. The high level api is <see cref="TypeAsync(string, TypeOptions)"/>, which takes raw characters and generates proper keydown, keypress/input, and keyup events on your page.
    ///
    /// For finer control, you can use <see cref="IKeyboard.DownAsync(string, DownOptions)"/>, <see cref="UpAsync(string)"/>, and <see cref="SendCharacterAsync(string)"/> to manually fire events as if they were generated from a real keyboard.
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// Dispatches a <c>keydown</c> event.
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">down options.</param>
        /// <remarks>
        /// If <c>key</c> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also generated. The <c>text</c> option can be specified to force an input event to be generated.
        /// If <c>key</c> is a modifier key, <c>Shift</c>, <c>Meta</c>, <c>Control</c>, or <c>Alt</c>, subsequent key presses will be sent with that modifier active. To release the modifier key, use <see cref="UpAsync(string)"/>
        /// After the key is pressed once, subsequent calls to <see cref="DownAsync(string, DownOptions)"/> will have <see href="https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/repeat">repeat</see> set to <c>true</c>. To release the key, use <see cref="UpAsync(string)"/>.
        /// </remarks>
        /// <returns>Task.</returns>
        Task DownAsync(string key, DownOptions options = null);

        /// <summary>
        /// Shortcut for <see cref="DownAsync(string, DownOptions)"/> and <see cref="UpAsync(string)"/>.
        /// </summary>
        /// <param name="key">Name of key to press, such as <c>ArrowLeft</c>. <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <param name="options">press options.</param>
        /// <remarks>
        /// If <paramref name="key"/> is a single character and no modifier keys besides <c>Shift</c> are being held down, a <c>keypress</c>/<c>input</c> event will also generated. The <see cref="DownOptions.Text"/> option can be specified to force an input event to be generated.
        /// Modifier keys DO effect <see cref="IElementHandle.PressAsync(string, PressOptions)"/>. Holding down <c>Shift</c> will type the text in upper case.
        /// </remarks>
        /// <returns>Task.</returns>
        Task PressAsync(string key, PressOptions options = null);

        /// <summary>
        /// Dispatches a <c>keypress</c> and <c>input</c> event. This does not send a <c>keydown</c> or <c>keyup</c> event.
        /// </summary>
        /// <param name="charText">Character to send into the page.</param>
        /// <returns>Task.</returns>
        Task SendCharacterAsync(string charText);

        /// <summary>
        /// Sends a <c>keydown</c>, <c>keypress</c>/<c>input</c>, and <c>keyup</c> event for each character in the text.
        /// </summary>
        /// <param name="text">A text to type into a focused element.</param>
        /// <param name="options">type options.</param>
        /// <remarks>
        /// To press a special key, like <c>Control</c> or <c>ArrowDown</c>, use <see cref="PressAsync(string, PressOptions)"/>.
        /// </remarks>
        /// <returns>Task.</returns>
        Task TypeAsync(string text, TypeOptions options = null);

        /// <summary>
        /// Dispatches a <c>keyup</c> event.
        /// </summary>
        /// <param name="key">Name of key to release, such as `ArrowLeft`. See <see cref="KeyDefinitions"/> for a list of all key names.</param>
        /// <returns>Task.</returns>
        Task UpAsync(string key);
    }
}
