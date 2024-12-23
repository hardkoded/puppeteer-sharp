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
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

/// <summary>
/// A BidiLazyArg is an evaluation argument that will be resolved when the CDP call is built.
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
        => DeserializeResult<T>((await EvaluateAsync(true, true, script).ConfigureAwait(false)).Result.Value);

    internal override Task<JsonElement?> EvaluateExpressionAsync(string script) => throw new System.NotImplementedException();

    internal override async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        => DeserializeResult<T>((await EvaluateAsync(true, false, script, args).ConfigureAwait(false)).Result.Value);

    internal override async Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
        => DeserializeResult<JsonElement?>((await EvaluateAsync(true, false, script, args).ConfigureAwait(false)).Result.Value);

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

    private T DeserializeResult<T>(object result)
    {
        if (result is null)
        {
            return default;
        }

        if (typeof(T) == typeof(JsonElement?))
        {
            return (T)(object)JsonSerializer.SerializeToElement(result);
        }

        // Convert known types first
        if (typeof(T) == typeof(int))
        {
            return (T)(object)Convert.ToInt32(result, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(double))
        {
            return (T)(object)Convert.ToDouble(result, CultureInfo.InvariantCulture);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)result;
        }

        if (typeof(T) == typeof(bool))
        {
            return (T)(object)Convert.ToBoolean(result, CultureInfo.InvariantCulture);
        }

        if (typeof(T).IsArray)
        {
            // Get the element type of the array
            var elementType = typeof(T).GetElementType();

            if (elementType != null && result is IEnumerable enumerable)
            {
                // Create a list of the element type
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                // Iterate over the input and add converted items to the list
                foreach (var item in enumerable)
                {
                    var itemToSerialize = item;

                    if (item is RemoteValue remoteValue)
                    {
                        itemToSerialize = remoteValue.Value;
                    }

                    // Maybe there is a better way to do this.
                    var deserializedItem = typeof(BidiRealm)
                        .GetMethod(nameof(DeserializeResult), BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.MakeGenericMethod(elementType)
                        .Invoke(this, [itemToSerialize]);

                    list.Add(deserializedItem);
                }

                // Convert the list to an array
                return (T)list.GetType().GetMethod("ToArray")!.Invoke(list, null)!;
            }
        }

        return (T)result;
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

        if (arg is null)
        {
            return LocalValue.Null;
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
