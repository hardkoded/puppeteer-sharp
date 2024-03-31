using System.Threading.Tasks;

namespace PuppeteerSharp.Input
{
    /// <inheritdoc/>
    public abstract class Keyboard : IKeyboard
    {
        internal int Modifiers { get; set; }

        /// <inheritdoc/>
        public abstract Task DownAsync(string key, DownOptions options = null);

        /// <inheritdoc/>
        public abstract Task UpAsync(string key);

        /// <inheritdoc/>
        public abstract Task SendCharacterAsync(string charText);

        /// <inheritdoc/>
        public abstract Task TypeAsync(string text, TypeOptions options = null);

        /// <inheritdoc/>
        public abstract Task PressAsync(string key, PressOptions options = null);
    }
}
