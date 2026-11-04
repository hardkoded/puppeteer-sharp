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

#if !CDP_ONLY

using System;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;

namespace PuppeteerSharp.Bidi;

internal class BidiWebWorker : WebWorker
{
    private readonly BidiFrame _frame;
    private readonly BidiWorkerRealm _realm;

    private BidiWebWorker(BidiFrame frame, DedicatedWorkerRealm realm) : base(realm.Origin)
    {
        _frame = frame;
        _realm = BidiWorkerRealm.From(realm, this);
        RealmId = realm.Id;
    }

    public override ICDPSession Client => throw new NotSupportedException();

    internal override IsolatedWorld World => throw new NotSupportedException();

    internal BidiFrame Frame => _frame;

    internal TimeoutSettings TimeoutSettings => _frame.TimeoutSettings;

    internal string RealmId { get; }

    public override Task CloseAsync()
    {
        // BiDi doesn't support closing workers directly.
        // Workers are closed when they terminate themselves or when the owning page/frame closes.
        // This matches the upstream Puppeteer behavior where close() throws UnsupportedOperation.
        throw new NotSupportedException("WebWorker.CloseAsync() is not supported in BiDi protocol");
    }

    internal static BidiWebWorker From(BidiFrame frame, DedicatedWorkerRealm realm)
    {
        return new BidiWebWorker(frame, realm);
    }

    internal override Realm GetMainRealm() => _realm;
}

#endif
