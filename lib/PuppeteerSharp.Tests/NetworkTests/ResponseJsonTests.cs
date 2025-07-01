using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseJsonTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Response.json", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");

            Assert.That((await response.JsonAsync<JsonElement>()).GetProperty("foo").GetString(), Is.EqualTo("bar"));
        }
    }
}
