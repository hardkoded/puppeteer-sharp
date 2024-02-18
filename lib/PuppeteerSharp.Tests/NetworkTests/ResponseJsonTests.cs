using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseJsonTests : PuppeteerPageBaseTest
    {
        public ResponseJsonTests() : base()
        {
        }

        [Test,  Retry(2), PuppeteerTest("network.spec", "network Response.json", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.AreEqual(JObject.Parse("{foo: 'bar'}"), await response.JsonAsync());
        }
    }
}
