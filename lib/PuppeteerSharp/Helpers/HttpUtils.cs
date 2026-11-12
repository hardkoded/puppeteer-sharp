using System.Collections.Generic;
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
    /// <see href="https://www.rfc-editor.org/rfc/rfc9110.html#section-5.2">RFC 9110 Section 5.2</see>,
    /// except for <c>Set-Cookie</c>, which is joined with newlines. See
    /// <see href="https://www.rfc-editor.org/rfc/rfc9110.html#name-field-order">RFC 9110 Section 5.3</see>
    /// for the <c>Set-Cookie</c> exception.
    /// </summary>
    /// <param name="name">The lower-cased header name.</param>
    /// <param name="value">The header value to normalize.</param>
    /// <returns>The normalized header value.</returns>
    public static string NormalizeHeaderValue(string name, string value)
    {
        if (value == null || value.IndexOf('\n') == -1)
        {
            return value;
        }

        var separator = name == "set-cookie" ? "\n " : ", ";
        var parts = value.Split('\n');
        var builder = new StringBuilder(value.Length);
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
                builder.Append(separator);
            }

            builder.Append(trimmed);
            first = false;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Validates that header names and values do not contain CRLF characters.
    /// </summary>
    /// <param name="headers">The headers to validate.</param>
    /// <exception cref="PuppeteerException">Thrown when a header name or value contains CR or LF.</exception>
    public static void ValidateHeaders(IEnumerable<KeyValuePair<string, string>> headers)
    {
        if (headers == null)
        {
            return;
        }

        foreach (var header in headers)
        {
            if (header.Key.IndexOfAny(['\r', '\n']) != -1 || (header.Value?.IndexOfAny(['\r', '\n']) ?? -1) != -1)
            {
                throw new PuppeteerException(
                    $"Invalid header: CRLF characters are not allowed in header names or values (header: \"{header.Key}\").");
            }
        }
    }
}
