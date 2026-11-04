using System.Collections.Generic;

namespace PuppeteerSharp.Helpers
{
    internal static class ConnectionOptionsHelper
    {
        internal static Dictionary<string, string> GetEffectiveHeaders(IConnectionOptions options)
        {
#pragma warning disable CS0618
            return options.WsOptions?.Headers ?? options.Headers;
#pragma warning restore CS0618
        }

        internal static void ConfigureWebSocketKeepAlive(System.Net.WebSockets.ClientWebSocket client, WsOptions wsOptions)
        {
            if (wsOptions?.KeepAlive != true)
            {
                client.Options.KeepAliveInterval = System.TimeSpan.Zero;
                return;
            }

            var intervalMs = wsOptions.KeepAliveIntervalMs ?? WsOptions.DefaultKeepAliveIntervalMs;
            var interval = System.TimeSpan.FromMilliseconds(intervalMs);
            client.Options.KeepAliveInterval = interval;
#if NET9_0_OR_GREATER
            client.Options.KeepAliveTimeout = interval;
#endif
        }
    }
}
