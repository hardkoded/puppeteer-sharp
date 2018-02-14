using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PuppeteerSharp.Helpers
{
    public static class DynamicExtensions
    {
        public static bool ContainsProperty(object obj, string property)
        {
            if (obj is ExpandoObject)
            {
                return ((IDictionary<string, object>)obj).ContainsKey(property);
            }

            return obj.GetType().GetProperty(property) != null;
        }

        public static T GetOrDefault<T>(dynamic obj, string property, T defaultValue)
        {
            if (ContainsProperty(obj, property))
            {
                if (obj is ExpandoObject)
                {
                    return (T)((IDictionary<string, object>)obj)[property];
                }

                return (T)obj?.GetType().GetProperty(property)?.GetValue(obj, null);
            }

            return defaultValue;
        }
    }
}
