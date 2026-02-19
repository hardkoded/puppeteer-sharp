// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Text.Json;
using WebDriverBiDi.Network;
using WebDriverBiDi.Storage;
using BidiCookie = WebDriverBiDi.Network.Cookie;

namespace PuppeteerSharp.Bidi;

internal static class BidiCookieHelper
{
    /// <summary>
    /// Converts a BiDi cookie to a PuppeteerSharp cookie.
    /// </summary>
    public static CookieParam BidiToPuppeteerCookie(BidiCookie bidiCookie)
        => new()
        {
            Name = bidiCookie.Name,
            Value = bidiCookie.Value.Value,
            Domain = bidiCookie.Domain,
            Path = bidiCookie.Path,
            Size = (int)bidiCookie.Size,
            HttpOnly = bidiCookie.HttpOnly,
            Secure = bidiCookie.Secure,
            SameSite = ConvertSameSiteBidiToPuppeteer(bidiCookie.SameSite),
            Expires = bidiCookie.EpochExpires.HasValue ? (double)bidiCookie.EpochExpires.Value / 1000 : -1,
            Session = !bidiCookie.EpochExpires.HasValue || bidiCookie.EpochExpires.Value == 0,
            SourceScheme = GetSourceScheme(bidiCookie),
            PartitionKey = GetPartitionKey(bidiCookie),
        };

    /// <summary>
    /// Converts a PuppeteerSharp cookie to a BiDi partial cookie.
    /// </summary>
    public static PartialCookie PuppeteerToBidiCookie(CookieParam cookie, string domain)
    {
        var bidiCookie = new PartialCookie(
            cookie.Name,
            BytesValue.FromString(cookie.Value),
            domain)
        {
            Path = cookie.Path,
            SameSite = ConvertSameSitePuppeteerToBidi(cookie.SameSite),
        };

        // Only set HttpOnly and Secure if explicitly provided
        if (cookie.HttpOnly.HasValue)
        {
            bidiCookie.HttpOnly = cookie.HttpOnly.Value;
        }

        if (cookie.Secure.HasValue)
        {
            bidiCookie.Secure = cookie.Secure.Value;
        }

        // Convert expiration
        if (cookie.Expires.HasValue && cookie.Expires.Value != -1)
        {
            bidiCookie.Expires = DateTimeOffset.FromUnixTimeSeconds((long)cookie.Expires.Value).DateTime;
        }

        // Add CDP-specific properties if needed
#pragma warning disable CS0618 // SameParty is deprecated
        if (cookie.SameParty.HasValue)
        {
            bidiCookie.AdditionalData["goog:sameParty"] = cookie.SameParty.Value;
        }
#pragma warning restore CS0618

        if (cookie.SourceScheme.HasValue)
        {
            bidiCookie.AdditionalData["goog:sourceScheme"] = ConvertSourceSchemeEnumToString(cookie.SourceScheme.Value);
        }

        if (cookie.Priority.HasValue)
        {
            bidiCookie.AdditionalData["goog:priority"] = ConvertPriorityEnumToString(cookie.Priority.Value);
        }

        if (!string.IsNullOrEmpty(cookie.Url))
        {
            bidiCookie.AdditionalData["goog:url"] = cookie.Url;
        }

        return bidiCookie;
    }

    /// <summary>
    /// Checks if a cookie matches a URL according to the spec.
    /// </summary>
    public static bool TestUrlMatchCookie(CookieParam cookie, Uri url)
    {
        return TestUrlMatchCookieHostname(cookie, url) && TestUrlMatchCookiePath(cookie, url);
    }

