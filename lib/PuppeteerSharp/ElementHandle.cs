using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class ElementHandle : JSHandle
    {
        private Page _page;

        public ElementHandle(ExecutionContext context, Session client, object args, Page page) :
            base(context, client, args)
        {
            _page = page;
        }

        public Task ClickAsync()
        {
            throw new NotImplementedException();
        }

        internal Task TapAsync()
        {
            throw new NotImplementedException();
        }

        internal Task DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}