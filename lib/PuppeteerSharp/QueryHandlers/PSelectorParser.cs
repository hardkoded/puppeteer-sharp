using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PuppeteerSharp.QueryHandlers
{
    /// <summary>
    /// Parses Puppeteer-specific selectors (P-selectors) that extend CSS with
    /// deep combinators (>>> and >>>>) and pseudo-elements (::-p-text, ::-p-xpath, ::-p-aria, etc.).
    /// Port of upstream PSelectorParser.ts.
    /// </summary>
    internal static class PSelectorParser
    {
        private const string CombinatorDescendent = ">>>";
        private const string CombinatorChild = ">>>>";

        private static readonly Regex EscapeRegexp = new(@"\\[\s\S]", RegexOptions.Compiled);

        /// <summary>
        /// Parses a P-selector string into a structured representation.
        /// </summary>
        /// <param name="selector">The selector string to parse.</param>
        /// <returns>A tuple of (selectorList, isPureCSS, hasPseudoClasses, hasAria).</returns>
        internal static (string JsonSelector, bool IsPureCSS, bool HasPseudoClasses, bool HasAria) Parse(string selector)
        {
            if (string.IsNullOrEmpty(selector))
            {
                return ("[]", true, false, false);
            }

            var isPureCSS = true;
            var hasAria = false;
            var hasPseudoClasses = false;

            // Current compound selector being built (list of CSS strings or pseudo-selector objects)
            var compoundSelector = new List<object>();

            // Current complex selector (alternating compound selectors and combinators)
            var complexSelector = new List<object>();
            complexSelector.Add(compoundSelector);

            // The full selector list (for comma-separated selectors)
            var selectorList = new List<object>();
            selectorList.Add(complexSelector);

            // Buffer for accumulating CSS text
            var cssBuffer = new System.Text.StringBuilder();

            var i = 0;
            while (i < selector.Length)
            {
                var ch = selector[i];

                // Check for deep combinators >>>> and >>>
                if (ch == '>' && i + 2 < selector.Length && selector[i + 1] == '>' && selector[i + 2] == '>')
                {
                    isPureCSS = false;

                    // Flush CSS buffer
                    FlushCssBuffer(cssBuffer, compoundSelector);

                    // Check for >>>> vs >>>
                    if (i + 3 < selector.Length && selector[i + 3] == '>')
                    {
                        // >>>> (Child combinator)
                        complexSelector.Add(CombinatorChild);
                        i += 4;
                    }
                    else
                    {
                        // >>> (Descendent combinator)
                        complexSelector.Add(CombinatorDescendent);
                        i += 3;
                    }

                    // Skip whitespace after combinator
                    while (i < selector.Length && char.IsWhiteSpace(selector[i]))
                    {
                        i++;
                    }

                    compoundSelector = new List<object>();
                    complexSelector.Add(compoundSelector);
                    continue;
                }

                // Check for ::-p-* pseudo-elements
                if (ch == ':' && i + 1 < selector.Length && selector[i + 1] == ':')
                {
                    var pseudoMatch = MatchPseudoElement(selector, i);
                    if (pseudoMatch.HasValue)
                    {
                        isPureCSS = false;

                        // Flush CSS buffer
                        FlushCssBuffer(cssBuffer, compoundSelector);

                        var name = pseudoMatch.Value.Name;
                        var value = Unquote(pseudoMatch.Value.Value);

                        if (name == "aria")
                        {
                            hasAria = true;
                        }

                        compoundSelector.Add(new PseudoSelector(name, value));

                        i = pseudoMatch.Value.EndIndex;
                        continue;
                    }
                }

                // Check for pseudo-classes (for hasPseudoClasses flag)
                if (ch == ':' && (i + 1 >= selector.Length || selector[i + 1] != ':'))
                {
                    hasPseudoClasses = true;

                    // Handle functional pseudo-classes like :nth-child(...)
                    cssBuffer.Append(ch);
                    i++;

                    // Consume the pseudo-class name
                    while (i < selector.Length && (char.IsLetterOrDigit(selector[i]) || selector[i] == '-'))
                    {
                        cssBuffer.Append(selector[i]);
                        i++;
                    }

                    // If followed by '(', consume the argument
                    if (i < selector.Length && selector[i] == '(')
                    {
                        var parenContent = ConsumeParenthesized(selector, i);
                        cssBuffer.Append(parenContent.Text);
                        i = parenContent.EndIndex;
                    }

                    continue;
                }

                // Check for comma (selector list separator)
                if (ch == ',')
                {
                    FlushCssBuffer(cssBuffer, compoundSelector);

                    compoundSelector = new List<object>();
                    complexSelector = new List<object>();
                    complexSelector.Add(compoundSelector);
                    selectorList.Add(complexSelector);

                    i++;

                    // Skip whitespace after comma
                    while (i < selector.Length && char.IsWhiteSpace(selector[i]))
                    {
                        i++;
                    }

                    continue;
                }

                // Handle strings (quoted values)
                if (ch == '"' || ch == '\'')
                {
                    var str = ConsumeString(selector, i);
                    cssBuffer.Append(str.Text);
                    i = str.EndIndex;
                    continue;
                }

                // Handle brackets [...]
                if (ch == '[')
                {
                    var bracket = ConsumeBracketed(selector, i, '[', ']');
                    cssBuffer.Append(bracket.Text);
                    i = bracket.EndIndex;
                    continue;
                }

                // Handle parentheses (...)
                if (ch == '(')
                {
                    var paren = ConsumeParenthesized(selector, i);
                    cssBuffer.Append(paren.Text);
                    i = paren.EndIndex;
                    continue;
                }

                // Regular character - add to CSS buffer
                cssBuffer.Append(ch);
                i++;
            }

            // Flush remaining CSS
            FlushCssBuffer(cssBuffer, compoundSelector);

            if (isPureCSS)
            {
                return (null, true, hasPseudoClasses, hasAria);
            }

            // Serialize to JSON
            var json = SerializeSelectorList(selectorList);
            return (json, false, hasPseudoClasses, hasAria);
        }

        private static void FlushCssBuffer(System.Text.StringBuilder buffer, List<object> compoundSelector)
        {
            if (buffer.Length > 0)
            {
                var css = buffer.ToString().Trim();
                if (!string.IsNullOrEmpty(css))
                {
                    compoundSelector.Add(css);
                }

                buffer.Clear();
            }
        }

        private static string Unquote(string text)
        {
            if (text.Length <= 1)
            {
                return text;
            }

            if ((text[0] == '"' || text[0] == '\'') && text[text.Length - 1] == text[0])
            {
                text = text.Substring(1, text.Length - 2);
            }

            return EscapeRegexp.Replace(text, m => m.Value.Substring(1));
        }

        private static PseudoElementMatch? MatchPseudoElement(string selector, int start)
        {
            // Must start with ::
            if (start + 1 >= selector.Length || selector[start] != ':' || selector[start + 1] != ':')
            {
                return null;
            }

            var i = start + 2;

            // Must start with -p-
            if (i + 3 > selector.Length || selector.Substring(i, 3) != "-p-")
            {
                return null;
            }

            i += 3;

            // Read the name
            var nameStart = i;
            while (i < selector.Length && (char.IsLetterOrDigit(selector[i]) || selector[i] == '-'))
            {
                i++;
            }

            if (i == nameStart)
            {
                return null;
            }

            var name = selector.Substring(nameStart, i - nameStart);

            // Check for optional argument in parentheses
            var value = string.Empty;
            if (i < selector.Length && selector[i] == '(')
            {
                var paren = ConsumeParenthesized(selector, i);

                // Strip the outer parentheses
                value = paren.Text.Substring(1, paren.Text.Length - 2);
                i = paren.EndIndex;
            }

            return new PseudoElementMatch(name, value, i);
        }

        private static (string Text, int EndIndex) ConsumeString(string selector, int start)
        {
            var quote = selector[start];
            var i = start + 1;
            var result = new System.Text.StringBuilder();
            result.Append(quote);

            while (i < selector.Length)
            {
                if (selector[i] == '\\' && i + 1 < selector.Length)
                {
                    result.Append(selector[i]);
                    result.Append(selector[i + 1]);
                    i += 2;
                    continue;
                }

                if (selector[i] == quote)
                {
                    result.Append(quote);
                    return (result.ToString(), i + 1);
                }

                result.Append(selector[i]);
                i++;
            }

            result.Append(quote);
            return (result.ToString(), i);
        }

        private static (string Text, int EndIndex) ConsumeParenthesized(string selector, int start)
        {
            return ConsumeBracketed(selector, start, '(', ')');
        }

        private static (string Text, int EndIndex) ConsumeBracketed(string selector, int start, char open, char close)
        {
            var depth = 0;
            var i = start;
            var result = new System.Text.StringBuilder();

            while (i < selector.Length)
            {
                var ch = selector[i];

                if (ch == '\\' && i + 1 < selector.Length)
                {
                    result.Append(ch);
                    result.Append(selector[i + 1]);
                    i += 2;
                    continue;
                }

                if (ch == '"' || ch == '\'')
                {
                    var str = ConsumeString(selector, i);
                    result.Append(str.Text);
                    i = str.EndIndex;
                    continue;
                }

                result.Append(ch);

                if (ch == open)
                {
                    depth++;
                }
                else if (ch == close)
                {
                    depth--;
                    if (depth == 0)
                    {
                        return (result.ToString(), i + 1);
                    }
                }

                i++;
            }

            return (result.ToString(), i);
        }

        private static string SerializeSelectorList(List<object> selectorList)
        {
            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            writer.WriteStartArray();

            foreach (var complexSelector in selectorList)
            {
                var complex = (List<object>)complexSelector;
                writer.WriteStartArray();

                foreach (var item in complex)
                {
                    if (item is string combinator)
                    {
                        writer.WriteStringValue(combinator);
                    }
                    else if (item is List<object> compound)
                    {
                        writer.WriteStartArray();

                        foreach (var part in compound)
                        {
                            if (part is string css)
                            {
                                writer.WriteStringValue(css);
                            }
                            else if (part is PseudoSelector pseudo)
                            {
                                writer.WriteStartObject();
                                writer.WriteString("name", pseudo.Name);
                                writer.WriteString("value", pseudo.Value);
                                writer.WriteEndObject();
                            }
                        }

                        writer.WriteEndArray();
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
            writer.Flush();

            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private readonly struct PseudoElementMatch
        {
            internal PseudoElementMatch(string name, string value, int endIndex)
            {
                Name = name;
                Value = value;
                EndIndex = endIndex;
            }

            internal string Name { get; }

            internal string Value { get; }

            internal int EndIndex { get; }
        }

        private sealed class PseudoSelector
        {
            internal PseudoSelector(string name, string value)
            {
                Name = name;
                Value = value;
            }

            internal string Name { get; }

            internal string Value { get; }
        }
    }
}
