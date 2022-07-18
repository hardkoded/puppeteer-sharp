using System;
using System.Numerics;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Helpers.Json;
using CefSharp.DevTools.Dom.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom.Helpers
{
    internal class RemoteObjectHelper
    {
        internal static object ValueFromRemoteObject<T>(RemoteObject remoteObject, bool stringify = false)
        {
            var unserializableValue = remoteObject.UnserializableValue;

            if (unserializableValue != null)
            {
                return ValueFromUnserializableValue(remoteObject, unserializableValue);
            }

            if (stringify)
            {
                if (remoteObject.Type == RemoteObjectType.Undefined)
                {
                    return "undefined";
                }

                if (remoteObject.Value == null)
                {
                    return "null";
                }
            }

            var value = remoteObject.Value;

            if (value == null)
            {
                return default(T);
            }

            return typeof(T) == typeof(JToken) ? value : ValueFromType<T>(value, remoteObject.Type, stringify);
        }

        private static object ValueFromType<T>(JToken value, RemoteObjectType objectType, bool stringify = false)
        {
            if (stringify)
            {
                switch (objectType)
                {
                    case RemoteObjectType.Object:
                        return value.ToObject<T>(true);
                    case RemoteObjectType.Undefined:
                        return "undefined";
                    case RemoteObjectType.Number:
                        return value.Value<T>();
                    case RemoteObjectType.Boolean:
                        return value.Value<bool>();
                    case RemoteObjectType.Bigint:
                        return value.Value<double>();
                    default: // string, symbol, function
                        return value.ToObject<T>();
                }
            }
            else
            {
                switch (objectType)
                {
                    case RemoteObjectType.Object:
                        return value.ToObject<T>(true);
                    case RemoteObjectType.Undefined:
                        return null;
                    case RemoteObjectType.Number:
                        return value.Value<T>();
                    case RemoteObjectType.Boolean:
                        return value.Value<bool>();
                    case RemoteObjectType.Bigint:
                        return value.Value<double>();
                    default: // string, symbol, function
                        return value.ToObject<T>();
                }
            }
        }

        private static object ValueFromUnserializableValue(RemoteObject remoteObject, string unserializableValue)
        {
            if (remoteObject.Type == RemoteObjectType.Bigint &&
                                decimal.TryParse(remoteObject.UnserializableValue.Replace("n", string.Empty), out var decimalValue))
            {
                return new BigInteger(decimalValue);
            }
            switch (unserializableValue)
            {
                case "-0":
                    return -0;
                case "NaN":
                    return double.NaN;
                case "Infinity":
                    return double.PositiveInfinity;
                case "-Infinity":
                    return double.NegativeInfinity;
                default:
                    throw new Exception("Unsupported unserializable value: " + unserializableValue);
            }
        }

        internal static async Task ReleaseObjectAsync(DevToolsConnection client, RemoteObject remoteObject, ILogger logger)
        {
            if (remoteObject.ObjectId == null)
            {
                return;
            }

            try
            {
                await client.SendAsync("Runtime.releaseObject", new RuntimeReleaseObjectRequest
                {
                    ObjectId = remoteObject.ObjectId
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Exceptions might happen in case of a page been navigated or closed.
                // Swallow these since they are harmless and we don't leak anything in this case.
                logger.LogWarning(ex.ToString());
            }
        }
    }
}
