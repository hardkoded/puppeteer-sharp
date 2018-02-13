using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PuppeteerSharp.Helpers
{
    public static class DynamicExtensions
    {
        public static bool ContainsProperty(dynamic obj, string property)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(property);
        }

        public static T GetOrDefault<T>(dynamic obj, string property, T defaultValue)
        {
            if (ContainsProperty(obj, property))
            {
                return (T)((IDictionary<string, object>)obj)[property];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
