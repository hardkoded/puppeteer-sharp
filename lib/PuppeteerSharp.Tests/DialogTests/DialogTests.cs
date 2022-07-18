using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.DialogTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DialogTests : DevToolsContextBaseTest
    {
        public DialogTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should fire")]
        [PuppeteerFact]
        public async Task ShouldFire()
        {
            DevToolsContext.Dialog += async (_, e) =>
            {
                Assert.Equal(DialogType.Alert, e.Dialog.DialogType);
                Assert.Equal(string.Empty, e.Dialog.DefaultValue);
                Assert.Equal("yo", e.Dialog.Message);

                await e.Dialog.Accept();
            };

            await DevToolsContext.EvaluateExpressionAsync("alert('yo');");
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should allow accepting prompts")]
        [PuppeteerFact]
        public async Task ShouldAllowAcceptingPrompts()
        {
            DevToolsContext.Dialog += async (_, e) =>
            {
                Assert.Equal(DialogType.Prompt, e.Dialog.DialogType);
                Assert.Equal("yes.", e.Dialog.DefaultValue);
                Assert.Equal("question?", e.Dialog.Message);

                await e.Dialog.Accept("answer!");
            };

            var result = await DevToolsContext.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");
            Assert.Equal("answer!", result);
        }

        [PuppeteerTest("dialog.spec.ts", "Page.Events.Dialog", "should dismiss the prompt")]
        [PuppeteerFact]
        public async Task ShouldDismissThePrompt()
        {
            DevToolsContext.Dialog += async (_, e) =>
            {
                await e.Dialog.Dismiss();
            };

            var result = await DevToolsContext.EvaluateExpressionAsync<string>("prompt('question?')");
            Assert.Null(result);
        }
    }
}
