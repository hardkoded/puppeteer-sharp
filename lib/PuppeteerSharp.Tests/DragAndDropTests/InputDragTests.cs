using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DragAndDropTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InputDragTests : PuppeteerPageBaseTest
    {
        public InputDragTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should throw an exception if not enabled before usage")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowAnExceptionIfNotEnabledBeforeUsage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            Assert.Contains("Drag Interception is not enabled!", exception.Message);
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragIntercepted event when dragged")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldEmitADragInterceptedEventWhenDragged()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            var data = await draggable.DragAsync(1, 1);

            Assert.Single(data.Items);
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.didDragStart"));
        }

        [PuppeteerTest("drag-and-drop.spec.ts", "Input.drag", "should emit a dragEnter")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
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
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task CanBeDisabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            Assert.False(Page.IsDragInterceptionEnabled);
            await Page.SetDragInterceptionAsync(true);
            Assert.True(Page.IsDragInterceptionEnabled);
            var draggable = await Page.QuerySelectorAsync("#drag");
            await draggable.DragAsync(1, 1);
            await Page.SetDragInterceptionAsync(false);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(() => draggable.DragAsync(1, 1));
            Assert.Contains("Drag Interception is not enabled!", exception.Message);
        }
    }
}
