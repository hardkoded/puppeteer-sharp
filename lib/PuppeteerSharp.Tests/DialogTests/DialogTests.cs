using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DialogTests
{
    public class DialogTests : PuppeteerPageBaseTest
    {
        public DialogTests() : base()
        {
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should fire")]
        [PuppeteerTimeout]
        public async Task ShouldFire()
        {
            Page.Dialog += async (_, e) =>
            {
                Assert.AreEqual(DialogType.Alert, e.Dialog.DialogType);
                Assert.AreEqual(string.Empty, e.Dialog.DefaultValue);
                Assert.AreEqual("yo", e.Dialog.Message);

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
                Assert.AreEqual(DialogType.Prompt, e.Dialog.DialogType);
                Assert.AreEqual("yes.", e.Dialog.DefaultValue);
                Assert.AreEqual("question?", e.Dialog.Message);

                await e.Dialog.Accept("answer!");
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");
            Assert.AreEqual("answer!", result);
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should dismiss the prompt")]
        [PuppeteerTimeout]
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
