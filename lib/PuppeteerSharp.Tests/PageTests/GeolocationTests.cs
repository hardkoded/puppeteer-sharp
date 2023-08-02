using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class GeolocationTests : PuppeteerPageBaseTest
    {
        public GeolocationTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setGeolocation", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Context.OverridePermissionsAsync(TestConstants.ServerUrl, new[] { OverridePermission.Geolocation });
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetGeolocationAsync(new GeolocationOption
            {
                Longitude = 10,
                Latitude = 10
            });
            var geolocation = await Page.EvaluateFunctionAsync<GeolocationOption>(
                @"() => new Promise(resolve => navigator.geolocation.getCurrentPosition(position => {
                    resolve({latitude: position.coords.latitude, longitude: position.coords.longitude});
                }))");
            Assert.Equal(new GeolocationOption
            {
                Latitude = 10,
                Longitude = 10
            }, geolocation);
        }

        [PuppeteerTest("page.spec.ts", "Page.setGeolocation", "should throw when invalid longitude")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenInvalidLongitude()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                Page.SetGeolocationAsync(new GeolocationOption
                {
                    Longitude = 200,
                    Latitude = 100
                }));
            Assert.Contains("Invalid longitude '200'", exception.Message);
        }
    }
}
