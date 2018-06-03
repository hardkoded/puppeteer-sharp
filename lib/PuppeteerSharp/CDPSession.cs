using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp
{
    /// <summary>
    /// The CDPSession instances are used to talk raw Chrome Devtools Protocol:
    ///  * Protocol methods can be called with <see cref="CDPSession.SendAsync(string, bool, dynamic)"/> method.
    ///  * Protocol events, using the <see cref="CDPSession.MessageReceived"/> event.
    /// 
    /// Documentation on DevTools Protocol can be found here: <see href="https://chromedevtools.github.io/devtools-protocol/"/>.
    /// 
    /// <code>
    /// <![CDATA[
    /// var client = await Page.Target.CreateCDPSessionAsync();
    /// await client.SendAsync("Animation.enable");
    /// client.MessageReceived += (sender, e) =>
    /// {
    ///      if (e.MessageID == "Animation.animationCreated")
    ///      {
    ///          Console.WriteLine("Animation created!");
    ///      }
    /// };
    /// dynamic response = await client.SendAsync("Animation.getPlaybackRate");
    /// Console.WriteLine("playback rate is " + response.playbackRate);
    /// await client.SendAsync("Animation.setPlaybackRate", new
    /// {
    ///     playbackRate = Convert.ToInt32(response.playbackRate / 2)
    /// });
    /// ]]></code>
    /// </summary>
    public class CDPSession : IDisposable
    {
        internal CDPSession(Connection connection, string targetId, string sessionId)
        {
            Connection = connection;
            TargetId = targetId;
            SessionId = sessionId;

            _callbacks = new Dictionary<int, MessageTask>();
            _logger = Connection.LoggerFactory.CreateLogger<CDPSession>();
        }

        #region Private Members
        private int _lastId;
        private readonly Dictionary<int, MessageTask> _callbacks;
        private readonly ILogger _logger;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId { get; }
        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public string SessionId { get; }
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public Connection Connection { get; private set; }
        /// <summary>
        /// Occurs when message received from Chromium.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;
        /// <summary>
        /// Occurs when tracing is completed.
        /// </summary>
        public event EventHandler<TracingCompleteEventArgs> TracingComplete;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CDPSession"/> is closed.
        /// </summary>
        /// <value><c>true</c> if is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; internal set; }
        #endregion

        #region Public Methods

        internal async Task<T> SendAsync<T>(string method, dynamic args = null)
        {
            var content = await SendAsync(method, true, args);
            return JsonConvert.DeserializeObject<T>(content);
        }

        internal Task<dynamic> SendAsync(string method, dynamic args = null)
        {
            return SendAsync(method, false, args ?? new { });
        }

        internal async Task<dynamic> SendAsync(string method, bool rawContent, dynamic args = null)
        {
            if (Connection == null)
            {
                throw new Exception($"Protocol error ({method}): Session closed. Most likely the page has been closed.");
            }
            int id = ++_lastId;
            var message = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"id", id},
                {"method", method},
                {"params", args}
            });
            _logger.LogTrace("Send ► {Id} Method {Method} Params {@Params}", id, method, (object)args);

            var callback = new MessageTask
            {
                TaskWrapper = new TaskCompletionSource<dynamic>(),
                Method = method,
                RawContent = rawContent
            };

            _callbacks[id] = callback;

            try
            {
                await Connection.SendAsync("Target.sendMessageToTarget", new Dictionary<string, object>
                {
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

        /// <summary>
        /// Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.
        /// </summary>
        /// <returns></returns>
        public Task DetachAsync()
            => Connection.SendAsync("Target.detachFromTarget", new { sessionId = SessionId });

        #endregion

        #region Private Methods

        /// <summary>
        /// Releases all resource used by the <see cref="CDPSession"/> object by sending a ""Target.closeTarget"
        /// using the <see cref="Connection.SendAsync(string, dynamic)"/> method.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="CDPSession"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="CDPSession"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="CDPSession"/> so the garbage collector can reclaim the memory that the
        /// <see cref="CDPSession"/> was occupying.</remarks>
        public void Dispose()
        {
            Connection.SendAsync("Target.closeTarget", new Dictionary<string, object>
            {
                ["targetId"] = TargetId
            }).GetAwaiter().GetResult();
        }

        internal void OnMessage(string message)
        {
            dynamic obj = JsonConvert.DeserializeObject(message);
            var objAsJObject = obj as JObject;

            _logger.LogTrace("◀ Receive {Message}", message);

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
            else if (obj.method == "Tracing.tracingComplete")
            {
                TracingComplete?.Invoke(this, new TracingCompleteEventArgs
                {
                    Stream = objAsJObject["params"].Value<string>("stream")
                });
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

        internal void OnClosed()
        {
            IsClosed = true;
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
