using System;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.Transport
{
    internal class PipeTransport : AbstractTransport
    {
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        internal PipeTransport(StreamReader streamReader, StreamWriter streamWriter)
        {
            _streamReader = streamReader;
            _streamWriter = streamWriter;
        }

        public override void Dispose()
        {
            _streamWriter = null;
            _streamReader = null;
        }

        internal override Task SendAsync(string message)
        {
            return _streamWriter.WriteAsync(message);
        }

        internal override void StartListening()
        {
            Task task = Task.Factory.StartNew(async () =>
            {
                await GetResponseAsync();
            });
        }

        private async Task GetResponseAsync()
        {
            var buffer = new byte[2048];

            while (true)
            {
                if (IsClosed)
                {
                    OnClose();
                    return;
                }

                var response = await _streamReader.ReadToEndAsync();

                if (!string.IsNullOrEmpty(response))
                {
                    MessageReceived(response);
                }
            }
        }
    }
}