using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
  public class Browser
  {

    public Browser(Connection connection, Dictionary<string, object> options)
    {
      Connection = connection;
      _ignoreHTTPSErrors = options.ContainsKey("ignoreHTTPSErrors") && (bool)options["ignoreHTTPSErrors"];
      _appMode = options.ContainsKey("appMode") && (bool)options["appMode"];
    }

    #region Private Members
    private readonly bool _ignoreHTTPSErrors;
    private readonly bool _appMode;
    #endregion
    #region Properties
    public Connection Connection { get; set; }
    public event EventHandler Closed;

    public string WebSocketEndpoint
    {
      get
      {
        return Connection.Url;
      }
    }
    #endregion

    #region Public Methods

    public async Task<Page> NewPageAsync()
    {
      var targetId = (int)(await Connection.SendAsync("Target.createTarget", new Dictionary<string, object>(){
        {"url", "about:blank"}
      }));
      var client = await Connection.CreateSession(targetId);
      return new Page(client, _ignoreHTTPSErrors, _appMode);
    }

    public async Task<string> GetVersionAsync()
    {
      dynamic version = await Connection.SendAsync("Browser.getVersion");
      return version.product.ToString();
    }

    public void Close()
    {
      Closed?.Invoke(this, new EventArgs());
    }
    #endregion
  }
}
