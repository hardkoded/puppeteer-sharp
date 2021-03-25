using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Provides static methods for asynchronous file access.
    /// </summary>
    internal static class AsyncFileHelper
    {
        /// <inheritdoc cref="System.IO.FileStream(string, FileMode)" />
        public static FileStream CreateStream(string path, FileMode mode)
            => CreateStream(path, mode, mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite);

        /// <inheritdoc cref="System.IO.FileStream(string, FileMode, FileAccess)" />
        public static FileStream CreateStream(string path, FileMode mode, FileAccess access)
            => CreateStream(path, mode, access, FileShare.Read);

        /// <inheritdoc cref="System.IO.FileStream(string, FileMode, FileAccess, FileShare)" />
        public static FileStream CreateStream(string path, FileMode mode, FileAccess access, FileShare share)
            => new FileStream(path, mode, access, share, 4096, true);

        /// <inheritdoc cref="System.IO.File.ReadAllText(string)" />
        public static Task<string> ReadAllText(string path)
            => ReadAllText(path, Encoding.UTF8);

        /// <inheritdoc cref="System.IO.File.ReadAllText(string, Encoding)" />
        public static async Task<string> ReadAllText(string path, Encoding encoding)
        {
            using (var reader = OpenText(path, encoding))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc cref="System.IO.File.OpenRead(string)" />
        public static FileStream OpenRead(string path)
            => CreateStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        /// <inheritdoc cref="System.IO.File.OpenText(string)" />
        /// <param name="path">File path.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        public static StreamReader OpenText(string path, Encoding encoding)
            => new StreamReader(OpenRead(path), encoding);
    }
}
