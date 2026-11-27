using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.PageAccessibility;

namespace PuppeteerSharp.Tests.AccessibilityTests
{
    public class CdpAccessibilityTests
    {
        [
            Test,
            PuppeteerTest(
                "Accessibility.test.ts",
                "Accessibility",
                "skips iframes outside the root subtree and misses"
            )
        ]
        public async Task SkipsIframesOutsideTheRootSubtreeAndMisses()
        {
            var adoptedBackendNodeIds = new List<int>();
            var describedBackendNodeId = 10;

            var client = CreateClient(method =>
                method switch
                {
                    "Accessibility.getFullAXTree" => new
                    {
                        nodes = new[]
                        {
                            CreateAXNode(
                                "root",
                                "RootWebArea",
                                ["outside-iframe", "target-region"]
                            ),
                            CreateAXNode("outside-iframe", "Iframe", [], 99),
                            CreateAXNode("target-region", "region", ["inside-iframe"], 10),
                            CreateAXNode("inside-iframe", "Iframe", [], 20),
                        },
                    },
                    "DOM.describeNode" => new
                    {
                        node = new { backendNodeId = describedBackendNodeId },
                    },
                    _ => throw new InvalidOperationException($"Unexpected CDP method: {method}"),
                }
            );

            var realm = Substitute.For<Realm>(new TimeoutSettings());
            realm
                .AdoptBackendNodeAsync(Arg.Any<object>())
                .Returns(callInfo =>
                {
                    adoptedBackendNodeIds.Add((int)callInfo.Arg<object>());
                    return Task.FromResult(Substitute.For<IElementHandle>());
                });

            var rootHandle = Substitute.For<IElementHandle, ICdpHandle>();
            ((ICdpHandle)rootHandle).RemoteObject.Returns(new RemoteObject { ObjectId = "obj-10" });
            var accessibility = new Accessibility(client, () => string.Empty, () => realm);

            await accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { Root = rootHandle, IncludeIframes = true }
            );
            Assert.That(adoptedBackendNodeIds, Is.EqualTo(new[] { 20 }));

            adoptedBackendNodeIds.Clear();
            describedBackendNodeId = 999;
            var missing = await accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { Root = rootHandle, IncludeIframes = true }
            );
            Assert.That(missing, Is.Null);
            Assert.That(adoptedBackendNodeIds, Is.Empty);
        }

        [
            Test,
            PuppeteerTest(
                "Accessibility.test.ts",
                "Accessibility",
                "ignores detached iframe errors from adoptBackendNode"
            )
        ]
        public async Task IgnoresDetachedIframeErrorsFromAdoptBackendNode()
        {
            var client = CreateClient(_ => new
            {
                nodes = new[]
                {
                    CreateAXNode("root", "RootWebArea", ["button", "iframe"]),
                    CreateAXNode("button", "button", [], 30, "Ok"),
                    CreateAXNode("iframe", "Iframe", [], 20),
                },
            });

            var logger = Substitute.For<ILogger>();
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
            client.LoggerFactory.Returns(loggerFactory);

            var realm = Substitute.For<Realm>(new TimeoutSettings());
            realm
                .AdoptBackendNodeAsync(Arg.Any<object>())
                .ThrowsAsync(
                    new PuppeteerException("Protocol error (DOM.resolveNode): No node found")
                );

            var accessibility = new Accessibility(client, () => string.Empty, () => realm);
            var snapshot = await accessibility.SnapshotAsync(
                new AccessibilitySnapshotOptions { IncludeIframes = true, InterestingOnly = false }
            );

            Assert.That(snapshot.Role, Is.EqualTo("RootWebArea"));
            Assert.That(snapshot.Children, Has.Length.EqualTo(2));
            Assert.That(snapshot.Children[0].Role, Is.EqualTo("button"));
            Assert.That(snapshot.Children[0].Name, Is.EqualTo("Ok"));
            Assert.That(snapshot.Children[1].Role, Is.EqualTo("Iframe"));
            Assert.That(
                logger.ReceivedCalls().Count(c => c.GetMethodInfo().Name == nameof(ILogger.Log)),
                Is.EqualTo(1)
            );
        }

        private static Dictionary<string, object> CreateAXNode(
            string nodeId,
            string role,
            string[] childIds,
            int? backendDOMNodeId = null,
            string name = null
        )
        {
            var node = new Dictionary<string, object>
            {
                ["nodeId"] = nodeId,
                ["ignored"] = false,
                ["role"] = new { type = "role", value = role },
                ["childIds"] = childIds,
            };

            if (name != null)
            {
                node["name"] = new { type = "computedString", value = name };
            }

            if (backendDOMNodeId != null)
            {
                node["backendDOMNodeId"] = backendDOMNodeId;
            }

            return node;
        }

        private static CDPSession CreateClient(Func<string, object> respond)
        {
            var client = Substitute.For<CDPSession>();
            client
                .SendAsync(
                    Arg.Any<string>(),
                    Arg.Any<object>(),
                    Arg.Any<bool>(),
                    Arg.Any<CommandOptions>()
                )
                .Returns(callInfo =>
                    Task.FromResult<JsonElement?>(
                        JsonSerializer.SerializeToElement(respond(callInfo.ArgAt<string>(0)))
                    )
                );
            return client;
        }
    }
}
