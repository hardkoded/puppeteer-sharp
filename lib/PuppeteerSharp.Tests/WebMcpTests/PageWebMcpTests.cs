using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WebMcpTests
{
    public class PageWebMcpTests : PuppeteerBaseTest
    {
        private static LaunchOptions WebMcpOptions() => new()
        {
            Args = new[] { "--enable-features=WebMCP" },
            AcceptInsecureCerts = true,
        };

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should list tools")]
        public async Task ShouldListTools()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var toolsAdded = new TaskCompletionSource<bool>();
            var count = 0;
            page.WebMcp.ToolsAdded += (_, _) =>
            {
                count++;
                if (count == 2)
                {
                    toolsAdded.TrySetResult(true);
                }
            };

            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'test-tool-1',
                    description: 'A test tool 1',
                    inputSchema: { type: 'object', properties: { text: { type: 'string' } }, required: ['text'] },
                    execute: (params) => {
                        return params.text;
                    },
                    annotations: { readOnlyHint: true, untrustedContentHint: true },
                });
            }");

            await page.EvaluateFunctionAsync(@"() => {
                const form = document.createElement('form');
                form.setAttribute('toolname', 'declarative tool name');
                form.setAttribute('tooldescription', 'tool description');
                document.body.appendChild(form);
            }");

            await toolsAdded.Task.WaitAsync(System.TimeSpan.FromSeconds(5));

            var tools = page.WebMcp.Tools();
            Assert.That(tools.Length, Is.GreaterThanOrEqualTo(2));
            var imperativeTool = Array.Find(tools, t => t.Name == "test-tool-1");
            Assert.That(imperativeTool, Is.Not.Null);
            Assert.That(imperativeTool.Annotations, Is.Not.Null);
            Assert.That(imperativeTool.Annotations!.ReadOnly, Is.True);
            Assert.That(imperativeTool.Annotations!.UntrustedContent, Is.True);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should fire toolsadded events")]
        public async Task ShouldFireToolsAddedEvents()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var tcs = new TaskCompletionSource<WebMcpTool[]>();
            page.WebMcp.ToolsAdded += (_, e) => tcs.TrySetResult(e.Tools);

            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'my-tool',
                    description: 'A tool',
                    execute: () => {},
                });
            }");

            var tools = await tcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));
            Assert.That(tools, Has.Length.GreaterThanOrEqualTo(1));
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should fire toolsremoved events")]
        public async Task ShouldFireToolsRemovedEvents()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var addedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => addedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"async () => {
                window._controller = new AbortController();
                await document.modelContext.registerTool({
                    name: 'removable-tool',
                    description: 'A removable tool',
                    execute: () => {},
                }, { signal: window._controller.signal });
            }");
            await addedTcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));

            var removedTcs = new TaskCompletionSource<WebMcpTool[]>();
            page.WebMcp.ToolsRemoved += (_, e) => removedTcs.TrySetResult(e.Tools);

            await page.EvaluateFunctionAsync("() => window._controller.abort()");

            var removed = await removedTcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));
            Assert.That(removed, Has.Length.GreaterThanOrEqualTo(1));
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should invoke tool")]
        public async Task ShouldInvokeTool()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var toolAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolAddedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'test-tool-1',
                    description: 'A test tool 1',
                    inputSchema: {
                        type: 'object',
                        properties: { text: { type: 'string', description: 'Some text' } },
                        required: ['text'],
                    },
                    execute: (params) => {
                        return `hello ${params.text}`;
                    },
                });
            }");

            await toolAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            var tools = page.WebMcp.Tools();
            var tool = tools[0];

            var toolCalledTcs = new TaskCompletionSource<WebMcpToolCall>();
            page.WebMcp.ToolInvoked += (_, call) => toolCalledTcs.TrySetResult(call);

            var response = await tool.ExecuteAsync(new { text = "world" });
            var call = await toolCalledTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(response.Id, Is.EqualTo(call.Id));
            Assert.That(response.Call, Is.SameAs(call));
            Assert.That(response.Status, Is.EqualTo(WebMcpInvocationStatus.Completed));
            Assert.That(response.Output?.ToString(), Contains.Substring("hello world"));
            Assert.That(response.ErrorText, Is.Null.Or.Empty);
            Assert.That(response.Exception, Is.Null);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should cancel tool execution")]
        public async Task ShouldCancelToolExecution()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var toolAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolAddedTcs.TrySetResult(true);

            // Register an imperative WebMCP tool with a delayed response.
            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'test-tool-1',
                    description: 'A test tool 1',
                    inputSchema: {
                        type: 'object',
                        properties: { text: { type: 'string', description: 'Some text' } },
                        required: ['text'],
                    },
                    execute: () => {
                        return new Promise(resolve => {
                            setTimeout(() => {
                                resolve('done');
                            }, 5000);
                        });
                    },
                });
            }");

            await toolAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            var tool = page.WebMcp.Tools()[0];

            var toolCalledTcs = new TaskCompletionSource<WebMcpToolCall>();
            page.WebMcp.ToolInvoked += (_, call) => toolCalledTcs.TrySetResult(call);

            using var cts = new CancellationTokenSource();
            var executeTask = tool.ExecuteAsync(
                new { text = "world" },
                new WebMcpToolExecuteOptions { CancellationToken = cts.Token });

            var call = await toolCalledTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await cts.CancelAsync();

            var response = await executeTask;

            Assert.That(response.Id, Is.EqualTo(call.Id));
            Assert.That(response.Call, Is.SameAs(call));
            Assert.That(response.Status, Is.EqualTo(WebMcpInvocationStatus.Canceled));
            Assert.That(response.Output, Is.Null);
            Assert.That(response.ErrorText, Is.EqualTo(string.Empty).Or.Null);
            Assert.That(response.Exception, Is.Null);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should cancel tool execution with already aborted signal")]
        public async Task ShouldCancelToolExecutionWithAlreadyAbortedSignal()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            Assert.That(page.WebMcp, Is.Not.Null);

            var toolAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolAddedTcs.TrySetResult(true);

            // Register an imperative WebMCP tool with a delayed response.
            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'test-tool-1',
                    description: 'A test tool 1',
                    inputSchema: {
                        type: 'object',
                        properties: { text: { type: 'string', description: 'Some text' } },
                        required: ['text'],
                    },
                    execute: () => {
                        return new Promise(resolve => {
                            setTimeout(() => {
                                resolve('done');
                            }, 5000);
                        });
                    },
                });
            }");

            await toolAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            var tool = page.WebMcp.Tools()[0];

            var toolCalledTcs = new TaskCompletionSource<WebMcpToolCall>();
            page.WebMcp.ToolInvoked += (_, call) => toolCalledTcs.TrySetResult(call);

            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var response = await tool.ExecuteAsync(
                new { text = "world" },
                new WebMcpToolExecuteOptions { CancellationToken = cts.Token });

            var call = await toolCalledTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(response.Id, Is.EqualTo(call.Id));
            Assert.That(response.Call, Is.SameAs(call));
            Assert.That(response.Status, Is.EqualTo(WebMcpInvocationStatus.Canceled));
            Assert.That(response.Output, Is.Null);
            Assert.That(response.ErrorText, Is.EqualTo(string.Empty).Or.Null);
            Assert.That(response.Exception, Is.Null);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should handle multiple navigations and report tools correctly")]
        public async Task ShouldHandleMultipleNavigationsAndReportToolsCorrectly()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            // 1. Register tool on first context
            var toolsAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolsAddedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"() => {
                const form = document.createElement('form');
                form.setAttribute('toolname', 'tool-1');
                form.setAttribute('tooldescription', 'desc-1');
                document.body.appendChild(form);
            }");
            await toolsAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools().Length, Is.EqualTo(1));
            Assert.That(page.WebMcp.Tools()[0].Name, Is.EqualTo("tool-1"));

            // 2. Navigate to new page - tools should be removed
            var toolsRemovedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsRemoved += (_, _) => toolsRemovedTcs.TrySetResult(true);

            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            await toolsRemovedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools(), Is.Empty);

            // 3. Register tool on second context
            toolsAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolsAddedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"() => {
                const form = document.createElement('form');
                form.setAttribute('toolname', 'tool-2');
                form.setAttribute('tooldescription', 'desc-2');
                document.body.appendChild(form);
            }");
            await toolsAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools().Length, Is.EqualTo(1));
            Assert.That(page.WebMcp.Tools()[0].Name, Is.EqualTo("tool-2"));

            // 4. Navigate again - tools should be removed again
            toolsRemovedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsRemoved += (_, _) => toolsRemovedTcs.TrySetResult(true);

            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            await toolsRemovedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools(), Is.Empty);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should not reset tools on same-document navigation")]
        public async Task ShouldNotResetToolsOnSameDocumentNavigation()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            var toolsAddedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => toolsAddedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"() => {
                const form = document.createElement('form');
                form.setAttribute('toolname', 'declarative tool name');
                form.setAttribute('tooldescription', 'tool description');
                document.body.appendChild(form);
            }");
            await toolsAddedTcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools().Length, Is.EqualTo(1));

            // Same-document (hash) navigation should not reset tools.
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html#hash");

            // Tools should still be present because context was not destroyed.
            Assert.That(page.WebMcp.Tools().Length, Is.EqualTo(1));
            Assert.That(page.WebMcp.Tools()[0].Name, Is.EqualTo("declarative tool name"));
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should remove tools on frame navigation")]
        public async Task ShouldRemoveToolsOnFrameNavigation()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            var addedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => addedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"async () => {
                await document.modelContext.registerTool({
                    name: 'nav-tool',
                    description: 'A tool',
                    execute: () => {},
                });
            }");
            await addedTcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));

            var removedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsRemoved += (_, _) => removedTcs.TrySetResult(true);

            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            await removedTcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));

            Assert.That(page.WebMcp.Tools(), Is.Empty);
        }
    }
}
