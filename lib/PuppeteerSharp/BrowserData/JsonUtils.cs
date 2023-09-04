using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PuppeteerSharp.BrowserData
{
    internal class JsonUtils
    {
        public static async Task<T> GetAsync<T>(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(response);
        }

        internal static async Task<string> GetTextAsync(string url)
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }
    }
}
