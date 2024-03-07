using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    public class DragAndDropTests : PuppeteerPageBaseTest
    {
        public DragAndDropTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drop")]
        public async Task ShouldDrop()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.NotNull(draggable);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.NotNull(dropzone);
            await dropzone.DropAsync(draggable);
            Assert.AreEqual(1234, await GetDragStateAsync());
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drop using mouse")]
        public async Task ShouldDropUsingMouse()
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

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drag and drop")]
        public async Task ShouldDragAndDrop()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.NotNull(draggable);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.NotNull(dropzone);

#pragma warning disable CS0618 // Type or member is obsolete
            await draggable.DragAsync(dropzone);
#pragma warning restore CS0618 // Type or member is obsolete
            await dropzone.DropAsync(draggable);

            Assert.AreEqual(1234, await GetDragStateAsync());
        }

        private Task<int> GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync<int>("element => parseInt(element.innerHTML, 10)");
    }
}
