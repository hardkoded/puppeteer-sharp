using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class MessageTask
    {
        internal MessageTask()
        {
        }

        internal TaskCompletionSource<JsonObject> TaskWrapper { get; set; }

        internal string Method { get; set; }
    }
}
