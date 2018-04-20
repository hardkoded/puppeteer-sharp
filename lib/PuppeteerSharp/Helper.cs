using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    internal class Helper
    {
        internal static string EvaluationString(string fun, params object[] args)
        {
            return $"({fun})({string.Join(",", args.Select(SerializeArgument))})";

            string SerializeArgument(object arg)
            {
                if (arg == null) return "undefined";
                return JsonConvert.SerializeObject(arg);
            }
        }

        internal static object ValueFromRemoteObject(dynamic remoteObject)
        {
            if (remoteObject.unserializableValue != null)
            {
                switch (remoteObject.unserializableValue.ToString())
                {
                    case "-0": return -0;
                    case "NaN": return double.NaN;
                    case "Infinity": return double.PositiveInfinity;
                    case "-Infinity": return double.NegativeInfinity;
                    default:
                        throw new Exception("Unsupported unserializable value: " + remoteObject.unserializableValue);
                }
            }

            if (remoteObject.value == null)
            {
                return null;
            }

            // https://chromedevtools.github.io/devtools-protocol/tot/Runtime#type-RemoteObject
            string objectValue = remoteObject.value.ToString();
            string objectType = remoteObject.type.ToString();
            
            switch (objectType)
            {
                case "object":
                    return JsonConvert.DeserializeObject<T>(objectValue);
                case "undefined": 
                    return null;
                case "number":
                    switch (Type.GetTypeCode(typeof(T)))
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            return int.Parse(objectValue);
                        default:
                            return float.Parse(objectValue);
                    }
                case "boolean": 
                    return bool.Parse(objectValue);
                case "bigint": 
                    return double.Parse(objectValue);
                default: // string, symbol, function
                    return objectValue;
            }
        }

        internal static async Task ReleaseObject(Session client, dynamic remoteObject)
        {
            if (remoteObject.objectId == null)
                return;
            try
            {
                await client.SendAsync("Runtime.releaseObject", new { remoteObject.objectId });
            }
            catch (Exception ex)
            {
                // Exceptions might happen in case of a page been navigated or closed.
                // Swallow these since they are harmless and we don't leak anything in this case.
                Console.WriteLine(ex.ToString());
            }
        }

        internal static string GetExceptionMessage(EvaluateExceptionDetails exceptionDetails)
        {
            if (exceptionDetails.Exception != null)
            {
                return exceptionDetails.Exception.Description;
            }
            var message = exceptionDetails.Text;
            if (exceptionDetails.StackTrace != null)
            {
                foreach (var callframe in exceptionDetails.StackTrace.CallFrames)
                {
                    var location = $"{callframe.Url}:{callframe.LineNumber}:{callframe.ColumnNumber}";
                    var functionName = string.IsNullOrEmpty(callframe.FunctionName) ? "<anonymous>" : callframe.FunctionName;
                    message += $"\n at ${functionName} (${location})";
                }
            }
            return message;
        }
    }
}