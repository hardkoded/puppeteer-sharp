using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PuppeteerPageBaseTest : PuppeteerBaseTest
    {
        protected PuppeteerSharp.Page Page { get; set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Page = await Browser.NewPageAsync();
        }

        public override async Task DisposeAsync()
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
