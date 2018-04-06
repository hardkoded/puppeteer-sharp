using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class DialogTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldFire()
        {
            Page.Dialog += (sender, e) =>
            {
                Assert.Equal(DialogType.Alert, e.DialogInfo.DialogType);
                Assert.Equal(string.Empty, e.DialogInfo.DefaultValue);
                Assert.Equal("yo", e.DialogInfo.Message);

                e.DialogInfo.Accept();
            };

            await Page.EvaluateExpressionAsync("alert('yo');");
        }

        [Fact]
        public async Task ShouldAllowAcceptingPrompts()
        {
            Page.Dialog += (sender, e) =>
            {
                Assert.Equal(DialogType.Prompt, e.DialogInfo.DialogType);
                Assert.Equal("yes.", e.DialogInfo.DefaultValue);
                Assert.Equal("question?", e.DialogInfo.Message);

                e.DialogInfo.Accept("answer!");
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?', 'yes.')");
            Assert.Equal("answer!", result);
        }

        [Fact]
        public async Task ShouldDismissThePrompt()
        {
            Page.Dialog += (sender, e) =>
            {
                e.DialogInfo.Dismiss();
            };

            var result = await Page.EvaluateExpressionAsync<string>("prompt('question?')");
            Assert.Null(result);
        }
    }
}
