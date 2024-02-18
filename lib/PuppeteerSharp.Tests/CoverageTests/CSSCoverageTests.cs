using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageCoverage;

namespace PuppeteerSharp.Tests.CoverageTests
{
    public class CSSCoverageTests : PuppeteerPageBaseTest
    {
        public CSSCoverageTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work")]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/simple.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            StringAssert.Contains("/csscoverage/simple.html", coverage[0].Url);
            Assert.AreEqual(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 1,
                    End = 22
                }
            }, coverage[0].Ranges);
            var range = coverage[0].Ranges[0];
            Assert.AreEqual("div { color: green; }", coverage[0].Text.Substring(range.Start, range.End - range.Start));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report sourceURLs")]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/sourceurl.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.AreEqual("nicename.css", coverage[0].Url);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report multiple stylesheets")]
        public async Task ShouldReportMultipleStylesheets()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.AreEqual(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url);
            StringAssert.Contains("/csscoverage/stylesheet1.css", orderedList.ElementAt(0).Url);
            StringAssert.Contains("/csscoverage/stylesheet2.css", orderedList.ElementAt(1).Url);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report stylesheets that have no coverage")]
        public async Task ShouldReportStylesheetsThatHaveNoCoverage()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/unused.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            StringAssert.Contains("unused.css", entry.Url);
            Assert.IsEmpty(entry.Ranges);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with media queries")]
        public async Task ShouldWorkWithMediaQueries()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/media.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            StringAssert.Contains("/csscoverage/media.html", entry.Url);
            Assert.AreEqual(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 8,
                    End = 15
                },
                new CoverageEntryRange
                {
                    Start = 17,
                    End = 38
                }
            }, coverage[0].Ranges);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with complicated usecases")]
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
                    ""Start"": 306,
                    ""End"": 323
                  },
                  {
                    ""Start"": 327,
                    ""End"": 433
                  }
                ],
                ""Text"": ""\n @charset \""utf - 8\"";\n@namespace svg url(http://www.w3.org/2000/svg);\n@font-face {\n  font-family: \""Example Font\"";\n src: url(\""./Dosis-Regular.ttf\"");\n}\n\n#fluffy {\n  border: 1px solid black;\n  z-index: 1;\n  /* -webkit-disabled-property: rgb(1, 2, 3) */\n  -lol-cats: \""dogs\"" /* non-existing property */\n}\n\n@media (min-width: 1px) {\n  span {\n    -webkit-border-radius: 10px;\n    font-family: \""Example Font\"";\n    animation: 1s identifier;\n  }\n}\n""
              }
            ]";
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/involved.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.AreEqual(
                TestUtils.CompressText(involved),
                Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @":\d{4}\/", ":<PORT>/"));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should ignore injected stylesheets")]
        public async Task ShouldIgnoreInjectedStylesheets()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.AddStyleTagAsync(new AddTagOptions
            {
                Content = "body { margin: 10px;}"
            });
            // trigger style recalc
            var margin = await Page.EvaluateExpressionAsync<string>("window.getComputedStyle(document.body).margin");
            Assert.AreEqual("10px", margin);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.IsEmpty(coverage);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with a recently loaded stylesheet")]
        public async Task ShouldWorkWithArRecentlyLoadedStylesheet()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.EvaluateFunctionAsync(@"async url => {
                document.body.textContent = 'hello, world';

                const link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = url;
                document.head.appendChild(link);
                await new Promise(x => link.onload = x);
            }", TestConstants.ServerUrl + "/csscoverage/stylesheet1.css");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
        }
    }
}
