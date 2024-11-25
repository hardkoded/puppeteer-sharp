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
using System.Text.Json;
using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

internal class BidiRealm(WindowRealm realm, TimeoutSettings timeoutSettings) : Realm(timeoutSettings), IDisposable
{
    private bool _disposed;
    private BidiJSHandle _internalPuppeteerUtil { get; set; }

    internal override IEnvironment Environment { get; }

    public void Dispose()
    {
        _disposed = true;
        TaskManager.TerminateAll(new PuppeteerException("waitForFunction failed: frame got detached."));
    }

    internal override Task<IJSHandle> AdoptHandleAsync(IJSHandle handle) => throw new System.NotImplementedException();

    internal override Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> TransferHandleAsync(IJSHandle handle) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> EvaluateExpressionHandleAsync(string script) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args) => throw new System.NotImplementedException();

    internal override Task<T> EvaluateExpressionAsync<T>(string script) => throw new System.NotImplementedException();

    internal override Task<JsonElement?> EvaluateExpressionAsync(string script) => throw new System.NotImplementedException();

    internal override Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
    {
        var sourceUrlComment = ExecutionUtils.GetSourceUrlComment();
        const resultOwnership = returnByValue
            ? Bidi.Script.ResultOwnership.None
            : Bidi.Script.ResultOwnership.Root;
    }

    internal override Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args) => throw new System.NotImplementedException();

    protected virtual void Initialize()
    {
        realm.Destroyed += (_, e) =>
        {
            TaskManager.TerminateAll(new PuppeteerException(e.Reason));
            Dispose();
        };

        realm.Updated += (_, __) =>
        {
            _internalPuppeteerUtil = null;
            TaskManager.RerunAll();
        };
    }
}
