using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Transport
{
    internal class WebSocketTransport : AbstractTransport
    {
        #region Fields
        private int _keepAliveInterval;
        private string _url;
        private ClientWebSocket _ws;
        private CancellationTokenSource _websocketReaderCancellationSource;
        #endregion

        #region Public Members
        public override void Dispose()
        {
            _ws.Dispose();
        }
        #endregion
        #region Internal Members
        internal WebSocketTransport(string url, int keepAliveInterval)
        {
            _url = url;
            _keepAliveInterval = keepAliveInterval;
            _websocketReaderCancellationSource = new CancellationTokenSource();
        }

        internal async Task ConnectAsync()
        {
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = new TimeSpan(0, 0, _keepAliveInterval);
            await _ws.ConnectAsync(new Uri(_url), default(CancellationToken)).ConfigureAwait(false);
        }

        internal override async Task SendAsync(string message)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, default(CancellationToken));
        }
        #endregion
        #region Private Members
        private async Task GetResponseAsync()
        {
            var buffer = new byte[2048];

            //If it's not in the list we wait for it
            while (true)
            {
                if (IsClosed)
                {
                    OnClose();
                    return;
                }

                var endOfMessage = false;
                string response = string.Empty;

                while (!endOfMessage)
                {
                    WebSocketReceiveResult result = null;
                    try
                    {
                        result = await _ws.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            _websocketReaderCancellationSource.Token);
                    }
                    catch (Exception) when (_stopListening)
                    {
                        return;
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        if (!IsClosed)
                        {
                            OnClose();
                            return;
                        }
                    }

                    endOfMessage = result.EndOfMessage;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        response += Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose();
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(response))
                {
                    MessageReceived(response);
                }
            }
        }

        internal override void Close()
        {
            if (IsClosed)
            {
                return;
            }
            _websocketReaderCancellationSource.Cancel();
        }

        internal override void StartListening()
        {
            Task task = Task.Factory.StartNew(async () =>
            {
                await GetResponseAsync();
            });
        }
        #endregion
    }
}