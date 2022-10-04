using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public static class TestUtils
    {
        public static async Task ShortWaitForCollectionToHaveAtLeastNElementsAsync(ICollection collection, int minLength, int attempts = 3, int timeout = 50)
        {
            for (var i = 0; i < attempts; i++)
            {
                if (collection.Count >= minLength)
                {
                    break;
                }
                await Task.Delay(timeout);
            }
        }

        public static string FindParentDirectory(string directory)
        {
            var current = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(current, directory)))
            {
                current = Directory.GetParent(current).FullName;
            }
            return Path.Combine(current, directory);
        }

        /// <summary>
        /// Removes as much whitespace as possible from a given string. Whitespace
        /// that separates letters and/or digits is collapsed to a space character.
        /// Other whitespace is fully removed.
        /// </summary>
        public static string CompressText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var sb = new StringBuilder();
            var inWhitespace = false;
            foreach (var ch in text)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (ch != '\n' && ch != '\r')
                    {
                        inWhitespace = true;
                    }
                }
                else
                {
                    if (inWhitespace)
                    {
                        inWhitespace = false;
                        if (sb.Length > 0 && char.IsLetterOrDigit(sb[sb.Length - 1]) && char.IsLetterOrDigit(ch))
                        {
                            sb.Append(' ');
                        }
                    }
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        internal static async Task WaitForCookieInChromiumFileAsync(string path, string valueToCheck)
        {
            var attempts = 0;
            const int maxAttempts = 10;
            var cookiesFile = Path.Combine(path, "Default", "Cookies");

            while (true)
            {
                attempts++;

                try
                {
                    if (File.Exists(cookiesFile) && File.ReadAllText(cookiesFile).Contains(valueToCheck))
                    {
                        return;
                    }
                }
                catch (IOException)
                {
                    if (attempts == maxAttempts)
                    {
                        break;
                    }
                }
                await Task.Delay(100);
            }
        }
        internal static bool IsFavicon(IRequest request) => request.Url.Contains("favicon.ico");
        internal static string CurateProtocol(string protocol)
            => protocol
                .ToLower()
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty);
    }
}
