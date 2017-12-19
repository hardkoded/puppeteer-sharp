using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace PuppeteerSharp.TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 8907);
                    options.Listen(IPAddress.Loopback, 8908, listenOptions =>
                    {
                        listenOptions.UseHttps("testCert.cer");
                    });
                })
                .Build();

            host.Run();
        }
    }
}
