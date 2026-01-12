using System.Text.Json;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    /// <summary>
    /// Unit tests for CookiePartitionKeyConverter. These are standalone tests that don't require browsers.
    /// </summary>
    [TestFixture]
    public class CookiePartitionKeyConverterTests
    {
        [Test]
        public void ShouldSerializeAndDeserializeAsString()
        {
            // Arrange
            var cookie = new CookieParam
            {
                Name = "test",
                Value = "value",
                PartitionKey = "https://example.com"
            };

            // Act - Serialize to JSON
            var json = JsonSerializer.Serialize(cookie);

            // Assert - Verify it's serialized as a string
            Assert.That(json, Does.Contain("\"PartitionKey\":\"https://example.com\""));

            // Act - Deserialize back
            var deserialized = JsonSerializer.Deserialize<CookieParam>(json);

            // Assert - Verify round-trip works
            Assert.That(deserialized.PartitionKey, Is.EqualTo("https://example.com"));
        }

        [Test]
        public void ShouldDeserializeCdpObjectFormat()
        {
            // Arrange - CDP returns object format
            var cdpJson = @"{""Name"":""test"",""Value"":""value"",""PartitionKey"":{""topLevelSite"":""https://example.com"",""hasCrossSiteAncestor"":false}}";

            // Act
            var cookie = JsonSerializer.Deserialize<CookieParam>(cdpJson);

            // Assert - Should extract the topLevelSite value
            Assert.That(cookie.PartitionKey, Is.EqualTo("https://example.com"));
        }

        [Test]
        public void ShouldDeserializeStringFormat()
        {
            // Arrange - User-saved format (simple string)
            var stringJson = @"{""Name"":""test"",""Value"":""value"",""PartitionKey"":""https://example.com""}";

            // Act
            var cookie = JsonSerializer.Deserialize<CookieParam>(stringJson);

            // Assert
            Assert.That(cookie.PartitionKey, Is.EqualTo("https://example.com"));
        }

        [Test]
        public void ShouldHandleNullPartitionKey()
        {
            // Arrange
            var cookie = new CookieParam
            {
                Name = "test",
                Value = "value",
                PartitionKey = null
            };

            // Act
            var json = JsonSerializer.Serialize(cookie);
            var deserialized = JsonSerializer.Deserialize<CookieParam>(json);

            // Assert
            Assert.That(deserialized.PartitionKey, Is.Null);
        }

        [Test]
        public void ShouldSupportRoundTripSerializationToFile()
        {
            // Arrange - Simulate getting cookies from browser
            var originalCookies = new[]
            {
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "value1",
                    Domain = "example.com",
                    PartitionKey = "https://example.com"
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "value2",
                    Domain = "test.com",
                    PartitionKey = "https://test.com"
                }
            };

            // Act - Serialize to JSON (simulating saving to file)
            var json = JsonSerializer.Serialize(originalCookies);

            // Act - Deserialize from JSON (simulating loading from file)
            var loadedCookies = JsonSerializer.Deserialize<CookieParam[]>(json);

            // Assert - Verify all data is preserved
            Assert.That(loadedCookies, Has.Length.EqualTo(2));
            Assert.That(loadedCookies[0].Name, Is.EqualTo("cookie1"));
            Assert.That(loadedCookies[0].PartitionKey, Is.EqualTo("https://example.com"));
            Assert.That(loadedCookies[1].Name, Is.EqualTo("cookie2"));
            Assert.That(loadedCookies[1].PartitionKey, Is.EqualTo("https://test.com"));
        }
    }
}
