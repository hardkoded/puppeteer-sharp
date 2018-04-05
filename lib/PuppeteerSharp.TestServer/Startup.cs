using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PuppeteerSharp.TestServer
{
    public class Startup
    {
        public static IWebHostBuilder GetWebHostBuilder() => new WebHostBuilder()
            .UseStartup<Startup>()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 8907);
                options.Listen(IPAddress.Loopback, 8908, listenOptions =>
                {
                    listenOptions.UseHttps("testCert.cer");
                });
            });

        public Startup()
        {
        }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRewriter(new RewriteOptions()
                .AddRedirect("plzredirect", "empty.html"));
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
