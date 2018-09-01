using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.PageCoverage;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSCoverageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class JSCoverageTests : PuppeteerPageBaseTest
    {
        public JSCoverageTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
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

        [Fact]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/sourceurl.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            Assert.Equal("nicename.js", coverage[0].Url);
        }

        [Fact]
        public async Task ShouldIgnoreEvalScriptsByDefault()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
        }

        [Fact]
        public async Task ShouldntIgnoreEvalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await Page.Coverage.StartJSCoverageAsync(new CoverageStartOptions
            {
                ReportAnonymousScripts = true
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.NotNull(coverage.FirstOrDefault(entry => entry.Url.StartsWith("debugger://", StringComparison.Ordinal)));
            Assert.Equal(2, coverage.Count());
        }

        [Fact]
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
            Assert.Empty(coverage);
        }

        [Fact]
        public async Task ShouldReportMultipleScripts()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url);
            Assert.Contains("/jscoverage/script1.js", orderedList.ElementAt(0).Url);
            Assert.Contains("/jscoverage/script2.js", orderedList.ElementAt(1).Url);
        }

        [Fact]
        public async Task ShouldReportRightRanges()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/ranges.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Single(entry.Ranges);
            var range = entry.Ranges[0];
            Assert.Equal("console.log('used!');", entry.Text.Substring(range.Start, range.End - range.Start));
        }

        [Fact]
        public async Task ShouldReportScriptsThatHaveNoCoverage()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/unused.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Contains("unused.html", entry.Url);
            Assert.Empty(entry.Ranges);
        }

        [Fact]
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
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal(
                TestUtils.CompressText(involved),
                Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @"\d{4}\/", "<PORT>/"));
        }
    }
}