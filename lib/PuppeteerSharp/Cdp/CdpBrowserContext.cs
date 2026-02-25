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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpBrowserContext : BrowserContext
{
    private readonly Connection _connection;
    private readonly CdpBrowser _browser;

    internal CdpBrowserContext(Connection connection, CdpBrowser browser, string contextId)
    {
        _connection = connection;
        Browser = browser;
        _browser = browser;
        Id = contextId;
    }

    internal DownloadBehavior DownloadBehavior { get; set; }

    /// <inheritdoc/>
    public override ITarget[] Targets() => Array.FindAll(Browser.Targets(), target => target.BrowserContext == this);

    /// <inheritdoc/>
    public override async Task<IPage[]> PagesAsync()
        => (await Task.WhenAll(
                Targets()
                    .Where(t =>
                        t.Type == TargetType.Page ||
                        (t.Type == TargetType.Other && Browser.IsPageTargetFunc(t as Target)) ||
                        (_browser.HandleDevToolsAsPage &&
                            t.Type == TargetType.Other &&
                            t.Url.StartsWith("devtools://devtools/bundled/devtools_app.html", StringComparison.OrdinalIgnoreCase)))
                    .Select(t => t.PageAsync())).ConfigureAwait(false))
            .Where(p => p != null).ToArray();

    /// <inheritdoc/>
    public override async Task<IPage> NewPageAsync(CreatePageOptions options = null)
    {
        var guard = await WaitForScreenshotOperationsAsync().ConfigureAwait(false);
        guard?.Dispose();
        return await _browser.CreatePageInContextAsync(Id, options).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task CloseAsync()
    {
        if (Id == null)
        {
            throw new PuppeteerException("Non-incognito profiles cannot be closed!");
        }

        return _browser.DisposeContextAsync(Id);
    }

    /// <inheritdoc/>
    public override Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
        => _connection.SendAsync("Browser.grantPermissions", new BrowserGrantPermissionsRequest
        {
            Origin = origin,
            BrowserContextId = Id,
            Permissions = permissions.ToArray(),
        });

    /// <inheritdoc/>
    public override async Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
    {
        await Task.WhenAll(permissions.Select(entry =>
        {
            var protocolPermission = new BrowserPermissionDescriptor
            {
                Name = entry.Permission.Name,
                UserVisibleOnly = entry.Permission.UserVisibleOnly,
                Sysex = entry.Permission.Sysex,
                AllowWithoutSanitization = entry.Permission.AllowWithoutSanitization,
                PanTiltZoom = entry.Permission.PanTiltZoom,
            };

            return _connection.SendAsync("Browser.setPermission", new BrowserSetPermissionRequest
            {
                Origin = origin == "*" ? null : origin,
                BrowserContextId = Id,
                Permission = protocolPermission,
                Setting = entry.State,
            });
        })).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task ClearPermissionOverridesAsync()
        => _connection.SendAsync("Browser.resetPermissions", new BrowserResetPermissionsRequest
        {
            BrowserContextId = Id,
        });

    /// <inheritdoc/>
    public override async Task<CookieParam[]> GetCookiesAsync()
    {
        var response = await _connection.SendAsync<StorageGetCookiesResponse>(
            "Storage.getCookies",
            new StorageGetCookiesRequest { BrowserContextId = Id }).ConfigureAwait(false);
        return response.Cookies;
    }

    /// <inheritdoc/>
    public override async Task SetCookieAsync(params CookieData[] cookies)
    {
        if (cookies == null)
        {
            throw new ArgumentNullException(nameof(cookies));
        }

        var cookiesToSet = cookies.Select(cookie => new CookieData
        {
            Name = cookie.Name,
            Value = cookie.Value,
            Domain = cookie.Domain,
            Path = cookie.Path,
            Secure = cookie.Secure,
            HttpOnly = cookie.HttpOnly,
            SameSite = ConvertSameSiteForCdp(cookie.SameSite),
            Expires = cookie.Expires,
            Priority = cookie.Priority,
            SourceScheme = cookie.SourceScheme,
            PartitionKey = cookie.PartitionKey,
        }).ToArray();

        await _connection.SendAsync(
            "Storage.setCookies",
            new StorageSetCookiesRequest
            {
                BrowserContextId = Id,
                Cookies = cookiesToSet,
            }).ConfigureAwait(false);
    }

    internal Task SetDownloadBehaviorAsync(DownloadBehavior downloadBehavior)
    {
        DownloadBehavior = downloadBehavior;
        return _connection.SendAsync("Browser.setDownloadBehavior", new Messaging.BrowserSetDownloadBehaviorRequest
        {
            Behavior = downloadBehavior.Policy,
            DownloadPath = downloadBehavior.DownloadPath,
            BrowserContextId = Id,
        });
    }

    private static SameSite? ConvertSameSiteForCdp(SameSite? sameSite)
    {
        return sameSite switch
        {
            SameSite.Strict or SameSite.Lax or SameSite.None => sameSite,
            _ => null,
        };
    }
}
