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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebDriverBiDi;
using WebDriverBiDi.Session;

namespace PuppeteerSharp.Bidi.Core;

internal class Session(BiDiDriver driver, NewCommandResult info) : IDisposable
{
    public event EventHandler<SessionEndArgs> Ended;

    public BiDiDriver Driver { get; } = driver;

    public NewCommandResult Info { get; } = info;

    public Browser Browser { get; private set; }

    public static async Task<Session> FromAsync(BiDiDriver driver, NewCommandParameters capabilities, ILoggerFactory loggerFactory)
    {
        var result = await driver.Session.NewSessionAsync(capabilities).ConfigureAwait(false);
        var session = new Session(driver, result);
        await session.InitializeAsync().ConfigureAwait(false);
        return session;
    }

    public void Dispose()
    {
    }

    public Task SubscribeAsync(string[] events, string[] contexts = null)
    {
        var args = new SubscribeCommandParameters();
        args.Events.AddRange(events);
        args.Contexts.AddRange(contexts ?? []);
        Driver.Session.SubscribeAsync(args);

        return Task.CompletedTask;
    }

    internal virtual void OnEnded(SessionEndArgs e) => Ended?.Invoke(this, e);

    private async Task InitializeAsync()
    {
        Browser = await Browser.From(this).ConfigureAwait(false);
    }
}
