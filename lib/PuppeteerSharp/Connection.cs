using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    public class Connection : IDisposable
    {
        public Connection(string url, int delay, ClientWebSocket ws)
        {
            Url = url;
            Delay = delay;
            WebSocket = ws;

            _responses = new Dictionary<int, TaskCompletionSource<dynamic>>();
            _sessions = new Dictionary<string, Session>();
            _connectionCloseTask = new TaskCompletionSource<bool>();

            Task task = Task.Factory.StartNew(async () =>
            {
                await GetResponseAsync();
            });
        }

        #region Private Members
        private int _lastId;
        private Dictionary<int, TaskCompletionSource<dynamic>> _responses;
        private Dictionary<string, Session> _sessions;
        private TaskCompletionSource<bool> _connectionCloseTask;

        private bool _closeMessageSent;
        private const string CloseMessage = "Browser.close";
        #endregion

        #region Properties
        public string Url { get; set; }
        public int Delay { get; set; }
        public WebSocket WebSocket { get; set; }
        public event EventHandler Closed;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public bool IsClosed { get; internal set; }

        #endregion

        #region Public Methods

        public async Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            var id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>(){
                {"id", id},
                {"method", method},
                {"params", args}
            });

            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            QueueId(id);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default(CancellationToken));

            //I don't know if this will be the final solution.
            //For now this will prevent the WebSocket from failing after the process is killed by the close method.
            if (method == CloseMessage)
            {
                _closeMessageSent = true;
            }

            return await _responses[id].Task;
        }

        private void QueueId(int id)
        {
            _responses[id] = new TaskCompletionSource<dynamic>();
        }

        public async Task<Session> CreateSession(string targetId)
        {
            string sessionId = (await SendAsync("Target.attachToTarget", new { targetId })).sessionId;
            var session = new Session(this, targetId, sessionId);
            _sessions.Add(sessionId, session);
            return session;
        }
        #endregion

        #region Private Methods

        private void OnClose()
        {
            if (IsClosed)
            {
                return;
            }

            IsClosed = true;
            _connectionCloseTask.SetResult(true);

            Closed?.Invoke(this, new EventArgs());

            _responses.Clear();
            _sessions.Clear();
        }

        /// <summary>
        /// Starts listening the socket
        /// </summary>
        /// <returns>The start.</returns>
        private async Task<object> GetResponseAsync()
        {
            var buffer = new byte[2048];

            //If it's not in the list we wait for it
            while (true)
            {
                if (IsClosed)
                {
                    OnClose();
                    return null;
                }

                var endOfMessage = false;
                string response = string.Empty;

                while (!endOfMessage)
                {
                    var socketTask = WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    await Task.WhenAny(
                        _connectionCloseTask.Task,
                        socketTask
                    );

                    if (IsClosed)
                    {
                        OnClose();
                        return null;
                    }

                    WebSocketReceiveResult result = null;
                    try
                    {
                        result = socketTask.Result;
                    }
                    catch (AggregateException) when (_closeMessageSent)
                    {
                        if (!IsClosed)
                        {
                            OnClose();
                            return null;
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
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(response))
                {
                    dynamic obj = JsonConvert.DeserializeObject(response);
                    var objAsJObject = obj as JObject;

                    if (objAsJObject["id"] != null)
                    {
                        int id = (int)objAsJObject["id"];

                        //If we get the object we are waiting for we return if
                        //if not we add this to the list, sooner or later some one will come for it 
                        if (!_responses.ContainsKey(id))
                        {
                            QueueId(id);
                        }

                        _responses[id].SetResult(obj.result);
                    }
                    else
                    {
                        if (obj.method == "Target.receivedMessageFromTarget")
                        {
                            var session = _sessions.GetValueOrDefault(objAsJObject["params"]["sessionId"].ToString());
                            if (session != null)
                            {
                                session.OnMessage(objAsJObject["params"]["message"].ToString());
                            }
                        }
                        else if (obj.method == "Target.detachedFromTarget")
                        {
                            var session = _sessions.GetValueOrDefault(objAsJObject["params"]["sessionId"].ToString());
                            if (session != null)
                            {
                                session.Close();
                            }

                            _sessions.Remove(objAsJObject["params"]["sessionId"].ToString());
                        }
                        else
                        {
                            MessageReceived?.Invoke(this, new MessageEventArgs
                            {
                                MessageID = obj.method,
                                MessageData = objAsJObject["params"] as dynamic
                            });
                        }
                    }
                }
            }
        }
        #endregion
        #region Static Methods

        public static async Task<Connection> Create(string url, int delay = 0, int keepAliveInterval = 60)
        {
            var ws = new ClientWebSocket();
            ws.Options.KeepAliveInterval = new TimeSpan(0, 0, keepAliveInterval);
            await ws.ConnectAsync(new Uri(url), default(CancellationToken)).ConfigureAwait(false);
            return new Connection(url, delay, ws);
        }

        public void Dispose()
        {
            OnClose();
            WebSocket.Dispose();
        }

        #endregion
    }
}
