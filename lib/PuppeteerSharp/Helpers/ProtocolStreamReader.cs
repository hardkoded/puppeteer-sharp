using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Helpers
{
    internal static class ProtocolStreamReader
    {
        internal static async Task<string> ReadProtocolStreamStringAsync(CDPSession client, string handle, string path)
        {
            var result = new StringBuilder();
            var fs = !string.IsNullOrEmpty(path) ? AsyncFileHelper.CreateStream(path, FileMode.Create) : null;

            try
            {
                var eof = false;

                while (!eof)
                {
                    var response = await client.SendAsync<IOReadResponse>("IO.read", new IOReadRequest
                    {
                        Handle = handle,
                    }).ConfigureAwait(false);

                    eof = response.Eof;

                    result.Append(response.Data);

                    if (fs != null)
                    {
                        var data = Encoding.UTF8.GetBytes(response.Data);
                        await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    }
                }

                await client.SendAsync("IO.close", new IOCloseRequest
                {
                    Handle = handle,
                }).ConfigureAwait(false);

                return result.ToString();
            }
            finally
            {
                fs?.Dispose();
            }
        }

        internal static async Task<byte[]> ReadProtocolStreamByteAsync(CDPSession client, string handle, string path)
        {
            IEnumerable<byte> result = null;
            var eof = false;
            var fs = !string.IsNullOrEmpty(path) ? AsyncFileHelper.CreateStream(path, FileMode.Create) : null;

            try
            {
                while (!eof)
                {
                    var response = await client.SendAsync<IOReadResponse>("IO.read", new IOReadRequest
                    {
                        Handle = handle,
                    }).ConfigureAwait(false);

                    eof = response.Eof;
                    var data = Convert.FromBase64String(response.Data);
                    result = result == null ? data : result.Concat(data);

                    if (fs != null)
                    {
                        await fs.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                    }
                }

                await client.SendAsync("IO.close", new IOCloseRequest
                {
                    Handle = handle,
                }).ConfigureAwait(false);

                return result.ToArray();
            }
            finally
            {
                fs?.Dispose();
            }
        }
    }
}
