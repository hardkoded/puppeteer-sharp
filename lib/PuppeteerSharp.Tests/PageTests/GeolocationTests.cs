using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class GeolocationTests : PuppeteerPageBaseTest
    {
        public GeolocationTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setGeolocation", "should work")]
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
            Assert.AreEqual(new GeolocationOption
            {
                Latitude = 10,
                Longitude = 10
            }, geolocation);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setGeolocation", "should throw when invalid longitude")]
        public void ShouldThrowWhenInvalidLongitude()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                Page.SetGeolocationAsync(new GeolocationOption
                {
                    Longitude = 200,
                    Latitude = 100
                }));
            StringAssert.Contains("Invalid longitude '200'", exception.Message);
        }
    }
}
