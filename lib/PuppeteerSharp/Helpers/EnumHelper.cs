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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Helpers;

internal static class EnumHelper
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, Enum>> _stringToEnumCache = new();

    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Enum, string>> _enumToStringCache = new();

    public static TEnum ToEnum<TEnum>(this string value)
        where TEnum : Enum
    {
        var enumValues = _stringToEnumCache.GetOrAdd(typeof(TEnum), type =>
        {
            var names = Enum.GetNames(type);
            var values = (TEnum[])Enum.GetValues(type);
            var dictionary = new Dictionary<string, Enum>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < names.Length; i++)
            {
                dictionary.Add(names[i], values[i]);
                var memberValue = type.GetField(names[i]).GetCustomAttribute<EnumMemberAttribute>()?.Value;
                if (memberValue != null)
                {
                    dictionary[memberValue] = values[i];
                }
            }

            return dictionary;
        });
        return (TEnum)enumValues[value];
    }

    public static string ToValueString<TEnum>(this TEnum value)
        where TEnum : Enum
    {
        var enumValues = _enumToStringCache.GetOrAdd(typeof(TEnum), type =>
        {
            var names = Enum.GetNames(type);
            var dictionary = new Dictionary<Enum, string>();
            foreach (var t in names)
            {
                var field = type.GetField(t);
                var valueName = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? t;
                var fieldValue = (TEnum)field.GetValue(null);
                dictionary[fieldValue] = valueName;
            }

            return dictionary;
        });

        return enumValues[value];
    }

    public static TEnum FromValueString<TEnum>(string value)
        where TEnum : struct, Enum
    {
        var enumValues = _stringToEnumCache.GetOrAdd(typeof(TEnum), type =>
        {
            var names = Enum.GetNames(type);
            var dictionary = new Dictionary<string, Enum>(StringComparer.OrdinalIgnoreCase);
            foreach (var valueName in names)
            {
                var field = type.GetField(valueName);
                var fieldValue = (TEnum)field.GetValue(null);
                dictionary[valueName] = fieldValue;
                if (field.GetCustomAttribute<EnumMemberAttribute>()?.Value is { } enumMember)
                {
                    dictionary[enumMember] = fieldValue;
                }
            }

            return dictionary;
        });

        if (enumValues.TryGetValue(value, out var enumValue))
        {
            return (TEnum)enumValue;
        }

        var defaultEnumAttribute = typeof(TEnum).GetCustomAttribute<DefaultEnumValueAttribute>();
        if (defaultEnumAttribute != null)
        {
            return (TEnum)(object)defaultEnumAttribute.Value;
        }

        throw new ArgumentException($"Unknown value '{value}' for enum {typeof(TEnum).Name}");
    }
}
