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

namespace PuppeteerSharp.Bidi.Core;

internal class Browser(Session session) : IDisposable
{
    public Session Session { get; } = session;

    public async Task<Browser> From(Session session)
    {
        var browser = new Browser(session);
        await browser.InitializeAsync().ConfigureAwait(false);
        return browser;
    }

    private async Task InitializeAsync()
    {
        var sessionEmitter = this.#disposables.use(
            new EventEmitter(this.session)
        );
        sessionEmitter.once('ended', ({reason}) => {
            this.dispose(reason);
        });

        sessionEmitter.on('script.realmCreated', info => {
            if (info.type !== 'shared-worker') {
                return;
            }
            this.#sharedWorkers.set(
                info.realm,
                SharedWorkerRealm.from(this, info.realm, info.origin)
            );
        });

        await this.#syncUserContexts();
        await this.#syncBrowsingContexts();
    }
}
