using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DialogTests
{
    public class DialogTests : PuppeteerPageBaseTest
    {
        public DialogTests(): base()
        {
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should fire")]
        [PuppeteerFact]
        public async Task ShouldFire()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.Equal(DialogType.Alert, e.Dialog.DialogType);
                Assert.Equal(string.Empty, e.Dialog.DefaultValue);
                Assert.Equal("yo", e.Dialog.Message);

                await e.Dialog.Accept();
            };

            await Page.EvaluateExpressionAsync("alert('yo');");
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should allow accepting prompts")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAllowAcceptingPrompts()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.Equal(DialogType.Prompt, e.Dialog.DialogType);
                Assert.Equal("yes.", e.Dialog.DefaultValue);
                Assert.Equal("question?", e.Dialog.Message);

                await e.Dialog.Accept("answer!");
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");
            Assert.Equal("answer!", result);
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should dismiss the prompt")]
        [PuppeteerFact]
        public async Task ShouldDismissThePrompt()
        {
            Page.Dialog += async (_, e) =>
            {
                await e.Dialog.Dismiss();
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?')");
            Assert.Null(result);
        }
    }
}
