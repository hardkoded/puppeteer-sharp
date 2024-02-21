using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
#pragma warning disable CS0618 // Type or member is obsolete
namespace PuppeteerSharp.Tests.DragAndDropTests
{
    public class LegacyDragAndDropTests : PuppeteerPageBaseTest
    {
        public LegacyDragAndDropTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Legacy Drag n' Drop", "should emit a dragIntercepted event when dragged")]
        public async Task ShouldEmitADragInterceptedEventWhenDragged()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);

            Assert.That(data.Items, Has.Exactly(1).Items);
            Assert.AreEqual(1, await GetDragStateAsync());
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Legacy Drag n' Drop", "should emit a dragEnter")]
        public async Task ShouldEmitADragEnter()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);

            Assert.AreEqual(12, await GetDragStateAsync());
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Legacy Drag n' Drop", "should emit a dragOver event")]
        public async Task ShouldEmitADragOver()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);
            await dropzone.DragOverAsync(data);

            Assert.AreEqual(123, await GetDragStateAsync());
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Legacy Drag n' Drop", "can be dropped")]
        public async Task CanBeDropped()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);
            await dropzone.DragOverAsync(data);
            await dropzone.DropAsync(data);

            Assert.AreEqual(12334, await GetDragStateAsync());
        }

        [Test, Retry(2), PuppeteerTest("drag-and-drop.spec", "Legacy Drag n' Drop", "can be dragged and dropped with a single function")]
        public async Task CanBeDraggedAndDroppedWithASingleFunction()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var dropzone = await Page.QuerySelectorAsync("#drop");
            await draggable.DragAndDropAsync(dropzone);

            Assert.AreEqual(12334, await GetDragStateAsync());
        }

        private Task<int> GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync<int>("element => parseInt(element.innerHTML, 10)");
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
