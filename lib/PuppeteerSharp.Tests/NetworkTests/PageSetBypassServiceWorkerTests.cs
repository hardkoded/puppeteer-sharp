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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests;

public class PageSetBypassServiceWorkerTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("network.spec", "network Page.setBypassServiceWorker", "bypass for network")]
    public async Task BypassForNetwork()
    {
        var responses = new Dictionary<string, IResponse>();

        Page.Response += (_, e) =>
        {
            if (!TestUtils.IsFavicon(e.Response.Request))
            {
                responses[e.Response.Url.Split('/').Last()] = e.Response;
            }
        };

        await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html",
            WaitUntilNavigation.Networkidle2);

        await Page.EvaluateFunctionAsync($@"async () => {{
            return await globalThis.activationPromise;
        }}");

        await Page.ReloadAsync(new NavigationOptions()
        {
            WaitUntil = [WaitUntilNavigation.Networkidle2],
        });

        Assert.False(Page.IsServiceWorkerBypassed);
        Assert.AreEqual(2, responses.Count);
        Assert.AreEqual(HttpStatusCode.OK, responses["sw.html"].Status);
        Assert.True(responses["sw.html"].FromServiceWorker);
        Assert.AreEqual(HttpStatusCode.OK, responses["style.css"].Status);
        Assert.True(responses["style.css"].FromServiceWorker);

        await Page.SetBypassServiceWorkerAsync(true);
        await Page.ReloadAsync(new NavigationOptions()
        {
            WaitUntil = [WaitUntilNavigation.Networkidle2],
        });

        Assert.True(Page.IsServiceWorkerBypassed);
        Assert.AreEqual(HttpStatusCode.OK, responses["sw.html"].Status);
        Assert.False(responses["sw.html"].FromServiceWorker);
        Assert.AreEqual(HttpStatusCode.OK, responses["style.css"].Status);
        Assert.False(responses["style.css"].FromServiceWorker);
    }
}
