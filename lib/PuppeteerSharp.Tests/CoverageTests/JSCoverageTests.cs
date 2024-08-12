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
    public class JSCoverageTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should work")]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", WaitUntilNavigation.Networkidle0);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.That(coverage[0].Url, Does.Contain("/jscoverage/simple.html"));
            Assert.That(coverage[0].Ranges, Is.EqualTo(new CoverageEntryRange[]
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
            }));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should report sourceURLs")]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/sourceurl.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            Assert.That(coverage[0].Url, Is.EqualTo("nicename.js"));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should ignore eval() scripts by default")]
        public async Task ShouldIgnoreEvalScriptsByDefault()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should not ignore eval() scripts if reportAnonymousScripts is true")]
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
            var filtered = coverage.Where(entry => !entry.Url.StartsWith("debugger://"));
            Assert.That(filtered.Count(), Is.EqualTo(1));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should ignore pptr internal scripts if reportAnonymousScripts is true")]
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
            Assert.That(coverage, Is.Empty);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should report multiple scripts")]
        public async Task ShouldReportMultipleScripts()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Length.EqualTo(2));
            var orderedList = coverage.OrderBy(c => c.Url);
            Assert.That(orderedList.ElementAt(0).Url, Does.Contain("/jscoverage/script1.js"));
            Assert.That(orderedList.ElementAt(1).Url, Does.Contain("/jscoverage/script2.js"));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should report right ranges")]
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
            Assert.That(entry.Text.Substring(range.Start, range.End - range.Start), Is.EqualTo("console.log('used!');"));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should report scripts that have no coverage")]
        public async Task ShouldReportScriptsThatHaveNoCoverage()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/unused.html");
            // Prevent flaky tests.
            await Task.Delay(1000);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.That(coverage, Has.Exactly(1).Items);
            var entry = coverage[0];
            Assert.That(entry.Url, Does.Contain("unused.html"));
            Assert.That(entry.Ranges, Is.Empty);
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should work with conditionals")]
        public async Task ShouldWorkWithConditionals()
        {
            const string involved = @"[
              {
                ""RawScriptCoverage"": null,
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
            Assert.That(Regex.Replace(TestUtils.CompressText(JsonConvert.SerializeObject(coverage)), @"\d{4}\/", "<PORT>/"),
                Is.EqualTo(TestUtils.CompressText(involved)));
        }

        [Test, Retry(2), PuppeteerTest("coverage.spec", "Coverage specs JSCoverage", "should not hang when there is a debugger statement")]
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
