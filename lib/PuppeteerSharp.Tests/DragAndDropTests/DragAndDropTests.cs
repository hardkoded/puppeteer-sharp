using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    public class DragAndDropTests : PuppeteerPageBaseTest
    {
        public DragAndDropTests() : base()
        {
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Drag n' Drop", "should drop")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.NotNull(draggable);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.NotNull(dropzone);
            await dropzone.DropAsync(draggable);
            Assert.AreEqual(1234, await GetDragStateAsync());
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Drag n' Drop", "should drop using mouse")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.NotNull(draggable);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.NotNull(dropzone);

            await draggable.HoverAsync();
            await Page.Mouse.DownAsync();
            await dropzone.HoverAsync();

            Assert.AreEqual(123, await GetDragStateAsync());

            await Page.Mouse.UpAsync();
            Assert.AreEqual(1234, await GetDragStateAsync());
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Drag n' Drop", "should drag and drop")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.NotNull(draggable);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.NotNull(dropzone);

            await draggable.DragAsync(dropzone);
            await dropzone.DropAsync(draggable);

            Assert.AreEqual(1234, await GetDragStateAsync());
        }

        private Task GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync("element => parseInt(element.innerHTML, 10)");
    }
}
}
