using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CefSharp.Dom;
using CefSharp.Dom.PageCoverage;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JSCoverageTests : DevToolsContextBaseTest
    {
        public JSCoverageTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", WaitUntilNavigation.Networkidle0);
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            Assert.Contains("/jscoverage/simple.html", coverage[0].Url);
            Assert.Equal(new CoverageEntryRange[]
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

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report sourceURLs")]
        [PuppeteerFact]
        public async Task ShouldReportSourceUrls()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/sourceurl.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            Assert.Equal("nicename.js", coverage[0].Url);
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should ignore eval() scripts by default")]
        [PuppeteerFact]
        public async Task ShouldIgnoreEvalScriptsByDefault()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "shouldn't ignore eval() scripts if reportAnonymousScripts is true")]
        [PuppeteerFact]
        public async Task ShouldntIgnoreEvalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ReportAnonymousScripts = true
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.NotNull(coverage.FirstOrDefault(entry => entry.Url.StartsWith("debugger://", StringComparison.Ordinal)));
            Assert.Equal(2, coverage.Count());
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should ignore pptr internal scripts if reportAnonymousScripts is true")]
        [PuppeteerFact]
        public async Task ShouldIgnorePptrInternalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ReportAnonymousScripts = true
            });
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateExpressionAsync("console.log('foo')");
            await DevToolsContext.EvaluateFunctionAsync("() => console.log('bar')");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Empty(coverage);
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report multiple scripts")]
        [PuppeteerFact]
        public async Task ShouldReportMultipleScripts()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Equal(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url);
            Assert.Contains("/jscoverage/script1.js", orderedList.ElementAt(0).Url);
            Assert.Contains("/jscoverage/script2.js", orderedList.ElementAt(1).Url);
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report right ranges")]
        [PuppeteerFact(Skip ="Investigate")]
        public async Task ShouldReportRightRanges()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/ranges.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Single(entry.Ranges);
            var range = entry.Ranges[0];
            Assert.Equal("console.log('used!');", entry.Text.Substring(range.Start, range.End - range.Start));
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should report scripts that have no coverage")]
        [PuppeteerFact]
        public async Task ShouldReportScriptsThatHaveNoCoverage()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/unused.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Contains("unused.html", entry.Url);
            Assert.Empty(entry.Ranges);
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should work with conditionals")]
        [PuppeteerFact(Skip = "Investigate")]
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
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/jscoverage/involved.html");
            var coverage = await DevToolsContext.Coverage.StopJSCoverageAsync();
            Assert.Equal(
                TestUtils.CompressText(involved),
                Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @"\d{4}\/", "<PORT>/"));
        }

        [PuppeteerTest("coverage.spec.ts", "JSCoverage", "should not hang when there is a debugger statement")]
        [PuppeteerFact(Skip = "Skipped in puppeteer")]
        public async Task ShouldNotHangWhenThereIsADebuggerStatement()
        {
            await DevToolsContext.Coverage.StartJSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync(@"() => {
                debugger;
            }");
            await DevToolsContext.Coverage.StopJSCoverageAsync();
        }
    }
}
