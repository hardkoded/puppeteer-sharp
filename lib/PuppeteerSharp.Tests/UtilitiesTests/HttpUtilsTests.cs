using NUnit.Framework;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class HttpUtilsTests
    {
        [Test]
        public void ShouldGiveSingleLineHeaderValueUnchanged()
        {
            var header = "application/json; charset=utf-8";

            Assert.That(HttpUtils.NormalizeHeaderValue(header), Is.EqualTo(header));
        }

        [Test]
        public void ShouldNormalizeMultilineHeaderWithNewlines()
        {
            var header = "text/html;\n charset=utf-8;\n boundary=something";

            Assert.That(
                HttpUtils.NormalizeHeaderValue(header),
                Is.EqualTo("text/html;, charset=utf-8;, boundary=something"));
        }

        [Test]
        public void ShouldTrimWhitespaceFromEachLine()
        {
            var header = "text/html; \n  charset=utf-8  \n   boundary=something   ";

            Assert.That(
                HttpUtils.NormalizeHeaderValue(header),
                Is.EqualTo("text/html;, charset=utf-8, boundary=something"));
        }

        [Test]
        public void ShouldFilterOutEmptyLines()
        {
            var header = "text/html;\n\n charset=utf-8;\n\n\n boundary=something";

            Assert.That(
                HttpUtils.NormalizeHeaderValue(header),
                Is.EqualTo("text/html;, charset=utf-8;, boundary=something"));
        }
    }
}
