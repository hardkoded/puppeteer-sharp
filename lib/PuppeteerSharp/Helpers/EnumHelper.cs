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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Helpers;

internal static class EnumHelper
{
    public static string[] GetNames<TEnum>()
        where TEnum : struct, Enum =>
#if NET8_0_OR_GREATER
        Enum.GetNames<TEnum>();
#else
        Enum.GetNames(typeof(TEnum));
#endif

    public static TEnum[] GetValues<TEnum>()
        where TEnum : struct, Enum =>
#if NET8_0_OR_GREATER
        Enum.GetValues<TEnum>();
#else
        (TEnum[])Enum.GetValues(typeof(TEnum));
#endif

    public static TEnum Parse<TEnum>(string value, bool ignoreCase)
        where TEnum : struct, Enum =>
#if NET8_0_OR_GREATER
        Enum.Parse<TEnum>(value, ignoreCase);
#else
        (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
#endif

    public static TEnum ToEnum<TEnum>(this string value)
        where TEnum : struct, Enum
        => StringToEnum<TEnum>.Cache[value];

    public static string ToValueString<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => EnumToString<TEnum>.Cache[value];

    public static TEnum FromValueString<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (StringToEnum<TEnum>.Cache.TryGetValue(value, out var enumValue))
        {
            return enumValue;
        }

        var defaultEnumAttribute = typeof(TEnum).GetCustomAttribute<DefaultEnumValueAttribute>();
        if (defaultEnumAttribute != null)
        {
            return (TEnum)(object)defaultEnumAttribute.Value;
        }

        throw new ArgumentException($"Unknown value '{value}' for enum {typeof(TEnum).Name}");
    }

    private static class StringToEnum<TEnum>
        where TEnum : struct, Enum
    {
        public static readonly IReadOnlyDictionary<string, TEnum> Cache = Compute();

        private static Dictionary<string, TEnum> Compute()
        {
            var names = GetNames<TEnum>();
            var dictionary = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in names)
            {
                var field = typeof(TEnum).GetField(name);
                var fieldValue = (TEnum)field.GetValue(null);
                dictionary[name] = fieldValue;
                if (field.GetCustomAttribute<EnumMemberAttribute>()?.Value is { } enumMember)
                {
                    dictionary[enumMember] = fieldValue;
                }
            }

            return dictionary;
        }
    }

    private static class EnumToString<TEnum>
        where TEnum : struct, Enum
    {
        public static readonly IReadOnlyDictionary<TEnum, string> Cache = Compute();

        private static Dictionary<TEnum, string> Compute()
        {
            var names = GetNames<TEnum>();
            var dictionary = new Dictionary<TEnum, string>();
            foreach (var name in names)
            {
                var field = typeof(TEnum).GetField(name);
                var valueName = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? name;
                var fieldValue = (TEnum)field.GetValue(null);
                dictionary[fieldValue] = valueName;
            }

            return dictionary;
        }
    }
}
