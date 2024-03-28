using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkManagerTests;

public class NetworkManagerTests : PuppeteerPageBaseTest
{
    // There are some missing calls in this function, but this is enough.
    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager", "should process extra info on multiple redirects")]
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

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should handle \"double pause\" (crbug.com/1196004) Fetch.requestPaused events for the same Network.requestWillBeSent event")]
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

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should handle Network.responseReceivedExtraInfo event after Network.responseReceived event (github.com/puppeteer/puppeteer/issues/8234)")]
    public async Task ShouldHandleResponseReceivedExtraInfo()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        var requests = new List<IRequest>();
        manager.RequestFinished += (sender, e) =>
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
                    RequestId = "1360.2",
                    LoaderId = "9E86B0282CC98B77FB0ABD49156DDFDD",
                    Request = new Payload { Url = "http://this.is.a.test.com:1080/test.js", Method = HttpMethod.Get, },
                    Initiator = new Initiator()
                    {
                        Type = InitiatorType.Parser,
                        Url = "http://this.is.the.start.page.com/",
                        LineNumber = 9,
                        ColumnNumber = 80
                    },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Script,
                    FrameId = "60E6C35E7E519F28E646056820095498",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "1360.2",
                    Response = new ResponsePayload()
                    {
                        Url = "http://this.is.a.test.com:1080",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "connection", "keep-alive" }, { "content-length", "85862" },
                        },
                        RemoteIPAddress = "127.0.0.1",
                        RemotePort = 1080,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "1360.2",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData = JToken.FromObject(new LoadingFinishedEventResponse { RequestId = "1360.2", })
            });

        Assert.AreEqual(1, requests.Count);
    }

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should resolve the response once the late responseReceivedExtraInfo event arrives")]
    public async Task ShouldResolveTheResponseOnceTheLateResponseReceivedExtraInfoEventArrives()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        var finishedRequests = new List<IRequest>();
        var pendingRequests = new List<IRequest>();

        manager.RequestFinished += (sender, e) => finishedRequests.Add(e.Request);
        manager.Request += (sender, e) => pendingRequests.Add(e.Request);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload()
                {
                    RequestId = "LOADERID",
                    LoaderId = "LOADERID",
                    Request = new Payload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "Upgrade-Insecure-Requests", "1" },
                            {
                                "User-Agent",
                                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"
                            },
                        },
                    },
                    Initiator = new Initiator() { Type = InitiatorType.Other, },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "LOADERID",
                    Response = new ResponsePayload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "Cache-Control", "no-cache, no-store" },
                            { "Connection", "keep-alive" },
                            { "Content-Length", "0" },
                            { "Content-Type", "text/html; charset=utf-8" },
                            { "Date", "Wed, 10 Aug 2022 08:45:57 GMT" },
                            { "Keep-Alive", "timeout=5" },
                        },
                        RemoteIPAddress = "10.1.0.39",
                        RemotePort = 42915,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData = JToken.FromObject(new LoadingFinishedEventResponse() { RequestId = "LOADERID", })
            });

        Assert.AreEqual(1, pendingRequests.Count);
        Assert.AreEqual(0, finishedRequests.Count);
        Assert.Null(pendingRequests[0].Response);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "LOADERID",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                })
            });

        Assert.AreEqual(1, pendingRequests.Count);
        Assert.AreEqual(1, finishedRequests.Count);
        Assert.NotNull(pendingRequests[0].Response);
    }

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should send responses for iframe that don't receive loadingFinished event")]
    public async Task ShouldSendResponsesForIframeThatDontReceiveLoadingFinishedEvent()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        var responses = new List<IResponse>();
        var requests = new List<IRequest>();

        manager.Request += (sender, e) => requests.Add(e.Request);
        manager.Response += (sender, e) => responses.Add(e.Response);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "94051D839ACF29E53A3D1273FB20B4C4",
                    LoaderId = "94051D839ACF29E53A3D1273FB20B4C4",
                    Request = new Payload
                    {
                        Url = "http://localhost:8080/iframe.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "1",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "94051D839ACF29E53A3D1273FB20B4C4",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "94051D839ACF29E53A3D1273FB20B4C4",
                    Response = new ResponsePayload()
                    {
                        Url = "http://this.is.a.test.com:1080",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "connection", "keep-alive" }, { "content-length", "85862" },
                        },
                        RemoteIPAddress = "127.0.0.1",
                        RemotePort = 1080,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        Assert.AreEqual(1, responses.Count);
        Assert.AreEqual(1, requests.Count);
        Assert.NotNull(requests[0].Response);
    }

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager",
        "should send responses for iframe that don't receive loadingFinished event")]
    public async Task ShouldSendResponsesForIframeThatDontReceiveLoadingFinishedEvent2()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        var responses = new List<IResponse>();
        var requests = new List<IRequest>();

        manager.Request += (sender, e) => requests.Add(e.Request);
        manager.Response += (sender, e) => responses.Add(e.Response);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload
                {
                    RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    LoaderId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    Request = new Payload
                    {
                        Url = "http://localhost:8080/iframe.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "F9C89A517341F1EFFE63310141630189",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    Response = new ResponsePayload()
                    {
                        Url = "http://this.is.a.test.com:1080",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "connection", "keep-alive" }, { "content-length", "85862" },
                        },
                        RemoteIPAddress = "127.0.0.1",
                        RemotePort = 1080,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JToken.FromObject(new LoadingFinishedEventResponse()
                    {
                        RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                })
            });

        Assert.AreEqual(1, requests.Count);
        Assert.AreEqual(1, responses.Count);
        Assert.NotNull(requests[0].Response);
    }

    [Test, Retry(2), PuppeteerTest("NetworkManager.test.ts", "NetworkManager", "should handle cached redirects")]
    public async Task ShouldHandleCachedRedirects()
    {
        var client = Substitute.For<ICDPSession>();

        var frameManagerMock = Substitute.For<IFrameProvider>();
        using var loggerFactory = new LoggerFactory();
        var manager = new NetworkManager(true, frameManagerMock, loggerFactory);

        await manager.AddClientAsync(client);
        var responses = new List<IResponse>();
        manager.Response += (sender, e) => responses.Add(e.Response);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload()
                {
                    RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    LoaderId = "6D76C8ACAECE880C722FA515AD380015",
                    Request = new Payload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "Upgrade-Insecure-Requests", "1" },
                            {
                                "User-Agent",
                                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"
                            },
                        },
                    },
                    Initiator = new Initiator() { Type = InitiatorType.Other, },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    Response = new ResponsePayload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "Cache-Control", "no-cache, no-store" },
                            { "Connection", "keep-alive" },
                            { "Content-Length", "0" },
                            { "Content-Type", "text/html; charset=utf-8" },
                            { "Date", "Wed, 10 Aug 2022 08:45:57 GMT" },
                            { "Keep-Alive", "timeout=5" },
                        },
                        RemoteIPAddress = "10.1.0.39",
                        RemotePort = 42915,
                        FromDiskCache = false,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JToken.FromObject(new LoadingFinishedEventResponse()
                    {
                        RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload()
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    LoaderId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Request = new Payload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "Upgrade-Insecure-Requests", "1" },
                            {
                                "User-Agent",
                                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"
                            },
                        },
                    },
                    Initiator = new Initiator() { Type = InitiatorType.Other, },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.Redirect,
                    HeadersText = "HTTP/1.1 302 Found\\r\\nLocation: http://localhost:3000/#from-redirect\\r\\nDate: Wed, 05 Apr 2023 12:39:13 GMT\\r\\nConnection: keep-alive\\r\\nKeep-Alive: timeout=5\\r\\nTransfer-Encoding: chunked\\r\\n\\r\\n",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JToken.FromObject(new RequestWillBeSentPayload()
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    LoaderId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Request = new Payload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>
                        {
                            { "Upgrade-Insecure-Requests", "1" },
                            {
                                "User-Agent",
                                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36"
                            },
                        },
                    },
                    Initiator = new Initiator() { Type = InitiatorType.Other, },
                    RedirectHasExtraInfo = true,
                    RedirectResponse = new ResponsePayload()
                    {
                        Url = "http://localhost:3000/redirect",
                        Status = HttpStatusCode.Redirect,
                        StatusText = "Found",
                    },
                    Type = ResourceType.Document,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JToken.FromObject(new ResponseReceivedResponse
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Response = new ResponsePayload()
                    {
                        Url = "http://10.1.0.39:42915/empty.html",
                        Status = HttpStatusCode.OK,
                        StatusText = "OK",
                        Headers = new Dictionary<string, string>
                        {
                            { "Cache-Control", "no-cache, no-store" },
                            { "Connection", "keep-alive" },
                            { "Content-Length", "0" },
                            { "Content-Type", "text/html; charset=utf-8" },
                            { "Date", "Wed, 10 Aug 2022 08:45:57 GMT" },
                            { "Keep-Alive", "timeout=5" },
                        },
                        RemoteIPAddress = "10.1.0.39",
                        RemotePort = 42915,
                        FromDiskCache = true,
                        FromServiceWorker = false,
                    },
                    HasExtraInfo = true,
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JToken.FromObject(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.Redirect,
                    HeadersText = "HTTP/1.1 302 Found",
                })
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JToken.FromObject(new LoadingFinishedEventResponse()
                    {
                        RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    })
            });

        Assert.AreEqual(new[] { HttpStatusCode.OK, HttpStatusCode.Found, HttpStatusCode.OK }, responses.Select(response => response.Status));
    }
}
