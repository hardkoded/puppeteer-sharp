using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Transport
{
    internal class PipeTransport : AbstractTransport
    {
        internal PipeTransport()
        {
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        internal override void Close()
        {
            throw new NotImplementedException();
        }

        internal override Task SendAsync(string message)
        {
            throw new NotImplementedException();
        }

        internal override void StartListening()
        {
            throw new NotImplementedException();
        }
    }
}