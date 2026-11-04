using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class SystemTextJsonSerializationContextTests
    {
        // Generic method type parameters and BCL/System.Text.Json types that JsonSerializerContext
        // always knows how to resolve on its own (they don't need a [JsonSerializable] entry).
        private static readonly HashSet<string> IgnoredTypeNames = new(StringComparer.Ordinal)
        {
            "T",
            "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong",
            "float", "double", "decimal", "char", "string", "object",
            "DateTime", "DateTimeOffset", "TimeSpan", "DateOnly", "TimeOnly", "Half", "Int128", "UInt128",
            "JsonElement", "JsonDocument", "JsonNode", "JsonArray", "JsonObject",
        };

        private static Type FindTypeByName(Assembly assembly, string typeName, ICollection<string> problems)
        {
            var candidates = assembly.GetTypes().Where(candidate => candidate.Name == typeName).ToList();

            if (candidates.Count == 0)
            {
                problems.Add($"{typeName}: could not locate a matching type via reflection (update this test if the type was renamed).");
                return null;
            }

            if (candidates.Count > 1)
            {
                problems.Add($"{typeName}: matches {candidates.Count} types ({string.Join(", ", candidates.Select(c => c.FullName))}) - " +
                    "make this test's type resolution namespace-aware so it checks the right one.");
                return null;
            }

            return candidates[0];
        }

        private static string GetPuppeteerSharpSourceDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null && !Directory.Exists(Path.Combine(directory.FullName, "PuppeteerSharp")))
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new InvalidOperationException("Could not locate the PuppeteerSharp source directory from the test output directory.");
            }

            return Path.Combine(directory.FullName, "PuppeteerSharp");
        }

        [Test]
        public void AllTypesDeserializedWithToObjectShouldBeRegisteredInTheSourceGenerationContext()
        {
            var libraryDirectory = GetPuppeteerSharpSourceDirectory();
            var assembly = typeof(JsonHelper).Assembly;

            // JsonHelper.cs is where ToObject<T> itself is *declared* (its own generic type
            // parameter T would otherwise be picked up as a false positive).
            var typeNames = Directory
                .EnumerateFiles(libraryDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith("JsonHelper.cs", StringComparison.Ordinal) &&
                               !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") &&
                               !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
                .SelectMany(path => Regex.Matches(File.ReadAllText(path), @"\.ToObject<\s*([A-Za-z_][A-Za-z0-9_]*)\s*(\[\])?\s*>")
                    .Select(match => match.Groups[1].Value))
                .Where(name => !IgnoredTypeNames.Contains(name))
                .Distinct()
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            // Sanity check: if this ever returns empty, the regex above stopped matching the
            // calling convention used in the codebase and the test would pass for the wrong reason.
            Assert.That(typeNames, Is.Not.Empty, "Expected to find at least one .ToObject<T>() call site in the PuppeteerSharp source tree.");

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = SystemTextJsonSerializationContext.Default,
            };

            var problems = new List<string>();

            foreach (var typeName in typeNames)
            {
                var type = FindTypeByName(assembly, typeName, problems);

                if (type == null)
                {
                    continue;
                }

                JsonTypeInfo typeInfo;

                try
                {
                    typeInfo = ((IJsonTypeInfoResolver)SystemTextJsonSerializationContext.Default).GetTypeInfo(type, options);
                }
                catch (Exception ex)
                {
                    problems.Add($"{type.FullName}: {ex.GetType().Name} - {ex.Message}");
                    continue;
                }

                if (typeInfo == null)
                {
                    problems.Add(type.FullName);
                }
            }

            Assert.That(
                problems,
                Is.Empty,
                "The following types are deserialized via ToObject<T>() but are not resolvable through " +
                "SystemTextJsonSerializationContext. Without a [JsonSerializable] entry (or being reachable " +
                "as a property from an already-registered type), apps that disable reflection-based " +
                "serialization (e.g. Native AOT) will throw a NotSupportedException at runtime. " +
                "See https://github.com/hardkoded/puppeteer-sharp/issues/3515. Problems found: " +
                Environment.NewLine + string.Join(Environment.NewLine, problems));
        }
    }
}
