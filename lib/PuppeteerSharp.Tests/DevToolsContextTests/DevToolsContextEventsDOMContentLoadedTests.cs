using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEventsDOMContentLoadedTests : DevToolsContextBaseTest
    {
        public DevToolsContextEventsDOMContentLoadedTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.DOMContentLoaded", "should fire when expected")]
        [PuppeteerFact]
        public async Task ShouldFireWhenExpected()
        {
            var _ = DevToolsContext.GoToAsync(TestConstants.AboutBlank);
            var completion = new TaskCompletionSource<bool>();
            DevToolsContext.DOMContentLoaded += (_, _) => completion.SetResult(true);
            await completion.Task;
        }
    }
}
