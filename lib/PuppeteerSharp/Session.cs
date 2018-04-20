using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    public class Session : IDisposable
    {
        public Session(Connection connection, string targetId, string sessionId)
        {
            Connection = connection;
            TargetId = targetId;
            SessionId = sessionId;

            _callbacks = new Dictionary<int, MessageTask>();
        }

        #region Private Memebers
        private int _lastId = 0;
        private Dictionary<int, MessageTask> _callbacks;
        #endregion

        #region Properties
        public string TargetId { get; }
        public string SessionId { get; private set; }
        public Connection Connection { get; private set; }
        public event EventHandler<MessageEventArgs> MessageReceived;
        #endregion

        #region Public Methods

        public async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            var content = await SendAsync(method, true, args);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            return SendAsync(method, false, args);
        }

        public async Task<dynamic> SendAsync(string method, bool rawContent, dynamic args = null)
        {
            if (Connection == null)
            {
                throw new Exception($"Protocol error ({method}): Session closed. Most likely the page has been closed.");
            }
            int id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>(){
                {"id", id},
                {"method", method},
                {"params", args}
            });

            var callback = new MessageTask
            {
                TaskWrapper = new TaskCompletionSource<dynamic>(),
                Method = method,
                RawContent = rawContent
            };

            _callbacks[id] = callback;

            try
            {
                await Connection.SendAsync("Target.sendMessageToTarget", new Dictionary<string, object>() {
                    {"sessionId", SessionId},
                    {"message", message}
                });
            }
            catch (Exception ex)
            {
                if (_callbacks.ContainsKey(id))
                {
                    _callbacks.Remove(id);
                    callback.TaskWrapper.SetException(new MessageException(ex.Message, ex));
                }
            }

            return await callback.TaskWrapper.Task;
        }

        public Task DetachAsync()
            => Connection.SendAsync("Target.detachFromTarget", new { sessionId = SessionId });

        #endregion

        #region Private Mathods

        public void Dispose()
        {
            Connection.SendAsync("Target.closeTarget", new Dictionary<string, object>() {
                {"targetId", TargetId}
            }).GetAwaiter().GetResult();
        }

        internal void OnMessage(string message)
        {
            dynamic obj = JsonConvert.DeserializeObject(message);
            var objAsJObject = obj as JObject;

            if (objAsJObject["id"] != null && _callbacks.ContainsKey((int)obj.id))
            {

                var callback = _callbacks[(int)obj.id];
                _callbacks.Remove((int)obj.id);

                if (objAsJObject["error"] != null)
                {
                    callback.TaskWrapper.SetException(new MessageException(
                        $"Protocol error ({ callback.Method }): {obj.error.message} {obj.error.data}"
                    ));
                }
                else
                {
                    if (callback.RawContent)
                    {
                        callback.TaskWrapper.SetResult(JsonConvert.SerializeObject(obj.result));
                    }
                    else
                    {
                        callback.TaskWrapper.SetResult(obj.result);
                    }
                }
            }
            else
            {
                MessageReceived?.Invoke(this, new MessageEventArgs
                {
                    MessageID = obj.method,
                    MessageData = objAsJObject["params"]
                });
            }
        }

        internal void Close()
        {
            foreach (var callback in _callbacks.Values)
            {
                callback.TaskWrapper.SetException(new TargetClosedException(
                    $"Protocol error({callback.Method}): Target closed."
                ));
            }
            _callbacks.Clear();
            Connection = null;
        }

        #endregion
    }
}
