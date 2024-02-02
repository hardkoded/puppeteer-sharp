using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkManagerTests;

public class NetworkManagerTests : PuppeteerPageBaseTest
{
    // There are some missing calls in this function, but this is enough.
    [PuppeteerTest("NetworkManager.test.ts", "NetworkManager", "should process extra info on multiple redirects")]
    [PuppeteerTimeout]
    public async Task ShouldProcessExtraInfoOnMultipleRedirects()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Payload() { Url = "http://localhost:8907/redirect/1.html", Method = HttpMethod.Get },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "099A5216AF03AAFEC988F214B024DF08",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Payload() { Url = "http://localhost:8907/redirect/2.html", Method = HttpMethod.Get, },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = true,
                    RedirectResponse = new ResponsePayload
                    {
                        Url = "http://localhost:8907/redirect/1.html",
                        Status = System.Net.HttpStatusCode.Found,
                        StatusText = "Found",
                        RemoteIPAddress = "[::1]",
                        RemotePort = 8907,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    Type = ResourceType.Document,
                    FrameId = "099A5216AF03AAFEC988F214B024DF08",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Payload { Url = "http://localhost:8907/redirect/3.html", Method = HttpMethod.Get, },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = true,
                    RedirectResponse = new ResponsePayload
                    {
                        Url = "http://localhost:8907/redirect/2.html",
                        Status = System.Net.HttpStatusCode.Found,
                        StatusText = "Found",
                        RemoteIPAddress = "[::1]",
                        RemotePort = 8907,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    Type = ResourceType.Document,
                    FrameId = "099A5216AF03AAFEC988F214B024DF08",
                })
            });
    }

    [PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should handle \"double pause\" (crbug.com/1196004) Fetch.requestPaused events for the same Network.requestWillBeSent event")]
    [PuppeteerTimeout]
    public async Task ShouldHandleDoublePause()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        await manager.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        manager.Request += (sender, e) =>
        {
            requests.Add(e.Request);
            e.Request.ContinueAsync();
        };

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "11ACE9783588040D644B905E8B55285B",
                    LoaderId = "11ACE9783588040D644B905E8B55285B",
                    Request = new Payload()
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Fetch.requestPaused",
                MessageData = JToken.FromObject(new FetchRequestPausedResponse
                {
                    RequestId = "interception-job-1.0",
                    Request = new Payload
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                    ResourceType = ResourceType.Document,
                    NetworkId = "11ACE9783588040D644B905E8B55285B",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Fetch.requestPaused",
                MessageData = JToken.FromObject(new FetchRequestPausedResponse
                {
                    RequestId = "interception-job-2.0",
                    Request = new Payload()
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                    ResourceType = ResourceType.Document,
                    NetworkId = "11ACE9783588040D644B905E8B55285B",
                })
            });

        Assert.AreEqual(2, requests.Count);
    }
}
