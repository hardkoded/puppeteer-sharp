using System.Text.Json;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class MessageTask
    {
        internal string Message { get; set; }

        internal TaskCompletionSource<JsonElement?> TaskWrapper { get; set; }

        internal string Method { get; set; }
    }
}
