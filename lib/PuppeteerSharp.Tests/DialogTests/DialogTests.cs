using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DialogTests
{
    public class DialogTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("dialog.spec", "Page.Events.Dialog", "should fire")]
        public async Task ShouldFire()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.That(e.Dialog.DialogType, Is.EqualTo(DialogType.Alert));
                Assert.That(e.Dialog.DefaultValue, Is.EqualTo(string.Empty));
                Assert.That(e.Dialog.Message, Is.EqualTo("yo"));

                await e.Dialog.Accept();
            };

            await Page.EvaluateExpressionAsync("alert('yo');");
        }

        [Test, PuppeteerTest("dialog.spec", "Page.Events.Dialog", "should allow accepting prompts")]
        public async Task ShouldAllowAcceptingPrompts()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.That(e.Dialog.DialogType, Is.EqualTo(DialogType.Prompt));
                Assert.That(e.Dialog.DefaultValue, Is.EqualTo("yes."));
                Assert.That(e.Dialog.Message, Is.EqualTo("question?"));

                await e.Dialog.Accept("answer!");
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");
            Assert.That(result, Is.EqualTo("answer!"));
        }

        [Test, PuppeteerTest("dialog.spec", "Page.Events.Dialog", "should dismiss the prompt")]
        public async Task ShouldDismissThePrompt()
        {
            Page.Dialog += async (_, e) =>
            {
                await e.Dialog.Dismiss();
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?')");
            Assert.That(result, Is.Null);
        }

        [Test, PuppeteerTest("dialog.spec", "Page.Events.Dialog", "should see dialogs handled by other connections")]
        public async Task ShouldSeeDialogsHandledByOtherConnections()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await using var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions
            {
                BrowserWSEndpoint = Browser.WebSocketEndpoint,
                Protocol = ((Browser)Browser).Protocol,
            });

            var page2 = (await browser2.PagesAsync()).FirstOrDefault(page => page.Url == TestConstants.EmptyPage);
            Assert.That(page2, Is.Not.Null, "Could not find page2");

            var dialog1Task = WaitForDialogAsync(Page);
            var dialog2Task = WaitForDialogAsync(page2);
            var evaluateTask = page2.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");

            var dialog1 = await dialog1Task;
            var dialog2 = await dialog2Task;
            await dialog2.Accept("answer!");

            var result = await evaluateTask;
            Assert.That(result, Is.EqualTo("answer!"));

            // Wait for the event to be processed by the first connection.
            await Page.EvaluateExpressionAsync<int>("1");

            Assert.That(dialog1.Handled, Is.True);
            Assert.That(dialog2.Handled, Is.True);

            static Task<Dialog> WaitForDialogAsync(IPage page)
            {
                var dialogTask = new TaskCompletionSource<Dialog>(TaskCreationOptions.RunContinuationsAsynchronously);
                EventHandler<DialogEventArgs> handler = null;
                handler = (_, e) =>
                {
                    page.Dialog -= handler;
                    dialogTask.TrySetResult(e.Dialog);
                };
                page.Dialog += handler;
                return dialogTask.Task;
            }
        }

        [Test, PuppeteerTest("dialog.spec", "Page.Events.Dialog", "should expose handled getter")]
        public async Task ShouldExposeHandledGetter()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.That(e.Dialog.Handled, Is.False);
                await e.Dialog.Accept();
                Assert.That(e.Dialog.Handled, Is.True);
            };

            await Page.EvaluateExpressionAsync("alert('yo');");
        }
    }
}
