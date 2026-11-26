using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Media;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class CdpEmulationManagerMediaTests
    {
        private static readonly MediaFeatureValue _reducedMotion = new()
        {
            MediaFeature = MediaFeature.PrefersReducedMotion,
            Value = "reduce",
        };

        [Test]
        public async Task ShouldNotKeepAnUnsupportedMediaType()
        {
            var client = Substitute.For<ICDPSession>();
            var manager = new CdpEmulationManager(client);
            await manager.EmulateMediaTypeAsync(MediaType.Print);

            var exception = Assert.Throws<ArgumentException>(() =>
                manager.EmulateMediaTypeAsync((MediaType)(-1))
            );
            await manager.EmulateMediaFeaturesAsync([_reducedMotion]);

            Assert.That(exception.Message, Does.Contain("Unsupported media type: -1"));
            var requests = GetRequests(client);
            Assert.That(requests, Has.Length.EqualTo(2));
            Assert.That(requests[1].Media, Is.EqualTo(MediaType.Print));
        }

        [Test]
        public async Task ShouldNotKeepUnsupportedMediaFeatures()
        {
            var client = Substitute.For<ICDPSession>();
            var manager = new CdpEmulationManager(client);
            await manager.EmulateMediaFeaturesAsync([_reducedMotion]);

            var unsupported = Assert.Throws<ArgumentException>(() =>
                manager.EmulateMediaFeaturesAsync([
                    new MediaFeatureValue { MediaFeature = (MediaFeature)(-1), Value = "reduce" },
                ])
            );
            var missing = Assert.Throws<ArgumentException>(() =>
                manager.EmulateMediaFeaturesAsync([null])
            );
            var withoutValue = Assert.Throws<ArgumentException>(() =>
                manager.EmulateMediaFeaturesAsync([
                    new MediaFeatureValue { MediaFeature = MediaFeature.PrefersColorScheme },
                ])
            );
            await manager.EmulateMediaTypeAsync(MediaType.Screen);

            Assert.That(unsupported.Message, Does.Contain("Unsupported media feature: -1"));
            Assert.That(missing.Message, Does.Contain("Unsupported media feature: null"));
            Assert.That(
                withoutValue.Message,
                Does.Contain("Missing value for media feature: PrefersColorScheme")
            );
            var requests = GetRequests(client);
            Assert.That(requests, Has.Length.EqualTo(2));
            Assert.That(requests[1].Features, Is.EqualTo(new[] { _reducedMotion }));
        }

        [Test]
        public async Task ShouldNotSendMediaRequestsOutOfOrder()
        {
            var client = Substitute.For<ICDPSession>();
            using var firstSendStarted = new ManualResetEventSlim();
            using var secondSendStarted = new ManualResetEventSlim();
            using var releaseFirstSend = new ManualResetEventSlim();
            client
                .SendAsync(
                    Arg.Any<string>(),
                    Arg.Any<object>(),
                    Arg.Any<bool>(),
                    Arg.Any<CommandOptions>()
                )
                .Returns(callInfo =>
                {
                    var request = (EmulationSetEmulatedMediaRequest)callInfo[1];
                    if (request.Features == null)
                    {
                        firstSendStarted.Set();
                        releaseFirstSend.Wait(TimeSpan.FromSeconds(10));
                    }
                    else
                    {
                        secondSendStarted.Set();
                    }

                    return Task.FromResult<JsonElement?>(null);
                });
            var manager = new CdpEmulationManager(client);

            var first = Task.Run(() => manager.EmulateMediaTypeAsync(MediaType.Print));
            Assert.That(firstSendStarted.Wait(TimeSpan.FromSeconds(10)), Is.True);
            var second = Task.Run(() => manager.EmulateMediaFeaturesAsync([_reducedMotion]));
            var overtook = secondSendStarted.Wait(TimeSpan.FromSeconds(1));
            releaseFirstSend.Set();
            await Task.WhenAll(first, second);

            Assert.That(overtook, Is.False);
            Assert.That(secondSendStarted.IsSet, Is.True);
        }

        private static EmulationSetEmulatedMediaRequest[] GetRequests(ICDPSession client) =>
            client
                .ReceivedCalls()
                .Where(call => call.GetMethodInfo().Name == nameof(ICDPSession.SendAsync))
                .Select(call => (EmulationSetEmulatedMediaRequest)call.GetArguments()[1])
                .ToArray();
    }
}
