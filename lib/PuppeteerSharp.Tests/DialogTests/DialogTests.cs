using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DialogTests
{
    public class DialogTests : PuppeteerPageBaseTest
    {
        public DialogTests() : base()
        {
        }

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
    }
}
