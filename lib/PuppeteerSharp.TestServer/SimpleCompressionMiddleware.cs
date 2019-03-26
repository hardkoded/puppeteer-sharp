using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PuppeteerSharp.TestServer
{
    internal class SimpleCompressionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SimpleServer _server;

        public SimpleCompressionMiddleware(RequestDelegate next, SimpleServer server)
        {
            _next = next;
            _server = server;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_server.GzipRoutes.Contains(context.Request.Path))
            {
                await _next(context);
            }

            var bodyWrapperStream = new MemoryStream();
            context.Response.Body = bodyWrapperStream;

            try
            {
                await _next(context);
            }
            finally
            {
                context.Response.Headers["Accept-Encoding"] = "gzip";
                var response = new MemoryStream();
                using (var compressionStream = new GZipStream(response, CompressionMode.Compress, true))
                {
                    bodyWrapperStream.CopyTo(compressionStream);
                }
                context.Response.Body = response;
            }
        }
    }
}