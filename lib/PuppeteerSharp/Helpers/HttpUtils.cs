using System.Text;

namespace PuppeteerSharp.Helpers;

/// <summary>
/// Helpers for normalizing HTTP-related values.
/// </summary>
internal static class HttpUtils
{
    /// <summary>
    /// Normalizes HTTP header values by handling multiline values.
    /// Multiline header values are joined with commas according to
    /// <see href="https://www.rfc-editor.org/rfc/rfc9110.html#section-5.2">RFC 9110 Section 5.2</see>.
    /// </summary>
    /// <param name="header">The header value to normalize.</param>
    /// <returns>The normalized header value.</returns>
    public static string NormalizeHeaderValue(string header)
    {
        if (header == null || header.IndexOf('\n') == -1)
        {
            return header;
        }

        var parts = header.Split('\n');
        var builder = new StringBuilder(header.Length);
        var first = true;
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            if (!first)
            {
                builder.Append(", ");
            }

            builder.Append(trimmed);
            first = false;
        }

        return builder.ToString();
    }
}
