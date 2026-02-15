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

using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

internal class BidiWorkerRealm : BidiRealm
{
    private readonly DedicatedWorkerRealm _realm;
    private readonly BidiWebWorker _worker;
    private readonly TaskQueue _puppeteerUtilQueue = new();
    private IJSHandle _puppeteerUtil;

    private BidiWorkerRealm(DedicatedWorkerRealm realm, BidiWebWorker worker)
        : base(realm, worker.TimeoutSettings)
    {
        _realm = realm;
        _worker = worker;
    }

    internal override IEnvironment Environment => _worker;

    internal BidiWebWorker Worker => _worker;

    public static BidiWorkerRealm From(DedicatedWorkerRealm realm, BidiWebWorker worker)
    {
        var workerRealm = new BidiWorkerRealm(realm, worker);
        workerRealm.Initialize();
        return workerRealm;
    }

    public override async Task<IJSHandle> GetPuppeteerUtilAsync()
    {
        var scriptInjector = _realm.Session.ScriptInjector;

        await _puppeteerUtilQueue.Enqueue(async () =>
        {
            if (_puppeteerUtil == null)
            {
                await scriptInjector.InjectAsync(
                    async (script) =>
                    {
                        if (_puppeteerUtil != null)
                        {
                            await _puppeteerUtil.DisposeAsync().ConfigureAwait(false);
                        }

                        _puppeteerUtil = await EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
                    },
                    _puppeteerUtil == null).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);

        return _puppeteerUtil;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _realm.Destroyed += (sender, args) => Dispose();
        _realm.Updated += (sender, args) =>
        {
            // Reset PuppeteerUtil when the realm is updated
            _puppeteerUtil = null;
            TaskManager.RerunAll();
        };
    }
}

#endif
