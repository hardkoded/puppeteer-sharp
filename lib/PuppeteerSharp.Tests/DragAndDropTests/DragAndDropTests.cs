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

        [Test, PuppeteerTest("drag-and-drop.spec", "Drag n' Drop", "should release the mouse button when the drop fails")]
        public async Task ShouldReleaseTheMouseButtonWhenTheDropFails()
        {
            // The page re-renders while the drag is in flight, which detaches the
            // dragged node, as a reactive list would. The drop then fails.
            await Page.SetContentAsync(@"
              <div id=""drag"">drag me</div>
              <div id=""drop"">drop here</div>
              <script>
                let rerendered = false;
                document.addEventListener('mousemove', () => {
                  if (rerendered) {
                    return;
                  }
                  rerendered = true;
                  const drag = document.getElementById('drag');
                  drag.replaceWith(drag.cloneNode(true));
                });
              </script>
            ");

            var draggable = await Page.QuerySelectorAsync("#drag");
            Assert.That(draggable, Is.Not.Null);
            var dropzone = await Page.QuerySelectorAsync("#drop");
            Assert.That(dropzone, Is.Not.Null);

#pragma warning disable CS0618 // Type or member is obsolete
            await draggable.DragAsync(dropzone);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.ThrowsAsync<PuppeteerException>(() => dropzone.DropAsync(draggable));

            // The drag pressed the mouse button down. If the failed drop leaves it
            // pressed, every later mouse event still carries it, and the next click
            // fires twice.
            await Page.EvaluateFunctionAsync(@"() => {
              globalThis.buttons = undefined;
              document.addEventListener(
                'mousemove',
                event => {
                  globalThis.buttons = event.buttons;
                },
                { once: true },
              );
            }");
            await Page.Mouse.MoveAsync(20, 20);

            Assert.That(
                await Page.EvaluateFunctionAsync<int?>("() => globalThis.buttons"),
                Is.EqualTo(0));
        }

        private Task<int> GetDragStateAsync()
            => Page.QuerySelectorAsync("#drag-state").EvaluateFunctionAsync<int>("element => parseInt(element.innerHTML, 10)");
    }
}

