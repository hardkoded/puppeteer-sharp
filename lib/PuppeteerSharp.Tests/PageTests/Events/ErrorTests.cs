using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ErrorTests : PuppeteerPageBaseTest
    {
        public ErrorTests(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            Page.Error += (_, e) => error = e.Error;
            var gotoTask = Page.GoToAsync("chrome://crash");

            await WaitForError();
            Assert.Equal("Page crashed!", error);
        }
    }
}
