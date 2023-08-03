using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    public class InputDragTests : PuppeteerPageBaseTest
    {
        public InputDragTests(): base()
        {
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should throw an exception if not enabled before usage")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            var exception = Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            StringAssert.Contains("Drag Interception is not enabled!", exception.Message);
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragIntercepted event when dragged")]
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
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragEnter")]
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

            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragOver event")]
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

            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be dropped")]
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

            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDrop"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be dragged and dropped with a single function")]
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

            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDrop"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be disabled")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task CanBeDisabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            await draggable.DragAsync(1, 1);
            await Page.SetDragInterceptionAsync(false);
            var exception = Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            StringAssert.Contains("Drag Interception is not enabled!", exception.Message);
        }
    }
}
