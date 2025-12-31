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

        [Test, PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drop")]
        public async Task ShouldDrop()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.That(draggable, Is.Not.Null);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.That(dropzone, Is.Not.Null);
            await dropzone.DropAsync(draggable);
            Assert.That(await GetDragStateAsync(), Is.EqualTo(1234));
        }

        [Test, PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drop using mouse")]
        public async Task ShouldDropUsingMouse()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.That(draggable, Is.Not.Null);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.That(dropzone, Is.Not.Null);

            await draggable.HoverAsync();
            await Page.Mouse.DownAsync();
            await dropzone.HoverAsync();

            Assert.That(await GetDragStateAsync(), Is.EqualTo(123));

            await Page.Mouse.UpAsync();
            Assert.That(await GetDragStateAsync(), Is.EqualTo(1234));
        }

        [Test, PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should drag and drop")]
        public async Task ShouldDragAndDrop()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/drag-and-drop.html");
            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.That(draggable, Is.Not.Null);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.That(dropzone, Is.Not.Null);

#pragma warning disable CS0618 // Type or member is obsolete
            await draggable.DragAsync(dropzone);
#pragma warning restore CS0618 // Type or member is obsolete
            await dropzone.DropAsync(draggable);

            Assert.That(await GetDragStateAsync(), Is.EqualTo(1234));
        }

        private Task<int> GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync<int>("element => parseInt(element.innerHTML, 10)");
    }
}
