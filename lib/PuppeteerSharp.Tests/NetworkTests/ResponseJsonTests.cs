using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ResponseJsonTests : PuppeteerPageBaseTest
    {
        public ResponseJsonTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.Equal(JObject.Parse("{foo: 'bar'}"), await response.JsonAsync());
        }
    }
}
