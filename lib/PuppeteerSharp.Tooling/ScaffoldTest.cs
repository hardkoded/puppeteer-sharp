/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tooling
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "CodeDom is complicated.")]
    internal static partial class ScaffoldTest
    {
        private static readonly TextInfo _textInfo = CultureInfo.InvariantCulture.TextInfo;

        public static void FindTestsInFile(string path, Action<string, string> callback)
        {
            var keywords1 = new string[] { "it", "itChromeOnly", "itFailsFirefox", "xit" };
            var keywords2 = new string[] { "describe", "describeChromeOnly", "describeFailsFirefox", "describeWithDebugLogs" };
            var keywords = keywords1.Concat(keywords2);
            var rx = new Regex(@"^( *).*\b(" + string.Join("|", keywords) + @")\b.*[(](?:[']([^']+)[']|[""]([^""]+)[""])", RegexOptions.Multiline);

            var stack = new Stack<(int Indent, string Func, string Name)>();
            stack.Push((-1, null, null));

            foreach (var line in File.ReadAllLines(path))
            {
                var m = rx.Match(line);
                if (m?.Success == false)
                {
                    continue;
                }

                // keep in mind, group 0 is the entire match
                var indent = m.Groups[1].Value.Length;
                var func = m.Groups[2].Value;
                var name = string.IsNullOrEmpty(m.Groups[3].Value) ? m.Groups[4].Value : m.Groups[3].Value;

                while (indent <= stack.Peek().Indent)
                {
                    stack.Pop();
                }

                stack.Push((indent, func, name));
                var branch = stack.ToArray();

                if (keywords1.Contains(branch.First().Func))
                {
                    var testName = branch.First().Name;
                    string describe = null;

                    //if (branch.Length >= 2 && keywords2.Contains(branch[1].Func))
                    if (branch.Any(b => keywords2.Contains(b.Func)))
                    {
                        describe = string.Join(
                            " ",
                            branch
                                .Reverse()
                                .Where(b => keywords2.Contains(b.Func))
                                .Select(item => item.Name)
                                .ToList());
                    }

                    callback(describe, testName);
                }
            }
        }

        /// <summary>
        /// Generates a clean name from the test name.
        /// </summary>
        /// <param name="testDescribe">The original test name.</param>
        /// <returns>Returns a "clean" string, suitable for C# method names.</returns>
        public static string CleanName(string testDescribe)
            => new(Array.FindAll(_textInfo.ToTitleCase(testDescribe).ToCharArray(), c => char.IsLetterOrDigit(c)));

        public static void Run(ScaffoldTestOptions options)
        {
            if (!File.Exists(options.SpecFile))
            {
                throw new FileNotFoundException();
            }

            var fileInfo = new FileInfo(options.SpecFile);

            var dotSeparator = fileInfo.Name.IndexOf('.');
            var name = _textInfo.ToTitleCase(fileInfo.Name.Substring(0, dotSeparator)) + "Tests";
            var targetClass = GenerateClass(options.Namespace, name, fileInfo.Name);

            FindTestsInFile(options.SpecFile, (describe, testName) => AddTest(targetClass, new PuppeteerTestAttribute(fileInfo.Name, describe, testName)));

            using var provider = CodeDomProvider.CreateProvider("CSharp");
            var codegenOptions = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
            };

            using var sourceWriter = new StreamWriter(options.OutputFile);
            provider.GenerateCodeFromCompileUnit(
                targetClass, sourceWriter, codegenOptions);
        }

        private static CodeCompileUnit GenerateClass(string @namespace, string @class, string fileOrigin)
        {
            var targetUnit = new CodeCompileUnit();
            var globalNamespace = new CodeNamespace();

            // add imports
            globalNamespace.Imports.Add(new CodeNamespaceImport("System.Threading.Tasks"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("PuppeteerSharp.Tests.BaseTests"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("Xunit"));
            globalNamespace.Imports.Add(new CodeNamespaceImport("Xunit.Abstractions"));

            targetUnit.Namespaces.Add(globalNamespace);

            var codeNamespace = new CodeNamespace(@namespace);
            var targetClass = new CodeTypeDeclaration(@class)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed,
            };

            targetClass.BaseTypes.Add(new CodeTypeReference("PuppeteerSharpPageBaseTest"));

            _ = targetClass.CustomAttributes.Add(new CodeAttributeDeclaration(
                "Collection",
                new CodeAttributeArgument[]
                {
                    new CodeAttributeArgument(
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression("TestConstants"),
                            "TestFixtureBrowserCollectionName")),
                }));

            targetClass.Comments.Add(new CodeCommentStatement($"<puppeteer-file>{fileOrigin}</puppeteer-file>", true));
            codeNamespace.Types.Add(targetClass);

            targetUnit.Namespaces.Add(codeNamespace);

            // add constructor
            var constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public,
            };

            constructor.Parameters.Add(new CodeParameterDeclarationExpression("ITestOutputHelper", "output"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("output"));
            constructor.Comments.Add(new CodeCommentStatement("<inheritdoc/>", true));
            targetClass.Members.Add(constructor);

            return targetUnit;
        }

        private static void AddTest(CodeCompileUnit @class, PuppeteerTestAttribute test)
        {
            // make name out of the describe, and we should ignore any whitespaces, hyphens, etc.
            var name = CleanName(test.TestName);

            Console.WriteLine($"Adding {name}");

            var method = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("async Task"),
                Name = name,
            };

            @class.Namespaces[1].Types[0].Members.Add(method);

            method.Comments.Add(new CodeCommentStatement($"<puppeteer-file>{test.FileName}</puppeteer-file>", true));

            if (test.Describe != null)
            {
                method.Comments.Add(new CodeCommentStatement($"<puppeteer-describe>{test.Describe}</puppeteer-describe>", true));
            }

            method.Comments.Add(new CodeCommentStatement($"<puppeteer-it>{test.TestName}</puppeteer-it>", true));

            method.CustomAttributes.Add(new CodeAttributeDeclaration(
                "Fact",
                new CodeAttributeArgument[]
                {
                    new CodeAttributeArgument(
                        "Timeout",
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression("PuppeteerSharp.Puppeteer"),
                            "DefaultTimeout")),
                    new CodeAttributeArgument(
                        "Skip",
                        new CodePrimitiveExpression("This test is not yet implemented.")),
                }));
        }
    }
}
