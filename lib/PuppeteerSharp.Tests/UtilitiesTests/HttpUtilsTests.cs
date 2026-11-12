using System.Collections.Generic;
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

            Assert.That(HttpUtils.NormalizeHeaderValue("content-type", header), Is.EqualTo(header));
        }

        [Test]
        public void ShouldNormalizeMultilineHeaderWithNewlines()
        {
            var header = "text/html;\n charset=utf-8;\n boundary=something";

            Assert.That(
                HttpUtils.NormalizeHeaderValue("content-type", header),
                Is.EqualTo("text/html;, charset=utf-8;, boundary=something"));
        }

        [Test]
        public void ShouldTrimWhitespaceFromEachLine()
        {
            var header = "text/html; \n  charset=utf-8  \n   boundary=something   ";

            Assert.That(
                HttpUtils.NormalizeHeaderValue("content-type", header),
                Is.EqualTo("text/html;, charset=utf-8, boundary=something"));
        }

        [Test]
        public void ShouldFilterOutEmptyLines()
        {
            var header = "text/html;\n\n charset=utf-8;\n\n\n boundary=something";

            Assert.That(
                HttpUtils.NormalizeHeaderValue("content-type", header),
                Is.EqualTo("text/html;, charset=utf-8;, boundary=something"));
        }

        [Test]
        public void ShouldNormalizeSetCookieWithNewlines()
        {
            var header = "a=b\n c=d";

            Assert.That(
                HttpUtils.NormalizeHeaderValue("set-cookie", header),
                Is.EqualTo("a=b\n c=d"));
        }

        [Test]
        public void ShouldThrowWhenHeaderNameContainsCrlf()
        {
            var headers = new Dictionary<string, string> { ["x-foo\r\nx-injected"] = "bar" };

            Assert.That(() => HttpUtils.ValidateHeaders(headers), Throws.TypeOf<PuppeteerException>().With.Message.Contains("CRLF"));
        }

        [Test]
        public void ShouldThrowWhenHeaderValueContainsCrlf()
        {
            var headers = new Dictionary<string, string> { ["x-foo"] = "bar\r\nx-injected: evil" };

            Assert.That(() => HttpUtils.ValidateHeaders(headers), Throws.TypeOf<PuppeteerException>().With.Message.Contains("CRLF"));
        }

        [Test]
        public void ShouldAcceptValidHeaders()
        {
            Assert.That(() => HttpUtils.ValidateHeaders(new Dictionary<string, string> { ["x-foo"] = "bar" }), Throws.Nothing);
            Assert.That(() => HttpUtils.ValidateHeaders(null), Throws.Nothing);
        }
    }
}
