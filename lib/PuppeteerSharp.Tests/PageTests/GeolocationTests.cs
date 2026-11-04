using System;
using System.Text.Json;
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

        [Test, PuppeteerTest("page.spec", "Page Page.setGeolocation", "should work")]
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
            Assert.That(geolocation, Is.EqualTo(new GeolocationOption
            {
                Latitude = 10,
                Longitude = 10
            }));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setGeolocation", "should remove the override when called with null")]
        public async Task ShouldRemoveTheOverrideWhenCalledWithNull()
        {
            await Context.OverridePermissionsAsync(TestConstants.ServerUrl, new[] { OverridePermission.Geolocation });
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetGeolocationAsync(new GeolocationOption
            {
                Longitude = 10,
                Latitude = 10
            });
            await Page.SetGeolocationAsync(null);
            var geolocation = await Page.EvaluateFunctionAsync<JsonElement>(
                @"() => new Promise(resolve => navigator.geolocation.getCurrentPosition(
                    position => resolve({latitude: position.coords.latitude, longitude: position.coords.longitude}),
                    () => resolve('no position'),
                    {timeout: 1000}))");
            var overridden = geolocation.ValueKind == JsonValueKind.Object
                && geolocation.GetProperty("latitude").GetDouble() == 10
                && geolocation.GetProperty("longitude").GetDouble() == 10;
            Assert.That(overridden, Is.False);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setGeolocation", "should throw when invalid longitude")]
        public void ShouldThrowWhenInvalidLongitude()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                Page.SetGeolocationAsync(new GeolocationOption
                {
                    Longitude = 200,
                    Latitude = 100
                }));
            Assert.That(exception.Message, Does.Contain("Invalid longitude '200'"));
        }
    }
}
