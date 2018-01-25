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
    public class Connection
    {
        public Connection(string url, int delay, ClientWebSocket ws)
        {
            Url = url;
            Delay = delay;
            WebSocket = ws;

            _responses = new Dictionary<int, TaskCompletionSource<dynamic>>();
            _sessions = new Dictionary<string, Session>();

            Task task = Task.Factory.StartNew(async () =>
            {
                await GetResponseAsync();
            });
        }

        #region Private Members
        private int _lastId;
        private Dictionary<int, TaskCompletionSource<dynamic>> _responses;
        private Dictionary<string, Session> _sessions;
        private bool _closed = false;

        #endregion

        #region Properties
        public string Url { get; set; }
        public int Delay { get; set; }
        public WebSocket WebSocket { get; set; }
        public event EventHandler Closed;
        public event EventHandler<MessageEventArgs> MessageReceived;
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
            _closed = true;
            if (Closed != null)
            {
                Closed.Invoke(this, new EventArgs());
            }
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
                if (_closed)
                {
                    return null;
                }

                var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
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
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    OnClose();
                    return null;
                }

            }
        }
        #endregion
        #region Static Methods

        public static async Task<Connection> Create(string url, int delay = 0)
        {
            var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(url), default(CancellationToken)).ConfigureAwait(false);
            return new Connection(url, delay, ws);
        }

        #endregion
    }
}
