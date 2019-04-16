using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IBrowserContext
    {
        bool IsIncognito { get; }
        IBrowser Browser { get; }
        ITarget[] Targets();
        Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null);
        Task<IPage[]> PagesAsync();
        Task<IPage> NewPageAsync();
        Task CloseAsync();
        Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions);
        Task ClearPermissionOverridesAsync();
    }
}