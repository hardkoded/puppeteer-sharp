using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work")]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/simple.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.That(coverage[0].Url, Does.Contain("/csscoverage/simple.html"));
            Assert.That(coverage[0].Ranges, Is.EqualTo(new CoverageEntryRange[]
            {
                new CoverageEntryRange
                {
                    Start = 1,
                    End = 22
                }
            }));
            var range = coverage[0].Ranges[0];
            Assert.That(coverage[0].Text.Substring(range.Start, range.End - range.Start), Is.EqualTo("div { color: green; }"));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report sourceURLs")]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/sourceurl.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.That(coverage[0].Url, Is.EqualTo("nicename.css"));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report multiple stylesheets")]
        public async Task ShouldReportMultipleStylesheets()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/multiple.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Length.EqualTo(2));
            var orderedList = coverage.OrderBy(c => c.Url);
            Assert.That(orderedList.ElementAt(0).Url, Does.Contain("/csscoverage/stylesheet1.css"));
            Assert.That(orderedList.ElementAt(1).Url, Does.Contain("/csscoverage/stylesheet2.css"));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should report stylesheets that have no coverage")]
        public async Task ShouldReportStylesheetsThatHaveNoCoverage()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/unused.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            Assert.That(entry.Url, Does.Contain("unused.css"));
            Assert.That(entry.Ranges, Is.Empty);
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with media queries")]
        public async Task ShouldWorkWithMediaQueries()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/csscoverage/media.html");
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            Assert.That(entry.Url, Does.Contain("/csscoverage/media.html"));
            Assert.That(coverage[0].Ranges, Is.EqualTo(new CoverageEntryRange[]
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
            }));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with complicated usecases")]
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
            var coverageAsJsonString = JsonSerializer.Serialize(
                coverage,
                new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                });
            Assert.That(
                Regex.Replace(TestUtils.CompressText(coverageAsJsonString), @":\d{4,5}\/", ":<PORT>/"),
                Is.EqualTo(TestUtils.CompressText(involved)));
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should ignore injected stylesheets")]
        public async Task ShouldIgnoreInjectedStylesheets()
        {
            await Page.Coverage.StartCSSCoverageAsync();
            await Page.AddStyleTagAsync(new AddTagOptions
            {
                Content = "body { margin: 10px;}"
            });
            // trigger style recalc
            var margin = await Page.EvaluateExpressionAsync<string>("window.getComputedStyle(document.body).margin");
            Assert.That(margin, Is.EqualTo("10px"));
            var coverage = await Page.Coverage.StopCSSCoverageAsync();
            Assert.That(coverage, Is.Empty);
        }

        [Test, PuppeteerTest("coverage.spec", "Coverage specs CSSCoverage", "should work with a recently loaded stylesheet")]
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
