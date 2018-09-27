using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Helpers
{
    internal class RemoteObjectHelper
    {
        internal static object ValueFromRemoteObject<T>(JToken remoteObject)
        {
            var unserializableValue = remoteObject[Constants.UNSERIALIZABLE_VALUE]?.AsString();

            if (unserializableValue != null)
            {
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

            var value = remoteObject[Constants.VALUE];

            if (value == null)
            {
                return null;
            }

            // https://chromedevtools.github.io/devtools-protocol/tot/Runtime#type-RemoteObject
            var objectType = remoteObject[Constants.TYPE].AsString();

            switch (objectType)
            {
                case "object":
                    return value.ToObject<T>();
                case "undefined":
                    return null;
                case "number":
                    return value.Value<T>();
                case "boolean":
                    return value.Value<bool>();
                case "bigint":
                    return value.Value<double>();
                default: // string, symbol, function
                    return value.ToObject<T>();
            }
        }

        internal static async Task ReleaseObject(CDPSession client, JToken remoteObject, ILogger logger)
        {
            var objectId = remoteObject[Constants.OBJECT_ID]?.AsString();

            if (objectId == null)
            {
                return;
            }

            try
            {
                await client.SendAsync("Runtime.releaseObject", new { objectId }).ConfigureAwait(false);
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