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
                // ~500Kbps down
                Download = ((500 * 1000) / 8) * 0.8,

                // ~500Kbps up
                Upload = ((500 * 1000) / 8) * 0.8,

                // 400ms RTT
                Latency = 400 * 5,
            },
            [NetworkConditions.Fast3G] = new NetworkConditions
            {
                // ~1.6 Mbps down
                Download = ((1.6 * 1000 * 1000) / 8) * 0.9,

                // ~0.75 Mbps up
                Upload = ((750 * 1000) / 8) * 0.9,

                // 150ms RTT
                Latency = 150 * 3.75,
            },

            // alias to Fast 3G to align with Lighthouse (crbug.com/342406608)
            // and DevTools (crbug.com/342406608),
            [NetworkConditions.Slow4G] = new NetworkConditions
            {
                // ~1.6 Mbps down
                Download = ((1.6 * 1000 * 1000) / 8) * 0.9,

                // ~0.75 Mbps up
                Upload = ((750 * 1000) / 8) * 0.9,

                // 150ms RTT
                Latency = 150 * 3.75,
            },
            [NetworkConditions.Fast4G] = new NetworkConditions
            {
                // 9 Mbps down
                Download = ((9 * 1000 * 1000) / 8) * 0.9,

                // 1.5 Mbps up
                Upload = ((1.5 * 1000 * 1000) / 8) * 0.9,

                // 60ms RTT
                Latency = 60 * 2.75,
            },
        };

        private static readonly Lazy<IReadOnlyDictionary<string, NetworkConditions>> _readOnlyConditions =
            new(() => new ReadOnlyDictionary<string, NetworkConditions>(Conditions));

        internal static IReadOnlyDictionary<string, NetworkConditions> ToReadOnly() => _readOnlyConditions.Value;
    }
}
