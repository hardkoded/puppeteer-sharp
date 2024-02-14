using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.CoverageTests
{
    public class JSCoverageTests : PuppeteerPageBaseTest
    {
        public JSCoverageTests() : base()
        {
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should work")]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", WaitUntilNavigation.Networkidle0);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            StringAssert.Contains("/jscoverage/simple.html", coverage[0].Url);
            Assert.AreEqual(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 0,
                    End = 17
                },
                new CoverageEntryRange
                {
                    Start = 35,
                    End = 61
                },
            }, coverage[0].Ranges);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report sourceURLs")]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/sourceurl.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.AreEqual("nicename.js", coverage[0].Url);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should ignore eval() scripts by default")]
        public async Task ShouldIgnoreEvalScriptsByDefault()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should not ignore eval() scripts if reportAnonymousScripts is true")]
        public async Task ShouldNotIgnoreEvalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ReportAnonymousScripts = true
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.NotNull(coverage.FirstOrDefault(entry => entry.Url.StartsWith("debugger://", StringComparison.Ordinal)));
            Assert.AreEqual(2, coverage.Length);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should ignore pptr internal scripts if reportAnonymousScripts is true")]
        public async Task ShouldIgnorePptrInternalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ReportAnonymousScripts = true
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("console.log('foo')");
            await Page.EvaluateFunctionAsync("() => console.log('bar')");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.IsEmpty(coverage);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report multiple scripts")]
        public async Task ShouldReportMultipleScripts()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.AreEqual(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url);
            StringAssert.Contains("/jscoverage/script1.js", orderedList.ElementAt(0).Url);
            StringAssert.Contains("/jscoverage/script2.js", orderedList.ElementAt(1).Url);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report right ranges")]
        public async Task ShouldReportRightRanges()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/ranges.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            Assert.That(entry.Ranges, Has.Exactly(1).Items);
            var range = entry.Ranges[0];
            Assert.AreEqual("console.log('used!');", entry.Text.Substring(range.Start, range.End - range.Start));
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report scripts that have no coverage")]
        public async Task ShouldReportScriptsThatHaveNoCoverage()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/unused.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            StringAssert.Contains("unused.html", entry.Url);
            Assert.IsEmpty(entry.Ranges);
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should work with conditionals")]
        public async Task ShouldWorkWithConditionals()
        {
            const string involved = @"[
              {
                ""Url"": ""http://localhost:<PORT>/jscoverage/involved.html"",
                ""Ranges"": [
                  {
                    ""Start"": 0,
                    ""End"": 35
                  },
                  {
                    ""Start"": 50,
                    ""End"": 100
                  },
                  {
                    ""Start"": 107,
                    ""End"": 141
                  },
                  {
                    ""Start"": 148,
                    ""End"": 160
                  },
                  {
                    ""Start"": 168,
                    ""End"": 207
                  }
                ],
                ""Text"": ""\nfunction foo() {\n  if (1 > 2)\n    console.log(1);\n  if (1 < 2)\n    console.log(2);\n  let x = 1 > 2 ? 'foo' : 'bar';\n  let y = 1 < 2 ? 'foo' : 'bar';\n  let z = () => {};\n  let q = () => {};\n  q();\n}\n\nfoo();\n""
              }
            ]";
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/involved.html");
            // Give the coverage some time.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.AreEqual(
                TestUtils.CompressText(involved),
                Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @"\d{4}\/", "<PORT>/"));
        }

        [Test, PuppeteerTest("coverage.spec.ts", "JSCoverage", "should not hang when there is a debugger statement")]
        [Ignore("Skipped in puppeteer")]
        public async Task ShouldNotHangWhenThereIsADebuggerStatement()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() => {
                debugger;
            }");
            await Page.Coverage.StopJSCoverageAsync();
        }
    }
}
