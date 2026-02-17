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
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Bidi;

internal class BidiFrameRealm(WindowRealm realm, BidiFrame frame) : BidiRealm(realm, frame.TimeoutSettings)
{
    private readonly WindowRealm _realm = realm;
    private readonly TaskQueue _puppeteerUtilQueue = new();
    private IJSHandle _puppeteerUtil;

    internal override IEnvironment Environment => frame;

    internal BidiFrame Frame => frame;

    internal WindowRealm WindowRealm => _realm;

    public static BidiFrameRealm From(WindowRealm realm, BidiFrame frame)
    {
        var frameRealm = new BidiFrameRealm(realm, frame);
        frameRealm.Initialize();
        return frameRealm;
    }

    public override async Task<IJSHandle> GetPuppeteerUtilAsync()
    {
        var scriptInjector = frame.BrowsingContext.Session.ScriptInjector;

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

    protected override void ThrowIfDetached()
    {
        if (Frame.Detached)
        {
            throw new PuppeteerException($"Execution Context is not available in detached frame \"{Frame.Url}\" (are you trying to evaluate?)");
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        _realm.Updated += (_, __) =>
        {
            (Environment as Frame)?.ClearDocumentHandle();
            _puppeteerUtil = null;
        };
    }
}

#endif
