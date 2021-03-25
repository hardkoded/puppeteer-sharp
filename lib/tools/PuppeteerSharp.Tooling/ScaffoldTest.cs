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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;

namespace PuppeteerSharp.Tooling
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "CodeDom is complicated.")]
    internal static partial class ScaffoldTest
    {
        private static readonly TextInfo _textInfo = CultureInfo.InvariantCulture.TextInfo;

        public static void FindTestsInFile(string path, Action<string> callback)
        {
            var rx = new Regex(@"it\(\'(.*)\',");
            foreach (string line in File.ReadAllLines(path))
            {
                var m = rx.Match(line);
                if (m?.Success == false)
                {
                    continue;
                }

                // keep in mind, group 0 is the entire match, but
                // first (and only group), should give us the describe value
                callback(m.Groups[1].Value);
            }
        }

        /// <summary>
        /// Generates a clean name from the test name.
        /// </summary>
        /// <param name="testDescribe">The original test name.</param>
        /// <returns>Returns a "clean" string, suitable for C# method names.</returns>
        public static string CleanName(string testDescribe)
            => new string(Array.FindAll(_textInfo.ToTitleCase(testDescribe).ToCharArray(), c => char.IsLetterOrDigit(c)));

        public static void Run(ScaffoldTestOptions options)
        {
            if (!File.Exists(options.SpecFile))
            {
                throw new FileNotFoundException();
            }

            var fileInfo = new FileInfo(options.SpecFile);

            int dotSeparator = fileInfo.Name.IndexOf('.');
            string name = _textInfo.ToTitleCase(fileInfo.Name.Substring(0, dotSeparator)) + "Tests";
            var targetClass = GenerateClass(options.Namespace, name, fileInfo.Name);

            FindTestsInFile(options.SpecFile, (name) => AddTest(targetClass, name, fileInfo.Name));

            using CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions codegenOptions = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
            };

            using StreamWriter sourceWriter = new StreamWriter(options.OutputFile);
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

        private static void AddTest(CodeCompileUnit @class, string testDescribe, string testOrigin)
        {
            // make name out of the describe, and we should ignore any whitespaces, hyphens, etc.
            string name = CleanName(testDescribe);

            Console.WriteLine($"Adding {name}");

            CodeMemberMethod method = new CodeMemberMethod()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("async Task"),
                Name = name,
            };

            @class.Namespaces[1].Types[0].Members.Add(method);

            method.Comments.Add(new CodeCommentStatement($"<puppeteer-file>{testOrigin}</puppeteer-file>", true));
            method.Comments.Add(new CodeCommentStatement($"<puppeteer-it>{testDescribe}</puppeteer-it>", true));
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
