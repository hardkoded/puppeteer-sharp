// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Experimental WebMCP API. Requires Chrome 149+ with
/// --enable-features=WebMCPTesting,DevToolsWebMCPSupport flags.
/// </summary>
/// <seealso href="https://github.com/webmachinelearning/webmcp"/>
public class CdpWebMcp
{
    private readonly FrameManager _frameManager;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebMcpTool>> _tools = new();
    private readonly ConcurrentDictionary<string, WebMcpToolCall> _pendingCalls = new();
    private CDPSession _client;

    internal CdpWebMcp(CDPSession client, FrameManager frameManager)
    {
        _client = client;
        _frameManager = frameManager;
        _frameManager.FrameNavigated += OnFrameNavigated;
        BindListeners();
    }

    /// <summary>Emitted when tools are added to the page.</summary>
    public event EventHandler<WebMcpToolsAddedEventArgs> ToolsAdded;

    /// <summary>Emitted when tools are removed from the page.</summary>
    public event EventHandler<WebMcpToolsRemovedEventArgs> ToolsRemoved;

    /// <summary>Emitted when a tool invocation starts.</summary>
    public event EventHandler<WebMcpToolCall> ToolInvoked;

    /// <summary>Emitted when a tool invocation completes or fails.</summary>
    public event EventHandler<WebMcpToolCallResult> ToolResponded;

    /// <summary>
    /// Gets all WebMCP tools registered on the page.
    /// </summary>
    /// <returns>Array of registered tools.</returns>
    public WebMcpTool[] Tools()
        => _tools.Values.SelectMany(d => d.Values).ToArray();

    internal async Task InitializeAsync()
    {
        try
        {
            await _client.SendAsync("WebMCP.enable").ConfigureAwait(false);
        }
        catch
        {
            // WebMCP may not be available on older Chrome versions.
        }
    }

    internal async Task<string> InvokeToolAsync(WebMcpTool tool, object input)
    {
        var response = await _client.SendAsync<WebMcpInvokeToolResponse>(
            "WebMCP.invokeTool",
            new
            {
                frameId = tool.Frame?.Id ?? string.Empty,
                toolName = tool.Name,
                input,
            }).ConfigureAwait(false);
        return response?.InvocationId;
    }

    internal void UpdateClient(CDPSession newClient)
    {
        UnbindListeners();
        _client = newClient;
        BindListeners();
    }

    private void BindListeners()
    {
        _client.MessageReceived += OnMessageReceived;
    }

    private void UnbindListeners()
    {
        _client.MessageReceived -= OnMessageReceived;
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        switch (e.MessageID)
        {
            case "WebMCP.toolsAdded":
                OnToolsAdded(e.MessageData.ToObject<WebMcpToolsAddedProtocolEvent>());
                break;
            case "WebMCP.toolsRemoved":
                OnToolsRemoved(e.MessageData.ToObject<WebMcpToolsRemovedProtocolEvent>());
                break;
            case "WebMCP.toolInvoked":
                OnToolInvoked(e.MessageData.ToObject<WebMcpToolInvokedProtocolEvent>());
                break;
            case "WebMCP.toolResponded":
                OnToolResponded(e.MessageData.ToObject<WebMcpToolRespondedProtocolEvent>());
                break;
        }
    }

    private void OnToolsAdded(WebMcpToolsAddedProtocolEvent e)
    {
        var added = new List<WebMcpTool>();
        foreach (var tool in e.Tools)
        {
            var frame = _frameManager.FrameTree.GetById(tool.FrameId);
            if (frame == null)
            {
                continue;
            }

            var frameTools = _tools.GetOrAdd(tool.FrameId, _ => new ConcurrentDictionary<string, WebMcpTool>());

            ConsoleMessageLocation location = null;
            if (tool.StackTrace?.CallFrames?.Length > 0)
            {
                var cf = tool.StackTrace.CallFrames[0];
                location = new ConsoleMessageLocation
                {
                    URL = cf.URL,
                    LineNumber = cf.LineNumber,
                    ColumnNumber = cf.ColumnNumber,
                };
            }

            var webMcpTool = new WebMcpTool
            {
                Name = tool.Name,
                Description = tool.Description,
                InputSchema = tool.InputSchema,
                Annotations = tool.Annotations == null ? null : new WebMcpAnnotation
                {
                    ReadOnly = tool.Annotations.ReadOnly,
                    Autosubmit = tool.Annotations.Autosubmit,
                },
                Frame = frame,
                Location = location,
            };

            frameTools[tool.Name] = webMcpTool;
            added.Add(webMcpTool);
        }

        if (added.Count > 0)
        {
            ToolsAdded?.Invoke(this, new WebMcpToolsAddedEventArgs { Tools = added.ToArray() });
        }
    }

