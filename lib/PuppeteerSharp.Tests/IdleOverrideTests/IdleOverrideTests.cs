using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.IdleOverrideTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class IdleOverrideTests : PuppeteerPageBaseTest
    {
        public IdleOverrideTests(ITestOutputHelper output) : base(output)
        {
        }

        private async Task<string> GetIdleStateAsync()
        {
            var stateElement = await Page.QuerySelectorAsync("#state");
            return await Page.EvaluateFunctionAsync<string>(
                @"(element) => {
                    return element.innerText;
                }",
                stateElement);
        }

        private async Task VerifyStateAsync(string expectedState)
        {
            var actualState = await GetIdleStateAsync();
            Assert.Equal(expectedState, actualState);
        }

        [PuppeteerTest("idle_override.spec.ts", "Emulate idle state", "changing idle state emulation causes change of the IdleDetector state")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ChangingIdleStateEmulationCausesChangeOfTheIdleDetectorState()
        {
            await Context.OverridePermissionsAsync(
                TestConstants.ServerUrl + "/idle-detector.html",
                new[]
                {
                    OverridePermission.IdleDetection,
                });

            await Page.GoToAsync(TestConstants.ServerUrl+ "/idle-detector.html");

            // Store initial state, as soon as it is not guaranteed to be `active, unlocked`.
            var initialState = await GetIdleStateAsync();

            // Emulate Idle states and verify IdleDetector updates state accordingly.
            await Page.EmulateIdleStateAsync(new EmulateIdleOverrides {
                IsUserActive = false,
                IsScreenUnlocked =  false,
            });

            await VerifyStateAsync("Idle state: idle, locked.");

            await Page.EmulateIdleStateAsync(new EmulateIdleOverrides {
                IsUserActive = true,
                IsScreenUnlocked = false,
            });
            await VerifyStateAsync("Idle state: active, locked.");

            await Page.EmulateIdleStateAsync(new EmulateIdleOverrides {
                IsUserActive = true,
                IsScreenUnlocked = true,
            });
            await VerifyStateAsync("Idle state: active, unlocked.");

            await Page.EmulateIdleStateAsync(new EmulateIdleOverrides {
                IsUserActive = false,
                IsScreenUnlocked = true,
            });
            await VerifyStateAsync("Idle state: idle, unlocked.");

            // Remove Idle emulation and verify IdleDetector is in initial state.
            await Page.EmulateIdleStateAsync();
            await VerifyStateAsync(initialState);

            // Emulate idle state again after removing emulation.
            await Page.EmulateIdleStateAsync(new EmulateIdleOverrides {
                IsUserActive = false,
                IsScreenUnlocked = false,
            });
            await VerifyStateAsync("Idle state: idle, locked.");

            // Remove emulation second time.
            await Page.EmulateIdleStateAsync();
            await VerifyStateAsync(initialState);
        }
    }
}
