using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
  public class Connection
  {
    public Connection(string url, int delay, ClientWebSocket ws)
    {
      Url = url;
      Delay = delay;
      WebSocket = ws;
      _responses = new Dictionary<int, object>();
      _sessions = new Dictionary<int, Session>();
    }

    #region Private Members
    private int _lastId;
    private Dictionary<int, object> _responses;
    private Dictionary<int, Session> _sessions;
    private bool _closed = false;
    #endregion

    #region Properties
    public string Url { get; set; }
    public int Delay { get; set; }
    public WebSocket WebSocket { get; set; }

    #endregion
    #region Public Methods

    public async Task<object> SendAsync(string method, params object[] args)
    {
      var id = ++_lastId;
      var message = JsonConvert.SerializeObject(new Dictionary<string, object>(){
        {"id", id},
        {"method", method},
        {"params", args}
      });

      var encoded = Encoding.UTF8.GetBytes(message);
      var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
      await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default(CancellationToken));
      return await GetResponseAsync(id);
    }

    public async Task<Session> CreateSession(int targetId)
    {
      int sessionId = (int)await SendAsync("Target.attachToTarget", new {targetId});
      var session = new Session(this, targetId, sessionId);
      _sessions.Add(sessionId, session);
      return session;
    }
     #endregion

    #region Private Methods

    private void OnClose()
    {
      _closed = true;
      _responses.Clear();
      _sessions.Clear();
    }

		/// <summary>
		/// Starts listening the socket
		/// </summary>
		/// <returns>The start.</returns>
		private async Task<object> GetResponseAsync(int id)
		{
			var buffer = new byte[2048];

			//If the element is already in our list we just return it
			if (_responses.ContainsKey(id))
			{
				return _responses[id];
			}

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

					//If we get the object we are waiting for we return if
					//if not we add this to the list, sooner or later some one will come for it 
					if (obj.id == id)
					{
						return obj;
					}
					else
					{
						_responses.Add((int)obj.id, id);
					}

				}
				else if (result.MessageType == WebSocketMessageType.Binary)
				{
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

    public async Task<Connection> Create(string url, int delay = 0)
    {
      var ws = new ClientWebSocket();
      await ws.ConnectAsync(new Uri(url), default(CancellationToken)).ConfigureAwait(false);
      return new Connection(url, delay, ws);
    }

    #endregion
  }
}
