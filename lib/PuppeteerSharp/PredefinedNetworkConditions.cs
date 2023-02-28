using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PuppeteerSharp
{
    /// <summary>
    /// Predefined network conditions.
    /// </summary>
    public static class PredefinedNetworkConditions
    {
        private static readonly Dictionary<string, NetworkConditions> Conditions = new Dictionary<string, NetworkConditions>
        {
            [NetworkConditions.Slow3G] = new NetworkConditions
            {
                Download = ((500 * 1000) / 8) * 0.8,
                Upload = ((500 * 1000) / 8) * 0.8,
                Latency = 400 * 5,
            },
            [NetworkConditions.Fast3G] = new NetworkConditions
            {
                Download = ((1.6 * 1000 * 1000) / 8) * 0.9,
                Upload = ((750 * 1000) / 8) * 0.9,
                Latency = 150 * 3.75,
            },
        };

        private static readonly Lazy<IReadOnlyDictionary<string, NetworkConditions>> _readOnlyConditions =
            new Lazy<IReadOnlyDictionary<string, NetworkConditions>>(() => new ReadOnlyDictionary<string, NetworkConditions>(Conditions));

        internal static IReadOnlyDictionary<string, NetworkConditions> ToReadOnly() => _readOnlyConditions.Value;
    }
}
