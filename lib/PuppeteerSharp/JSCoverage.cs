using PuppeteerSharp.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class JSCoverage
    {
        private readonly Session _client;
        private readonly Dictionary<string, string> _scriptURLs;
        private readonly Dictionary<string, string> _scriptSources;

        private bool _enabled;
        private bool _resetOnNavigation;

        public JSCoverage(Session client)
        {
            _client = client;
            _enabled = false;
            _scriptURLs = new Dictionary<string, string>();
            _scriptSources = new Dictionary<string, string>();

            _resetOnNavigation = false;
        }

        internal async Task StartAsync(JSCoverageStartOptions options)
        {
            if (_enabled)
            {
                throw new InvalidOperationException("JSCoverage is already enabled");
            }

            _resetOnNavigation = options.ResetOnNavigation;
            _enabled = true;
            _scriptURLs.Clear();
            _scriptSources.Clear();

            _client.MessageReceived += client_MessageReceived;

            await Task.WhenAll(
                _client.SendAsync("Profiler.enable"),
                _client.SendAsync("Profiler.startPreciseCoverage", new { callCount = false, detailed = true }),
                _client.SendAsync("Debugger.enable"),
                _client.SendAsync("Debugger.setSkipAllPauses", new { skip = true })
            );
        }

        internal async Task StopAsync()
        {
            if (!_enabled)
            {
                throw new InvalidOperationException("JSCoverage is not enabled");
            }
            _enabled = false;

            var profileResponseTask = _client.SendAsync("Profiler.takePreciseCoverage");
            await Task.WhenAll(
               profileResponseTask,
               _client.SendAsync("Profiler.stopPreciseCoverage"),
               _client.SendAsync("Profiler.disable"),
               _client.SendAsync("Debugger.disable")
           );
            _client.MessageReceived -= client_MessageReceived;
            
            // TODO: return coverage;            
        }

        private void client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Debugger.scriptParsed":
                    OnScriptParsed();
                    break;
                case "Runtime.executionContextsCleared":
                    OnExecutionContextsCleared(e.MessageData.ToObject<RuntimeExecutionContextsClearedResponse>());
                    break;
            }
        }

        private void OnScriptParsed()
        {
            if (!_resetOnNavigation)
            {
                return;
            }

            _scriptURLs.Clear();
            _scriptSources.Clear();
        }

        private async void OnExecutionContextsCleared(RuntimeExecutionContextsClearedResponse @event)
        {
            if (@event.Url != null)
            {
                return;
            }

            try
            {
                var response = await _client.SendAsync("Debugger.getScriptSource", new { scriptId = @event.ScriptId });
                _scriptURLs.Add(@event.ScriptId, @event.Url);
                _scriptSources.Add(@event.ScriptId, response.scriptSource.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}