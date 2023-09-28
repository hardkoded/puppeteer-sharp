using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    public class LegacyDragAndDropTests : PuppeteerPageBaseTest
    {
        public LegacyDragAndDropTests() : base()
        {
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "should throw an exception if not enabled before usage")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");

            try
            {
                await draggable.DragAsync(1, 1);
            }
            catch (PuppeteerException exception)
            {
                Assert.Contains("Drag Interception is not enabled!", exception.Message);
            }
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "should emit a dragIntercepted event when dragged")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "should emit a dragEnter")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "should emit a dragOver event")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "can be dropped")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("drag-and-drop.spec.ts", "Legacy Drag n' Drop", "can be dragged and dropped with a single function")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        private Task GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync("element => parseInt(element.innerHTML, 10)");
    }
}
}
