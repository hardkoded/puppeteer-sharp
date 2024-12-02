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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// A LazyArg is an evaluation argument that will be resolved when the CDP call is built.
/// </summary>
/// <param name="context">Execution context.</param>
/// <returns>Resolved argument.</returns>
public delegate Task<LocalValue> BidiLazyArg(IPuppeteerUtilWrapper context);

internal class BidiRealm(Core.Realm realm, TimeoutSettings timeoutSettings) : Realm(timeoutSettings), IDisposable, IPuppeteerUtilWrapper
{
    private static readonly Regex _sourceUrlRegex = new(@"^[\x20\t]*//([@#])\s*sourceURL=\s{0,10}(\S*?)\s{0,10}$", RegexOptions.Multiline);

    public bool Disposed { get; private set; }

    public JSHandle InternalPuppeteerUtilHandle { get; set; }

    internal override IEnvironment Environment { get; }

    public void Dispose()
    {
        Disposed = true;
        TaskManager.TerminateAll(new PuppeteerException("waitForFunction failed: frame got detached."));
    }

    public virtual Task<IJSHandle> GetPuppeteerUtilAsync() => throw new NotImplementedException();

    internal override Task<IJSHandle> AdoptHandleAsync(IJSHandle handle) => throw new System.NotImplementedException();

    internal override Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> TransferHandleAsync(IJSHandle handle) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> EvaluateExpressionHandleAsync(string script) => throw new System.NotImplementedException();

    internal override Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args) => throw new System.NotImplementedException();

    internal override async Task<T> EvaluateExpressionAsync<T>(string script)
        => DeserializeEvaluationResult<T>(await EvaluateAsync(false, true, script).ConfigureAwait(false));

    internal override Task<JsonElement?> EvaluateExpressionAsync(string script) => throw new System.NotImplementedException();

    internal override async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        => DeserializeEvaluationResult<T>(await EvaluateAsync(false, false, script, args).ConfigureAwait(false));

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
            InternalPuppeteerUtilHandle = null;
            TaskManager.RerunAll();
        };
    }

    private async Task<EvaluateResultSuccess> EvaluateAsync(bool returnByValue, bool isExpression, string script, params object[] args)
    {
        var sourceUrlComment = ExecutionUtils.GetSourceUrlComment();
        var resultOwnership = returnByValue
            ? ResultOwnership.None
            : ResultOwnership.Root;

        var serializationOptions = new SerializationOptions();

        if (!returnByValue)
        {
            serializationOptions.MaxObjectDepth = 0;
            serializationOptions.MaxDomDepth = 0;
        }

        var functionDeclaration = _sourceUrlRegex.IsMatch(script)
            ? script
            : $"{script}\n{sourceUrlComment}\n";

        var options = new CallFunctionParameters();

        options.Arguments.AddRange(
            await Task.WhenAll(args.Select(FormatArgumentAsync).ToArray()).ConfigureAwait(false));
        options.ResultOwnership = resultOwnership;
        options.UserActivation = true;
        options.SerializationOptions = serializationOptions;

        EvaluateResult result;

        if (isExpression)
        {
            result = await realm.EvaluateAsync(
                functionDeclaration,
                true,
                options).ConfigureAwait(false);
        }
        else
        {
            result = await realm.CallFunctionAsync(
                functionDeclaration,
                true,
                options).ConfigureAwait(false);
        }

        if (result.ResultType == EvaluateResultType.Exception)
        {
            // TODO: Improve text details
            throw new EvaluateException(((EvaluateResultException)result).ExceptionDetails.Text);
        }

        return result as EvaluateResultSuccess;
    }

    private T DeserializeEvaluationResult<T>(EvaluateResultSuccess result)
    {
        // Convert known types first
        if (typeof(T) == typeof(int))
        {
            return (T)(object)Convert.ToInt32(result.Result.Value, CultureInfo.InvariantCulture);
        }

        switch (result.Result.Type)
        {
            case "number":
                // TODO: Way too many todos here.
                return (T)result.Result.Value;
        }

        return default;
    }

    private async Task<LocalValue> FormatArgumentAsync(object arg)
    {
        if (arg is TaskCompletionSource<object> tcs)
        {
            arg = await tcs.Task.ConfigureAwait(false);
        }

        if (arg is BidiLazyArg lazyArg)
        {
            arg = await lazyArg(this).ConfigureAwait(false);
        }

        switch (arg)
        {
            case BigInteger big:
                return LocalValue.BigInt(big);

            case int integer when integer == -0:
                return LocalValue.NegativeZero;

            case double d:
                if (double.IsPositiveInfinity(d))
                {
                    return LocalValue.Infinity;
                }

                if (double.IsNegativeInfinity(d))
                {
                    return LocalValue.NegativeInfinity;
                }

                if (double.IsNaN(d))
                {
                    return LocalValue.NaN;
                }

                break;
            case string stringValue:
                return LocalValue.String(stringValue);
            case int intValue:
                return LocalValue.Number(intValue);
            case bool boolValue:
                return LocalValue.Boolean(boolValue);
            case long longValue:
                return LocalValue.Number(longValue);
            case float floatValue:
                return LocalValue.Number(floatValue);
            case IEnumerable enumerable:
                var list = new List<LocalValue>();
                foreach (var item in enumerable)
                {
                    list.Add(await FormatArgumentAsync(item).ConfigureAwait(false));
                }

                return LocalValue.Array(list);
            case IJSHandle objectHandle:
                // TODO: Implement this
                return null;

                // TODO: Cover the rest of the cases
        }

        return null;
    }
}
