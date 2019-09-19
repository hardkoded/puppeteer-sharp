﻿using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerPageBaseTest : PuppeteerBrowserContextBaseTest
    {
        public PuppeteerPageBaseTest(ITestOutputHelper output) : base(output)
        {
        }

        protected Page Page { get; set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Page = await Context.NewPageAsync();
        }

        public override async Task DisposeAsync()
        {
            await Page.CloseAsync();
            await base.DisposeAsync();
        }

        protected Task WaitForError()
        {
            var wrapper = new TaskCompletionSource<bool>();

            void errorEvent(object sender, ErrorEventArgs e)
            {
                wrapper.SetResult(true);
                Page.Error -= errorEvent;
            }

            Page.Error += errorEvent;

            return wrapper.Task;
        }
    }
}
