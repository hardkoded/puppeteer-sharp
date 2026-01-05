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

    public async Task DestroyHandlesAsync(params BidiJSHandle[] handles)
    {
        if (Disposed)
        {
            return;
        }

        var handleIds = handles
            .Select((handle) => handle.Id)
            .Where(id => !string.IsNullOrEmpty(id)).ToArray();

        if (handleIds.Length == 0)
        {
            return;
        }

        try
        {
            await realm.DisownAsync(handleIds).ConfigureAwait(false);
        }
        catch
        {
            // TODO: Add Logger
        }
    }

    internal override async Task<IJSHandle> AdoptHandleAsync(IJSHandle handle)
    {
        try
        {
            return await EvaluateFunctionHandleAsync(
                @"node => {
                return node;
            }",
                handle).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    internal override Task<IElementHandle> AdoptBackendNodeAsync(object backendNodeId) => throw new System.NotImplementedException();

    internal override async Task<IJSHandle> TransferHandleAsync(IJSHandle handle)
    {
        if (handle is JSHandle handleImpl && handleImpl.Realm == this)
        {
            return handle;
        }

        var transferredHandle = AdoptHandleAsync(handle);
        await handle.DisposeAsync().ConfigureAwait(false);
        return await transferredHandle.ConfigureAwait(false);
    }

    internal override async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
        => CreateHandleAsync(await EvaluateAsync(false, true, script).ConfigureAwait(false));

    internal override async Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
        => CreateHandleAsync(await EvaluateAsync(false, false, script, args).ConfigureAwait(false));

    internal override async Task<T> EvaluateExpressionAsync<T>(string script)
        => DeserializeResult<T>((await EvaluateAsync(true, true, script).ConfigureAwait(false)).Result.Value);

    internal override async Task EvaluateExpressionAsync(string script)
        => await EvaluateAsync(true, true, script).ConfigureAwait(false);

    internal override async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
        => DeserializeResult<T>((await EvaluateAsync(true, false, script, args).ConfigureAwait(false)).Result.Value);

    internal override async Task EvaluateFunctionAsync(string script, params object[] args)
    {
        await EvaluateAsync(true, false, script, args).ConfigureAwait(false);
    }

    internal IJSHandle CreateHandle(RemoteValue remoteValue)
    {
        if (
            remoteValue.Type is "node" or "window" &&
            this is BidiFrameRealm)
        {
            return BidiElementHandle.From(remoteValue, this);
        }

        return BidiJSHandle.From(remoteValue, this);
    }

    protected virtual void ThrowIfDetached()
    {
        // Base implementation does nothing
        // BidiFrameRealm will override this to check if frame is detached
    }

    protected virtual void Initialize()
    {
        realm.Destroyed += (_, e) =>
        {
            TaskManager.TerminateAll(new PuppeteerException(e.Reason));
            Dispose();
        };

        realm.Updated += (_, _) =>
        {
            InternalPuppeteerUtilHandle = null;
            TaskManager.RerunAll();
        };
    }

    private IJSHandle CreateHandleAsync(EvaluateResultSuccess evaluateResult)
    {
        var result = evaluateResult.Result;

        if (
            result.Type is "node" or "window" &&
            this is BidiFrameRealm)
        {
            return BidiElementHandle.From(result, this);
        }

        return BidiJSHandle.From(result, this);
    }

    private async Task<EvaluateResultSuccess> EvaluateAsync(bool returnByValue, bool isExpression, string script, params object[] args)
    {
        ThrowIfDetached();
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

        try
        {
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
        }
        catch (WebDriverBiDi.WebDriverBiDiException ex)
            when (ex.Message.Contains("no such frame") || ex.Message.Contains("DiscardedBrowsingContextError"))
        {
            throw new EvaluationFailedException($"Protocol error ({ex.Message})", ex);
        }
        catch (WebDriverBiDi.WebDriverBiDiException ex)
            when (ex.Message.Contains("no such handle"))
        {
            throw new PuppeteerException("Could not serialize referenced object", ex);
        }

        if (result.ResultType == EvaluateResultType.Exception)
        {
            // TODO: Improve text details
            throw new EvaluationFailedException(((EvaluateResultException)result).ExceptionDetails.Text);
        }

        return result as EvaluateResultSuccess;
    }

    private T DeserializeResult<T>(object result)
    {
        if (result is null)
        {
            return default;
        }

        if (typeof(T) == typeof(object))
        {
            // When requesting object type, we need to deserialize complex types properly
            return (T)DeserializeToObject(result);
        }

        // Handle JsonElement? - return default since BiDi doesn't use JsonElement
        if (typeof(T) == typeof(JsonElement?))
        {
            return default;
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

        if (typeof(T) == typeof(decimal))
        {
            return (T)(object)Convert.ToDecimal(result, CultureInfo.InvariantCulture);
        }

        // Handle JsonElement - serialize the result to JSON and parse it back as JsonElement
        if (typeof(T) == typeof(JsonElement))
        {
            var json = JsonSerializer.Serialize(result);
            using var document = JsonDocument.Parse(json);
            return (T)(object)document.RootElement.Clone();
        }

        if (result is RemoteValueDictionary remoteValueDictionary)
        {
            return DeserializeRemoteValueDictionary<T>(remoteValueDictionary);
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

    private T DeserializeRemoteValueDictionary<T>(RemoteValueDictionary remoteValueDictionary)
    {
        var type = typeof(T);

        // Handle nullable value types (e.g., Point?)
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            // Recursively deserialize to the underlying type and convert back to nullable
            var deserializedValue = typeof(BidiRealm)
                .GetMethod(nameof(DeserializeRemoteValueDictionary), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(underlyingType)
                .Invoke(this, [remoteValueDictionary]);
            return (T)deserializedValue;
        }

        // Create an instance of T
        var instance = Activator.CreateInstance<T>();

        // Box the instance if it's a value type (struct) to properly handle property setting
        object boxedInstance = instance;

        // Iterate through the dictionary and populate properties
        foreach (var entry in remoteValueDictionary)
        {
            var propertyName = entry.Key?.ToString();
            if (string.IsNullOrEmpty(propertyName))
            {
                continue;
            }

            var remoteValue = entry.Value;

            // Find the property on the type (case-insensitive)
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property != null && property.CanWrite)
            {
                // Get the value from RemoteValue
                var value = remoteValue.Value;

                // Recursively deserialize the value to the property type
                var deserializedValue = typeof(BidiRealm)
                    .GetMethod(nameof(DeserializeResult), BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(property.PropertyType)
                    .Invoke(this, [value]);

                // Set the property value on the boxed instance
                property.SetValue(boxedInstance, deserializedValue);
            }
        }

        // Unbox back to T
        return (T)boxedInstance;
    }

    /// <summary>
    /// Deserializes a BiDi result value to a plain .NET object.
    /// Handles RemoteValueDictionary, RemoteValue, and primitive types.
    /// Returns null for the entire result if circular references are detected.
    /// </summary>
    private object DeserializeToObject(object value)
    {
        var hasCircularRef = false;
        var result = DeserializeToObjectInternal(value, ref hasCircularRef);

        // If circular references were detected, return null for the entire result
        // This matches CDP behavior where circular objects cannot be serialized
        if (hasCircularRef)
        {
            return null;
        }

        return result;
    }

    private object DeserializeToObjectInternal(object value, ref bool hasCircularRef)
    {
        if (value is null)
        {
            return null;
        }

        // Handle RemoteValueDictionary (represents objects/maps)
        if (value is RemoteValueDictionary dict)
        {
            var result = new Dictionary<string, object>();
            foreach (var entry in dict)
            {
                var key = entry.Key?.ToString();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                var remoteValue = entry.Value;

                // If RemoteValue has no value, it's a circular reference or unresolvable reference
                if (!remoteValue.HasValue)
                {
                    hasCircularRef = true;
                    result[key] = null;
                }
                else
                {
                    result[key] = DeserializeToObjectInternal(remoteValue.Value, ref hasCircularRef);
                }
            }

            return result;
        }

        // Handle RemoteValue directly (for nested values)
        if (value is RemoteValue remoteVal)
        {
            // If RemoteValue has no value, it's a circular reference
            if (!remoteVal.HasValue)
            {
                hasCircularRef = true;
                return null;
            }

            return DeserializeToObjectInternal(remoteVal.Value, ref hasCircularRef);
        }

        // Handle IEnumerable for arrays (but not strings)
        if (value is IEnumerable enumerable && value is not string)
        {
            var list = new List<object>();
            foreach (var item in enumerable)
            {
                if (item is RemoteValue rv)
                {
                    if (!rv.HasValue)
                    {
                        hasCircularRef = true;
                        list.Add(null);
                    }
                    else
                    {
                        list.Add(DeserializeToObjectInternal(rv.Value, ref hasCircularRef));
                    }
                }
                else
                {
                    list.Add(DeserializeToObjectInternal(item, ref hasCircularRef));
                }
            }

            return list.ToArray();
        }

        // For primitive types (string, numbers, bool, etc.), return as-is
        return value;
    }

    private async Task<ArgumentValue> FormatArgumentAsync(object arg)
    {
        if (arg is TaskCompletionSource<object> tcs)
        {
            arg = await tcs.Task.ConfigureAwait(false);
        }

        if (arg is LazyArg lazyArg)
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

            case decimal decimalValue:
                return LocalValue.Number(decimalValue);

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
                var list = new List<ArgumentValue>();
                foreach (var item in enumerable)
                {
                    list.Add(await FormatArgumentAsync(item).ConfigureAwait(false));
                }

                return LocalValue.Array(list.Select(el => el as LocalValue).ToList());
            case BidiJSHandle objectHandle:
                ValidateHandle(objectHandle);

                // If the handle doesn't have a valid handle ID (e.g., from console log args),
                // serialize its value directly instead of using it as a remote reference
                if (string.IsNullOrEmpty(objectHandle.Id))
                {
                    return SerializeRemoteValue(objectHandle.RemoteValue);
                }

                return objectHandle.RemoteValue.ToRemoteReference();
            case BidiElementHandle elementHandle:
                ValidateHandle(elementHandle.BidiJSHandle);
                return elementHandle.Value.ToRemoteReference();

                // TODO: Cover the rest of the cases
        }

        return null;
    }

    private void ValidateHandle(BidiJSHandle handle)
    {
        if (handle.Realm != this)
        {
            if (handle.Realm is not BidiFrameRealm || this is not BidiFrameRealm)
            {
                throw new PuppeteerException(
                    "Trying to evaluate JSHandle from different global types. Usually this means you're using a handle from a worker in a page or vice versa.");
            }

            if (handle.Realm.Environment != Environment)
            {
                throw new PuppeteerException(
                    "JSHandles can be evaluated only in the context they were created!");
            }
        }

        if (handle.Disposed)
        {
            throw new PuppeteerException("JSHandle is disposed!");
        }
    }

    private LocalValue SerializeRemoteValue(RemoteValue remoteValue)
    {
        return remoteValue.Type switch
        {
            "undefined" => LocalValue.Undefined,
            "null" => LocalValue.Null,
            "string" => LocalValue.String((string)remoteValue.Value),
            "number" => remoteValue.Value switch
            {
                long l => LocalValue.Number(l),
                double d when double.IsPositiveInfinity(d) => LocalValue.Infinity,
                double d when double.IsNegativeInfinity(d) => LocalValue.NegativeInfinity,
                double d when double.IsNaN(d) => LocalValue.NaN,
                double d => LocalValue.Number(d),
                _ => LocalValue.Number(Convert.ToDouble(remoteValue.Value, CultureInfo.InvariantCulture)),
            },
            "boolean" => LocalValue.Boolean((bool)remoteValue.Value),
            "bigint" => LocalValue.BigInt(BigInteger.Parse((string)remoteValue.Value, CultureInfo.InvariantCulture)),
            "array" => SerializeRemoteValueArray(remoteValue.Value),
            "object" => SerializeRemoteValueObject(remoteValue.Value),
            _ => throw new PuppeteerException($"Cannot serialize RemoteValue of type '{remoteValue.Type}' without a handle"),
        };
    }

    private LocalValue SerializeRemoteValueArray(object value)
    {
        var items = new List<LocalValue>();
        if (value is IEnumerable<RemoteValue> remoteValues)
        {
            foreach (var item in remoteValues)
            {
                items.Add(SerializeRemoteValue(item));
            }
        }

        return LocalValue.Array(items);
    }

    private LocalValue SerializeRemoteValueObject(object value)
    {
        var dict = new Dictionary<string, LocalValue>();
        if (value is RemoteValueDictionary remoteDict)
        {
            foreach (var kvp in remoteDict)
            {
                var key = kvp.Key switch
                {
                    string s => s,
                    _ => kvp.Key.ToString(),
                };
                var val = SerializeRemoteValue(kvp.Value);
                dict[key] = val;
            }
        }

        return LocalValue.Object(dict);
    }
}