    private void OnToolsRemoved(WebMcpToolsRemovedProtocolEvent e)
    {
        var removed = new List<WebMcpTool>();
        foreach (var tool in e.Tools)
        {
            if (_tools.TryGetValue(tool.FrameId, out var frameTools) &&
                frameTools.TryGetValue(tool.Name, out var removedTool))
            {
                removed.Add(removedTool);
                frameTools.TryRemove(tool.Name, out _);
            }
        }

        if (removed.Count > 0)
        {
            ToolsRemoved?.Invoke(this, new WebMcpToolsRemovedEventArgs { Tools = removed.ToArray() });
        }
    }

    private void OnToolInvoked(WebMcpToolInvokedProtocolEvent e)
    {
        if (!_tools.TryGetValue(e.FrameId, out var frameTools) ||
            !frameTools.TryGetValue(e.ToolName, out var tool))
        {
            return;
        }

        var call = new WebMcpToolCall(e.InvocationId, tool, e.Input ?? "{}");
        _pendingCalls[call.Id] = call;
        ToolInvoked?.Invoke(this, call);
    }

    private void OnToolResponded(WebMcpToolRespondedProtocolEvent e)
    {
        _pendingCalls.TryRemove(e.InvocationId, out var call);

        var status = e.Status switch
        {
            "Completed" => WebMcpInvocationStatus.Completed,
            "Canceled" => WebMcpInvocationStatus.Canceled,
            _ => WebMcpInvocationStatus.Error,
        };

        var result = new WebMcpToolCallResult
        {
            Id = e.InvocationId,
            Call = call,
            Status = status,
            Output = e.Output,
            ErrorText = e.ErrorText,
            Exception = e.Exception,
        };

        ToolResponded?.Invoke(this, result);
    }

    private void OnFrameNavigated(object sender, FrameNavigatedEventArgs e)
    {
        var frameId = e.Frame?.Id;
        if (string.IsNullOrEmpty(frameId) || !_tools.TryGetValue(frameId, out var frameTools))
        {
            return;
        }

        var tools = frameTools.Values.ToArray();
        _tools.TryRemove(frameId, out _);

        if (tools.Length > 0)
        {
            ToolsRemoved?.Invoke(this, new WebMcpToolsRemovedEventArgs { Tools = tools });
        }
    }

    private class WebMcpToolsAddedProtocolEvent
    {
        [JsonPropertyName("tools")]
        public WebMcpProtocolTool[] Tools { get; set; }
    }

    private class WebMcpToolsRemovedProtocolEvent
    {
        [JsonPropertyName("tools")]
        public WebMcpProtocolRemovedTool[] Tools { get; set; }
    }

    private class WebMcpToolInvokedProtocolEvent
    {
        [JsonPropertyName("frameId")]
        public string FrameId { get; set; }

        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("invocationId")]
        public string InvocationId { get; set; }

        [JsonPropertyName("toolName")]
        public string ToolName { get; set; }
    }

    private class WebMcpToolRespondedProtocolEvent
    {
        [JsonPropertyName("errorText")]
        public string ErrorText { get; set; }

        [JsonPropertyName("exception")]
        public RemoteObject Exception { get; set; }

        [JsonPropertyName("invocationId")]
        public string InvocationId { get; set; }

        [JsonPropertyName("output")]
        public object Output { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    private class WebMcpProtocolAnnotation
    {
        [JsonPropertyName("autosubmit")]
        public bool? Autosubmit { get; set; }

        [JsonPropertyName("readOnly")]
        public bool? ReadOnly { get; set; }
    }

    private class WebMcpProtocolRemovedTool
    {
        [JsonPropertyName("frameId")]
        public string FrameId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    private class WebMcpProtocolTool
    {
        [JsonPropertyName("annotations")]
        public WebMcpProtocolAnnotation Annotations { get; set; }

        [JsonPropertyName("backendNodeId")]
        public int? BackendNodeId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("frameId")]
        public string FrameId { get; set; }

        [JsonPropertyName("inputSchema")]
        public object InputSchema { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("stackTrace")]
        public StackTrace StackTrace { get; set; }
    }

    private class WebMcpInvokeToolResponse
    {
        [JsonPropertyName("invocationId")]
        public string InvocationId { get; set; }
    }
}
