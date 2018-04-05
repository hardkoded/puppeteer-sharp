﻿using Microsoft.AspNetCore.Hosting;
using PuppeteerSharp.TestServer;
using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerLoaderFixture : IDisposable
    {
        IWebHost _host;

        public PuppeteerLoaderFixture()
        {
            SetupAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        private async Task SetupAsync()
        {
            var downloaderTask = Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);
            var serverTask = StartWebServerAsync();

            await Task.WhenAll(downloaderTask, serverTask);
        }

        private async Task StartWebServerAsync()
        {
            var builder = Startup.GetWebHostBuilder();

            builder.UseContentRoot(TestUtils.FindParentDirectory("PuppeteerSharp.TestServer"));

            _host = builder.Build();

            await _host.StartAsync();
        }
    }
}
