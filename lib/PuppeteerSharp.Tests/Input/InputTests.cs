using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Input
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class InputTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldClickOnCheckboxInputAndToggle()
        {

        }

        [Fact]
        public async Task ShouldClickOnCheckboxLabelAndToggle()
        {

        }

        [Fact]
        public async Task ShouldFailToClickAMissingButton()
        {

        }

        [Fact]
        public async Task ShouldNotHangWithTouchEnabledViewports()
        {

        }

        [Fact]
        public async Task ShouldTypeIntoTheTextarea()
        {

        }

        [Fact]
        public async Task ShouldClickTheButtonAfterNavigation()
        {

        }

        [Fact]
        public async Task ShouldUploadTheFile()
        {

        }

        [Fact]
        public async Task ShouldMoveWithTheArrowKeys()
        {

        }

        [Fact]
        public async Task ShouldSendACharacterWithElementHandlePress()
        {

        }

        [Fact]
        public async Task ShouldSendACharacterWithSendCharacter()
        {

        }

        [Fact]
        public async Task ShouldReportShiftKey()
        {

        }

        [Fact]
        public async Task ShouldReportMultipleModifiers()
        {

        }

        [Fact]
        public async Task ShouldSendProperCodesWhileTyping()
        {

        }

        [Fact]
        public async Task ShouldSendProperCodesWhileTypingWithShift()
        {

        }

        [Fact]
        public async Task ShouldNotTypeCanceledEvents()
        {

        }

        [Fact]
        public async Task KeyboardModifier()
        {

        }

        [Fact]
        public async Task ShouldResizeTheTextarea()
        {

        }

        [Fact]
        public async Task ShouldScrollAndClickTheButton()
        {

        }

        [Fact]
        public async Task ShouldDoubleClickTheButton()
        {

        }

        [Fact]
        public async Task ShouldClickAPartiallyObscuredButton()
        {

        }

        [Fact]
        public async Task ShouldSelectTheTextWithMouse()
        {

        }

        [Fact]
        public async Task ShouldSelectTheTextByTripleClicking()
        {

        }

        [Fact]
        public async Task ShouldTriggerHoverState()
        {

        }

        [Fact]
        public async Task ShouldFireContextmenuEventOnRightClick()
        {

        }

        [Fact]
        public async Task ShouldSetModifierKeysOnClick()
        {

        }

        [Fact]
        public async Task ShouldSpecifyRepeatProperty()
        {

        }

        [Fact]
        public async Task ShouldClickLinksWhichCauseNavigation()
        {

        }

        [Fact]
        public async Task ShouldTweenMouseMovement()
        {

        }

        [Fact]
        public async Task ShouldTapTheButton()
        {

        }

        [Fact]
        public async Task ShouldReportTouches()
        {

        }

        [Fact]
        public async Task ShouldClickTheButtonInsideAnIframe()
        {

        }

        [Fact]
        public async Task ShouldClickTheButtonWithDeviceScaleFactorSet()
        {

        }

        [Fact]
        public async Task ShouldTypeAllKindsOfCharacters()
        {

        }

        [Fact]
        public async Task ShouldSpecifyLocation()
        {

        }

        [Fact]
        public async Task ShouldThrowOnUnknownKeys()
        {

        }
    }
}