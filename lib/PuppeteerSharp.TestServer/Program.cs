using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace PuppeteerSharp.TestServer
{
    public class Program
    {
        public static IWebHostBuilder GetWebHostBuilder(params string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 8907);
                options.Listen(IPAddress.Loopback, 8908, listenOptions =>
                {
                    listenOptions.UseHttps("testCert.cer");
                });
            });
    }
}
