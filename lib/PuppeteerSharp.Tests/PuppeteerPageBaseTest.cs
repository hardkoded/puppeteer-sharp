﻿using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerPageBaseTest : PuppeteerBaseTest
    {
        protected Page Page { get; set; }

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

        protected Task WaitForError()
        {
            TaskCompletionSource<bool> wrapper = new TaskCompletionSource<bool>();

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
