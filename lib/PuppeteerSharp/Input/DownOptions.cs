namespace PuppeteerSharp.Input
{
    /// <summary>
    /// options to use with <see cref="IKeyboard.DownAsync(string, DownOptions)"/>.
    /// </summary>
    public class DownOptions
    {
        /// <summary>
        /// If specified, generates an input event with this text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// If specified, the commands of keyboard shortcuts.
        /// See <see href="https://source.chromium.org/chromium/chromium/src/+/main:third_party/blink/renderer/core/editing/commands/editor_command_names.h">Chromium Source Code</see> for valid command names.
        /// </summary>
        public string[] Commands { get; set; }
    }
}
