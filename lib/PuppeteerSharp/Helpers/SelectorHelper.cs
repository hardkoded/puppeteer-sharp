using System.Text.RegularExpressions;

namespace PuppeteerSharp.Helpers
{
    internal static class SelectorHelper
    {
        // Matches a single colon followed by a letter (pseudo-class like :focus, :hover)
        // but not a double colon (pseudo-element like ::before, ::after).
        private static readonly Regex _pseudoClassRegex = new("(?<!:):[a-zA-Z]", RegexOptions.Compiled);

        /// <summary>
        /// Detects whether a CSS selector contains pseudo-classes.
        /// Pseudo-classes (e.g. :focus, :hover) require RAF-based polling because
        /// mutation observers cannot detect pseudo-class state changes.
        /// </summary>
        /// <param name="selector">The CSS selector to check.</param>
        /// <returns><c>true</c> if the selector contains pseudo-classes; otherwise, <c>false</c>.</returns>
        internal static bool HasPseudoClasses(string selector)
        {
            return !string.IsNullOrEmpty(selector) && _pseudoClassRegex.IsMatch(selector);
        }
    }
}
