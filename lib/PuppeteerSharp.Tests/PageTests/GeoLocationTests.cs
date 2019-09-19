using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class GeoLocationTests : PuppeteerPageBaseTest
    {
        public GeoLocationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public async Task ShouldWorkWithDecimalValues()
        {
            await Context.OverridePermissionsAsync(TestConstants.ServerUrl, new[] { OverridePermission.Geolocation });
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetGeolocationAsync(new GeolocationOption
            {
                Longitude = 10.25m,
                Latitude = 10.54m
            });
            var geolocation = await Page.EvaluateFunctionAsync<GeolocationOption>(
                @"() => new Promise(resolve => navigator.geolocation.getCurrentPosition(position => {
                    resolve({latitude: position.coords.latitude, longitude: position.coords.longitude});
                }))");
            Assert.Equal(new GeolocationOption
            {
                Longitude = 10.25m,
                Latitude = 10.54m
            }, geolocation);
        }
    }
}
