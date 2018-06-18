namespace PuppeteerSharp.Helpers
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Places double quotes around the passed path to prevent path parsing problems
        /// when folder or file names contain spaces.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string QuoteFilePath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            var trimmed = path.Trim();
            if (!trimmed.StartsWith("\""))
            {
                trimmed = "\"" + trimmed;
            }
            if (!trimmed.EndsWith("\""))
            {
                trimmed = trimmed + "\"";
            }
            return trimmed;
        }

        /// <summary>
        /// Removes double quotes around path parameters that might have been passed in from the
        /// user argument list. This is required as calls to BCL functions will not handle double
        /// quotes around a path and expect a straight string.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string UnQuoteFilePath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            var trimmed = path.Trim();
            if (trimmed.StartsWith("\""))
            {
                trimmed = trimmed.Substring(1, trimmed.Length - 1);
            }
            if (trimmed.EndsWith("\""))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 1);
            }
            return trimmed;
        }
    }
}
