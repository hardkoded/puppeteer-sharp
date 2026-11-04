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
using System.Linq;
using System.Threading.Tasks;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi.Core;

internal abstract class Realm(BrowsingContext context, string id, string origin) : IDisposable
{
    private string _reason;

    public event EventHandler<ClosedEventArgs> Destroyed;

    public event EventHandler Updated;

    public string Id { get; protected set; } = id;

    public string Origin { get; protected set; } = origin;

    public bool Disposed { get; private set; }

    public virtual ContextTarget Target => new(Id);

    public abstract Session Session { get; }

    // Just for testing purposes
    internal BrowsingContext Context => context;

    public void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }

    public virtual void Dispose()
    {
        _reason ??=
            "Realm already destroyed, probably because all associated browsing contexts closed.";
        OnDestroyed();
        Disposed = true;
    }

    public Task<EvaluateResult> EvaluateAsync(
        string functionDeclaration,
        bool awaitPromise,
        CallFunctionParameters options = null)
    {
        var parameters = new EvaluateCommandParameters(functionDeclaration, Target, awaitPromise);

        if (options != null)
        {
            parameters.ResultOwnership = options.ResultOwnership;
            parameters.UserActivation = options.UserActivation;
            parameters.SerializationOptions = options.SerializationOptions;
        }

        return Session.Driver.Script.EvaluateAsync(parameters);
    }

    public Task<EvaluateResult> CallFunctionAsync(
        string functionDeclaration,
        bool awaitPromise,
        CallFunctionParameters options = null)
    {
        var parameters = new CallFunctionCommandParameters(functionDeclaration, Target, awaitPromise);

        if (options != null)
        {
            parameters.Arguments.AddRange(options.Arguments);
            parameters.ResultOwnership = options.ResultOwnership;
            parameters.UserActivation = options.UserActivation;
            parameters.SerializationOptions = options.SerializationOptions;
        }

        return Session.Driver.Script.CallFunctionAsync(parameters);
    }

    public Task DisownAsync(string[] handleIds)
        => Session.Driver.Script.DisownAsync(new DisownCommandParameters(Target, handleIds));

    protected virtual void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

    private void OnDestroyed() => Destroyed?.Invoke(this, new ClosedEventArgs(_reason));
}

#endif
