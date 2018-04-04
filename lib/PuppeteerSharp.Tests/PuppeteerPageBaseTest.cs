using System;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerPageBaseTest : PuppeteerBaseTest
    {
        protected PuppeteerSharp.Page Page { get; set; }
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Page = await Browser.NewPageAsync();
        }
        protected override async Task DisposeAsync()
        {
            await Page.CloseAsync();
            await base.DisposeAsync();
        }
    }
}
