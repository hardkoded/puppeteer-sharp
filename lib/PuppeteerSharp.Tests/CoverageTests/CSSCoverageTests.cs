using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.PageCoverage;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CoverageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CSSCoverageTests : DevToolsContextBaseTest
    {
        public CSSCoverageTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/simple.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Single(coverage);
            Assert.Contains("/csscoverage/simple.html", coverage[0].Url);
            Assert.Equal(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 1,
                    End = 22
                }
            }, coverage[0].Ranges);
            var range = coverage[0].Ranges[0];
            Assert.Equal("div { color: green; }", coverage[0].Text.Substring(range.Start, range.End - range.Start));
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should report sourceURLs")]
        [PuppeteerFact]
        public async Task ShouldReportSourceUrls()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/sourceurl.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Single(coverage);
            Assert.Equal("nicename.css", coverage[0].Url);
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should report multiple stylesheets")]
        [PuppeteerFact]
        public async Task ShouldReportMultipleStylesheets()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Equal(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url);
            Assert.Contains("/csscoverage/stylesheet1.css", orderedList.ElementAt(0).Url);
            Assert.Contains("/csscoverage/stylesheet2.css", orderedList.ElementAt(1).Url);
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should report stylesheets that have no coverage")]
        [PuppeteerFact]
        public async Task ShouldReportStylesheetsThatHaveNoCoverage()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/unused.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Contains("unused.css", entry.Url);
            Assert.Empty(entry.Ranges);
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should work with media queries")]
        [PuppeteerFact]
        public async Task ShouldWorkWithMediaQueries()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/media.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Single(coverage);
            var entry = coverage[0];
            Assert.Contains("/csscoverage/media.html", entry.Url);
            Assert.Equal(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 17,
                    End = 38
                }
            }, coverage[0].Ranges);
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should work with complicated usecases")]
        [PuppeteerFact]
        public async Task ShouldWorkWithComplicatedUsecases()
        {
            const string involved = @"[
              {
                ""Url"": ""http://localhost:<PORT>/csscoverage/involved.html"",
                ""Ranges"": [
                  {
                    ""Start"": 149,
                    ""End"": 297
                  },
                  {
                    ""Start"": 327,
                    ""End"": 433
                  }
                ],
                ""Text"": ""\n @charset \""utf - 8\"";\n@namespace svg url(http://www.w3.org/2000/svg);\n@font-face {\n  font-family: \""Example Font\"";\n src: url(\""./Dosis-Regular.ttf\"");\n}\n\n#fluffy {\n  border: 1px solid black;\n  z-index: 1;\n  /* -webkit-disabled-property: rgb(1, 2, 3) */\n  -lol-cats: \""dogs\"" /* non-existing property */\n}\n\n@media (min-width: 1px) {\n  span {\n    -webkit-border-radius: 10px;\n    font-family: \""Example Font\"";\n    animation: 1s identifier;\n  }\n}\n""
              }
            ]";
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csscoverage/involved.html");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Equal(
                TestUtils.CompressText(involved),
                Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @":\d{4}\/", ":<PORT>/"));
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should ignore injected stylesheets")]
        [PuppeteerFact]
        public async Task ShouldIgnoreInjectedStylesheets()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.AddStyleTagAsync(new AddTagOptions
            {
                Content = "body { margin: 10px;}"
            });
            // trigger style recalc
            var margin = await DevToolsContext.EvaluateExpressionAsync<string>("window.getComputedStyle(document.body).margin");
            Assert.Equal("10px", margin);
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Empty(coverage);
        }

        [PuppeteerTest("coverage.spec.ts", "CSSCoverage", "should work with a recently loaded stylesheet")]
        [PuppeteerFact]
        public async Task ShouldWorkWithArRecentlyLoadedStylesheet()
        {
            await DevToolsContext.Coverage.StartCSSCoverageAsync();
            await DevToolsContext.EvaluateFunctionAsync(@"async url => {
                document.body.textContent = 'hello, world';

                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = url;
                document.head.appendChild(link);
                await new Promise(x => link.onload = x);
            }", TestConstants.ServerUrl + "/csscoverage/stylesheet1.css");
            var coverage = await DevToolsContext.Coverage.StopCSSCoverageAsync();
            Assert.Single(coverage);
        }
    }
}
