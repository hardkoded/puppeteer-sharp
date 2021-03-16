using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PuppeteerSharp
{
    /// <summary>
    /// Predefined network conditions name.
    /// </summary>
    public enum PredefinedNetworkConditionsName
    {
        /// <summary>
        /// Slow 3G
        /// </summary>
        Slow3G,

        /// <summary>
        /// Fast 3G
        /// </summary>
        Fast3G,
    }

    /// <summary>
    /// Predefined network conditions.
    /// </summary>
    public static class PredefinedNetworkConditions
    {
        private static readonly Dictionary<PredefinedNetworkConditionsName, NetworkConditions> Conditions = new Dictionary<PredefinedNetworkConditionsName, NetworkConditions>
        {
            [PredefinedNetworkConditionsName.Slow3G] = new NetworkConditions
            {
                Download = ((500 * 1000) / 8) * 0.8,
                Upload = ((500 * 1000) / 8) * 0.8,
                Latency = 400 * 5,
            },
            [PredefinedNetworkConditionsName.Fast3G] = new NetworkConditions
            {
                Download = ((1.6 * 1000 * 1000) / 8) * 0.9,
                Upload = ((750 * 1000) / 8) * 0.9,
                Latency = 150 * 3.75,
            },
        };

        private static Lazy<IReadOnlyDictionary<PredefinedNetworkConditionsName, NetworkConditions>> _readOnlyConditions =
            new Lazy<IReadOnlyDictionary<PredefinedNetworkConditionsName, NetworkConditions>>(() => new ReadOnlyDictionary<PredefinedNetworkConditionsName, NetworkConditions>(Conditions));

        internal static IReadOnlyDictionary<PredefinedNetworkConditionsName, NetworkConditions> ToReadOnly() => _readOnlyConditions.Value;
    }
}
