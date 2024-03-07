// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
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

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TouchScreenTests;

public class TouchscreenPrototypeTapTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("touchscreen.spec", "Touchscreen Touchscreen.prototype.tap", "should work")]
    public async Task ShouldWork()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/input/touchscreen.html");
        await Page.TapAsync("button");

        var result = await Page.EvaluateExpressionAsync<TouchEvent[]>("allEvents");
        Assert.AreEqual(
            JsonConvert.SerializeObject(new[]
            {
                new TouchEvent()
                {
                    Type = "pointerdown",
                    X = 5,
                    Y= 5,
                    Width = 1,
                    Height = 1,
                    AltitudeAngle = Convert.ToDecimal(Math.PI / 2),
                    AzimuthAngle = 0,
                    Pressure = 0.5m,
                    PointerType = "touch",
                    TiltX = 0,
                    TiltY = 0,
                    Twist = 0,
                },
                new TouchEvent()
                {
                    Type ="touchstart",
                    ChangedTouches = new[]
                    {
                        new TouchEvent.Detail()
                        {
                            ClientX = 5,
                            ClientY = 5,
                            RadiusX = 0.5m,
                            RadiusY = 0.5m,
                            Force = 0.5m,
                        },
                    },
                    ActiveTouches = new[]
                    {
                        new TouchEvent.Detail()
                        {
                            ClientX = 5,
                            ClientY = 5,
                            RadiusX = 0.5m,
                            RadiusY = 0.5m,
                            Force = 0.5m,
                        },
                    }
                },
                new TouchEvent()
                {
                    Type = "pointerup",
                    X = 5,
                    Y = 5,
                    Width = 1,
                    Height = 1,
                    AltitudeAngle = Convert.ToDecimal(Math.PI / 2),
                    AzimuthAngle = 0,
                    Pressure = 0,
                    PointerType = "touch",
                    TiltX = 0,
                    TiltY = 0,
                    Twist = 0,
                },
                new TouchEvent()
                {
                    Type ="touchend",
                    ChangedTouches = new[]
                    {
                        new TouchEvent.Detail()
                        {
                            ClientX = 5,
                            ClientY = 5,
                            RadiusX = 0.5m,
                            RadiusY = 0.5m,
                            Force = 0.5m,
                        },
                    },
                },
                new TouchEvent()
                {
                    Type = "click",
                    X = 5,
                    Y = 5,
                    Width = 1,
                    Height = 1,
                    AltitudeAngle = Convert.ToDecimal(Math.PI / 2),
                    AzimuthAngle = 0,
                    Pressure = 0,
                    PointerType = "touch",
                    TiltX = 0,
                    TiltY = 0,
                    Twist = 0,
                },
            }), JsonConvert.SerializeObject(result));
    }
}
