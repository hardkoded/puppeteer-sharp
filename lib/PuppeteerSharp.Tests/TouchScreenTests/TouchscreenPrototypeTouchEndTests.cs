
// * MIT License
//  *
//  * Copyright (c) Dario Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TouchScreenTests;

public class TouchscreenPrototypeTouchEndTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("touchscreen.spec", "Touchscreen Touchscreen.prototype.touchEnd", "should throw when ending touch through Touchscreeen that was already ended")]
    public async Task ShouldThrowWhenEndingTouchThroughTouchscreenThatWasAlreadyEnded()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/input/touchscreen.html");

        await Page.Touchscreen.TouchStartAsync(100, 100);
        await Page.Touchscreen.TouchMoveAsync(50, 100);
        await Page.Touchscreen.TouchEndAsync();

        var exception = Assert.ThrowsAsync<PuppeteerException>(async () =>
        {
            await Page.Touchscreen.TouchEndAsync();
        });

        Assert.That(exception.Message, Does.Contain("Must start a new Touch first"));
    }
}
