using System.Linq;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InputDragTests : DevToolsContextBaseTest
    {
        public InputDragTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should throw an exception if not enabled before usage")]
        [PuppeteerFact]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            Assert.Contains("Drag Interception is not enabled!", exception.Message);
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragIntercepted event when dragged")]
        [PuppeteerFact]
        public async Task ShouldEmitADragInterceptedEventWhenDragged()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);

            Assert.Single(data.Items);
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragEnter")]
        [PuppeteerFact]
        public async Task ShouldEmitADragEnter()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await DevToolsContext.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);

            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragOver event")]
        [PuppeteerFact]
        public async Task ShouldEmitADragOver()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await DevToolsContext.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);
            await dropzone.DragOverAsync(data);

            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be dropped")]
        [PuppeteerFact]
        public async Task CanBeDropped()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);
            var dropzone = await DevToolsContext.QuerySelectorAsync("#drop");
            await dropzone.DragEnterAsync(data);
            await dropzone.DragOverAsync(data);
            await dropzone.DropAsync(data);

            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDrop"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be dragged and dropped with a single function")]
        [PuppeteerFact]
        public async Task CanBeDraggedAndDroppedWithASingleFunction()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            var dropzone = await DevToolsContext.QuerySelectorAsync("#drop");
            await draggable.DragAndDropAsync(dropzone);

            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragEnter"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDragOver"));
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => globalThis.didDrop"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "can be disabled")]
        [PuppeteerFact]
        public async Task CanBeDisabled()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(DevToolsContext.IsDragInterceptionEnabled);
            await DevToolsContext.SetDragInterceptionAsync(true);
            Assert.True(DevToolsContext.IsDragInterceptionEnabled);
            var draggable = await DevToolsContext.QuerySelectorAsync("#drag");
            await draggable.DragAsync(1, 1);
            await DevToolsContext.SetDragInterceptionAsync(false);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            Assert.Contains("Drag Interception is not enabled!", exception.Message);
        }
    }
}