    /// <summary>
    /// Checks if cookie domain matches URL hostname.
    /// </summary>
    private static bool TestUrlMatchCookieHostname(CookieParam cookie, Uri url)
    {
        var cookieDomain = cookie.Domain?.ToLowerInvariant() ?? string.Empty;
        var urlHostname = url.Host.ToLowerInvariant();

        if (cookieDomain == urlHostname)
        {
            return true;
        }

        // TODO: does not consider additional restrictions w.r.t to IP
        // addresses which is fine as it is for representation and does not
        // mean that cookies actually apply that way in the browser.
        // https://datatracker.ietf.org/doc/html/rfc6265#section-5.1.3
        return cookieDomain.StartsWith(".", StringComparison.Ordinal) && urlHostname.EndsWith(cookieDomain, StringComparison.Ordinal);
    }

    /// <summary>
    /// Checks if cookie path matches URL path.
    /// Spec: https://datatracker.ietf.org/doc/html/rfc6265#section-5.1.4.
    /// </summary>
    private static bool TestUrlMatchCookiePath(CookieParam cookie, Uri url)
    {
        var uriPath = url.AbsolutePath;
        var cookiePath = cookie.Path;

        if (uriPath == cookiePath)
        {
            // The cookie-path and the request-path are identical.
            return true;
        }

        if (uriPath.StartsWith(cookiePath, StringComparison.Ordinal))
        {
            // The cookie-path is a prefix of the request-path.
            if (cookiePath.EndsWith("/", StringComparison.Ordinal))
            {
                // The last character of the cookie-path is %x2F ("/").
                return true;
            }

            if (uriPath.Length > cookiePath.Length && uriPath[cookiePath.Length] == '/')
            {
                // The first character of the request-path that is not included in the cookie-path
                // is a %x2F ("/") character.
                return true;
            }
        }

        return false;
    }

    private static SameSite ConvertSameSiteBidiToPuppeteer(CookieSameSiteValue sameSite)
    {
        return sameSite switch
        {
            CookieSameSiteValue.Strict => SameSite.Strict,
            CookieSameSiteValue.Lax => SameSite.Lax,
            CookieSameSiteValue.None => SameSite.None,
            _ => SameSite.Default,
        };
    }

    private static CookieSameSiteValue? ConvertSameSitePuppeteerToBidi(SameSite? sameSite)
    {
        return sameSite switch
        {
            SameSite.Strict => CookieSameSiteValue.Strict,
            SameSite.Lax => CookieSameSiteValue.Lax,
            SameSite.None => CookieSameSiteValue.None,
            _ => CookieSameSiteValue.Default,
        };
    }

    private static string ConvertSourceSchemeEnumToString(CookieSourceScheme sourceScheme)
    {
        return sourceScheme switch
        {
            CookieSourceScheme.Unset => "Unset",
            CookieSourceScheme.NonSecure => "NonSecure",
            CookieSourceScheme.Secure => "Secure",
            _ => "Unset",
        };
    }

    private static string ConvertPriorityEnumToString(CookiePriority priority)
    {
        return priority switch
        {
            CookiePriority.Low => "Low",
            CookiePriority.Medium => "Medium",
            CookiePriority.High => "High",
            _ => "Medium",
        };
    }

    private static string GetPartitionKey(BidiCookie bidiCookie)
    {
        if (!bidiCookie.AdditionalData.TryGetValue("goog:partitionKey", out var value))
        {
            return null;
        }

        if (value is string stringValue)
        {
            return stringValue;
        }

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                return jsonElement.GetString();
            }

            if (jsonElement.ValueKind == JsonValueKind.Object &&
                jsonElement.TryGetProperty("topLevelSite", out var topLevelSite))
            {
                return topLevelSite.GetString();
            }
        }

        return null;
    }

    private static CookieSourceScheme GetSourceScheme(BidiCookie bidiCookie)
    {
        if (!bidiCookie.AdditionalData.TryGetValue("goog:sourceScheme", out var value))
        {
            return CookieSourceScheme.Unset;
        }

        var scheme = value?.ToString();
        return scheme switch
        {
            "Secure" => CookieSourceScheme.Secure,
            "NonSecure" => CookieSourceScheme.NonSecure,
            _ => CookieSourceScheme.Unset,
        };
    }
}

#endif
