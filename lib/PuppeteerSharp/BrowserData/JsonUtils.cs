using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.BrowserData
{
    internal static class JsonUtils
    {
        public static async Task<T> GetAsync<T>(string url)
        {
            var response = await HttpUtil.GetTextAsync(url).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(
                response,
                JsonHelper.DefaultJsonSerializerSettings.Value
            );
        }

        internal static Task<string> GetTextAsync(string url) => HttpUtil.GetTextAsync(url);
    }
}
