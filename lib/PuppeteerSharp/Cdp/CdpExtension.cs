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

    /// <inheritdoc />
    public override async Task<WebWorker[]> WorkersAsync()
    {
        var targets = _browser.Targets();

        var extensionWorkers = targets.Where(target =>
            target.Type == TargetType.ServiceWorker &&
            target.Url.StartsWith("chrome-extension://" + Id, System.StringComparison.OrdinalIgnoreCase));

        var workers = new List<WebWorker>();
        foreach (var target in extensionWorkers)
        {
            var worker = await target.WorkerAsync().ConfigureAwait(false);
            if (worker != null)
            {
                workers.Add(worker);
            }
        }

        return workers.ToArray();
    }

    /// <inheritdoc />
    public override async Task<IPage[]> PagesAsync()
    {
        var targets = _browser.Targets();

        var extensionPages = targets.Where(target =>
            (target.Type == TargetType.Page || target.Type == TargetType.BackgroundPage) &&
            target.Url.StartsWith("chrome-extension://" + Id, System.StringComparison.OrdinalIgnoreCase));

        var pages = new List<IPage>();
        foreach (var target in extensionPages)
        {
            var page = await target.PageAsync().ConfigureAwait(false);
            if (page != null)
            {
                pages.Add(page);
            }
        }

        return pages.ToArray();
    }

    /// <inheritdoc />
    public override async Task TriggerActionAsync(IPage page)
    {
        var cdpPage = (CdpPage)page;
        await _browser.Connection.SendAsync(
            "Extensions.triggerAction",
            new ExtensionsTriggerActionRequest
            {
                Id = Id,
                TargetId = cdpPage.TabId,
            }).ConfigureAwait(false);
    }
}
