/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace PuppeteerSharp.Helpers;

internal static class EnumHelper
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Enum, string>> EnumToStringCache
        = new();

    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, Enum>> StringToEnumCache
        = new();

    public static string ToValueString<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        var enumValues = EnumToStringCache.GetOrAdd(typeof(TEnum), type =>
        {
            var names = Enum.GetNames(type);
            var dictionary = new Dictionary<Enum, string>();
            foreach (var t in names)
            {
                var field = type.GetField(t);
                var valueName = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? t;
                var value = (TEnum)field.GetValue(null);
                dictionary[value] = valueName;
            }

            return dictionary;
        });

        return enumValues[value];
    }

    public static TEnum FromValueString<TEnum>(string value)
        where TEnum : struct, Enum
    {
        var enumValues = StringToEnumCache.GetOrAdd(typeof(TEnum), type =>
        {
            var names = Enum.GetNames(type);
            var dictionary = new Dictionary<string, Enum>();
            foreach (var valueName in names)
            {
                var field = type.GetField(valueName);
                var value = (TEnum)field.GetValue(null);
                dictionary[valueName] = value;
                if (field.GetCustomAttribute<EnumMemberAttribute>()?.Value is string enumMember)
                {
                    dictionary[enumMember] = value;
                }
            }

            return dictionary;
        });

        return (TEnum)enumValues[value];
    }
}
