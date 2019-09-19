using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class Issue0648 : PuppeteerPageBaseTest
    {
        public Issue0648(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync("https://www.google.com/search?q=firewall&oq=firewall&ie=UTF-8");
        }
    }
}