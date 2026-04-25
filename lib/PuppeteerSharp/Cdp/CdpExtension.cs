using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// CDP implementation of <see cref="Extension"/>.
/// </summary>
internal class CdpExtension : Extension
{
    private readonly CdpBrowser _browser;

    internal CdpExtension(string id, string version, string name, CdpBrowser browser)
        : base(id, version, name)
    {
        _browser = browser;
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<WebWorker>> WorkersAsync()
    {
        var targets = _browser.Targets();

        var workers = new List<WebWorker>();
        foreach (var target in targets)
        {
            var targetUrl = target.Url;
            if (target.Type == TargetType.ServiceWorker &&
                targetUrl.StartsWith("chrome-extension://" + Id, System.StringComparison.Ordinal))
            {
                var worker = await target.WorkerAsync().ConfigureAwait(false);
                if (worker != null)
                {
                    workers.Add(worker);
                }
            }
        }

        return workers;
    }

    /// <inheritdoc/>
    public override async Task<IReadOnlyList<IPage>> PagesAsync()
    {
        var targets = _browser.Targets();

        var pages = new List<IPage>();
        foreach (var target in targets)
        {
            var targetUrl = target.Url;
            if ((target.Type == TargetType.Page || target.Type == TargetType.BackgroundPage) &&
                targetUrl.StartsWith("chrome-extension://" + Id, System.StringComparison.Ordinal))
            {
                try
                {
                    var page = await target.AsPageAsync().ConfigureAwait(false);
                    if (page != null)
                    {
                        pages.Add(page);
                    }
                }
                catch
                {
                    // Target may have closed between enumeration and page creation.
                }
            }
        }

        return pages;
    }

    /// <inheritdoc/>
    public override async Task TriggerActionAsync(IPage page)
    {
        var cdpPage = (CdpPage)page;
        await _browser.Connection.SendAsync("Extensions.triggerAction", new ExtensionsTriggerActionRequest
        {
            Id = Id,
            TargetId = cdpPage.TabId,
        }).ConfigureAwait(false);
    }
}
