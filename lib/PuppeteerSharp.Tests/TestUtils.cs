using System.IO;

namespace PuppeteerSharp.Tests
{
    public static class TestUtils
    {
        public static string FindParentDirectory(string directory)
        {
            var current = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(current, directory)))
            {
                current = Directory.GetParent(current).FullName;
            }
            return Path.Combine(current, directory);
        }

        public static string CompressText(string text) => text.Replace("\n", "").Replace("\t", "").Replace(" ", "");
    }
}
