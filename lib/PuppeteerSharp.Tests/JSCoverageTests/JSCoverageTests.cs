using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace PuppeteerSharp.Tests.JSCoverageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class JSCoverageTests : PuppeteerPageBaseTest
    {
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
                    Start = 0,
                    End = 17
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
        public async Task ShouldIgnoreAnonymousScripts()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("console.log(1);");
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
            Assert.Equal("console.log('used!');", entry.Text.Substring(range.Start, range.End));
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
                ""url"": ""http://localhost:<PORT>/jscoverage/involved.html"",
                ""ranges"": [
                  {
                    ""start"": 0,
                    ""end"": 35
                  },
                  {
                    ""start"": 50,
                    ""end"": 100
                  },
                  {
                    ""start"": 107,
                    ""end"": 141
                  },
                  {
                    ""start"": 148,
                    ""end"": 160
                  },
                  {
                    ""start"": 168,
                    ""end"": 207
                  }
                ],
                ""text"": ""\\nfunction foo() {\\n  if (1 > 2)\\n    console.log(1);\\n  if (1 < 2)\\n    console.log(2);\\n  let x = 1 > 2 ? 'foo' : 'bar';\\n  let y = 1 < 2 ? 'foo' : 'bar';\\n  let z = () => {};\\n  let q = () => {};\\n  q();\\n}\\n\\nfoo();\\n""
              }
            ]";
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/involved.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal(
                TestUtils.CompressText(involved),
                TestUtils.CompressText(JsonConvert.SerializeObject(coverage)));
        }
    }
}
