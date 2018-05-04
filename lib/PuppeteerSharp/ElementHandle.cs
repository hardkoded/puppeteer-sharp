using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class ElementHandle : JSHandle
    {
        internal Page Page { get; }

        public ElementHandle(ExecutionContext context, Session client, object remoteObject, Page page) :
            base(context, client, remoteObject)
        {
            Page = page;
        }

        public override ElementHandle AsElement() => this;

        public async Task ClickAsync(Dictionary<string, object> options = null)
        {
            var (x, y) = await VisibleCenterAsync();
            await Page.Mouse.Click(x, y, options ?? new Dictionary<string, object>());
        }

        internal Task TapAsync()
        {
            throw new NotImplementedException();
        }

        internal async Task<ElementHandle> GetElementAsync(string selector)
        {
            var handle = await ExecutionContext.EvaluateFunctionHandleAsync(
                "(element, selector) => element.querySelector(selector)",
                this, selector);

            var element = handle.AsElement();
            if (element != null)
            {
                return element;
            }

            await handle.Dispose();
            return null;
        }

        internal Task DisposeAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<(decimal x, decimal y)> VisibleCenterAsync()
        {
            await ScrollIntoViewIfNeededAsync();
            var box = await BoundingBoxAsync();
            if (box == null)
            {
                throw new PuppeteerException("Node is not visible");
            }

            return (
                x: box.X + (box.Width / 2),
                y: box.Y + (box.Height / 2)
            );
        }

        private async Task ScrollIntoViewIfNeededAsync()
        {
            var errorMessage = await ExecutionContext.EvaluateFunctionAsync<string>(@"element => {
                if (!element.isConnected)
                    return 'Node is detached from document';
                if (element.nodeType !== Node.ELEMENT_NODE)
                    return 'Node is not of type HTMLElement';
                element.scrollIntoViewIfNeeded();
                return null;
            }", this);

            if (errorMessage != null)
                throw new PuppeteerException(errorMessage);
        }

        private async Task<BoundingBox> BoundingBoxAsync()
        {
            var result = await _client.SendAsync("DOM.getBoxModel", new { objectId = RemoteObject.objectId.ToString() });

            if (result == null)
                return null;

            var quad = result.model.border.ToObject<decimal[]>();

            var x = new[] { quad[0], quad[2], quad[4], quad[6] }.Min();
            var y = new[] { quad[1], quad[3], quad[5], quad[7] }.Min();
            var width = new[] { quad[0], quad[2], quad[4], quad[6] }.Max() - x;
            var height = new[] { quad[1], quad[3], quad[5], quad[7] }.Max() - y;

            return new BoundingBox(x, y, width, height);

        }

        private class BoundingBox
        {
            public decimal X { get; }
            public decimal Y { get; }
            public decimal Width { get; }
            public decimal Height { get; }

            public BoundingBox(decimal x, decimal y, decimal width, decimal height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        }
    }
}