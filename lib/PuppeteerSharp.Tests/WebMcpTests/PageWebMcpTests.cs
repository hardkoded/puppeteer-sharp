using System;
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
            Args = new[] { "--enable-features=WebMCPTesting,DevToolsWebMCPSupport" },
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

            await page.EvaluateFunctionAsync(@"() => {
                window.navigator.modelContext.registerTool({
                    name: 'test-tool-1',
                    description: 'A test tool 1',
                    inputSchema: { type: 'object', properties: { text: { type: 'string' } }, required: ['text'] },
                    execute: () => {},
                    annotations: { readOnlyHint: true },
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

            await page.EvaluateFunctionAsync(@"() => {
                window.navigator.modelContext.registerTool({
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

            await page.EvaluateFunctionAsync(@"() => {
                window._tool = window.navigator.modelContext.registerTool({
                    name: 'removable-tool',
                    description: 'A removable tool',
                    execute: () => {},
                });
            }");
            await addedTcs.Task.WaitAsync(System.TimeSpan.FromSeconds(5));

            var removedTcs = new TaskCompletionSource<WebMcpTool[]>();
            page.WebMcp.ToolsRemoved += (_, e) => removedTcs.TrySetResult(e.Tools);

            await page.EvaluateFunctionAsync("() => window._tool.unregister()");

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

            await page.EvaluateFunctionAsync(@"() => {
                window.navigator.modelContext.registerTool({
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
            Assert.That(response.ErrorText, Is.Null);
            Assert.That(response.Exception, Is.Null);
        }

        [Test, PuppeteerTest("webmcp.spec", "Page.webmcp", "should remove tools on frame navigation")]
        public async Task ShouldRemoveToolsOnFrameNavigation()
        {
            await using var browser = await Puppeteer.LaunchAsync(WebMcpOptions(), TestConstants.LoggerFactory);
            var page = (CdpPage)await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            var addedTcs = new TaskCompletionSource<bool>();
            page.WebMcp.ToolsAdded += (_, _) => addedTcs.TrySetResult(true);

            await page.EvaluateFunctionAsync(@"() => {
                window.navigator.modelContext.registerTool({
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
