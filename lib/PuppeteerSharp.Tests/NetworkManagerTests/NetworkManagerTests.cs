using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Request() { Url = "http://localhost:8907/redirect/1.html", Method = HttpMethod.Get },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "099A5216AF03AAFEC988F214B024DF08",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Request() { Url = "http://localhost:8907/redirect/2.html", Method = HttpMethod.Get, },
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "7760711DEFCFA23132D98ABA6B4E175C",
                    LoaderId = "7760711DEFCFA23132D98ABA6B4E175C",
                    Request = new Request { Url = "http://localhost:8907/redirect/3.html", Method = HttpMethod.Get, },
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "11ACE9783588040D644B905E8B55285B",
                    LoaderId = "11ACE9783588040D644B905E8B55285B",
                    Request = new Request()
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Fetch.requestPaused",
                MessageData = JsonSerializer.SerializeToElement(new FetchRequestPausedResponse
                {
                    RequestId = "interception-job-1.0",
                    Request = new Request
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                    ResourceType = ResourceType.Document,
                    NetworkId = "11ACE9783588040D644B905E8B55285B",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Fetch.requestPaused",
                MessageData = JsonSerializer.SerializeToElement(new FetchRequestPausedResponse
                {
                    RequestId = "interception-job-2.0",
                    Request = new Request()
                    {
                        Url = "https://www.google.com/",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    FrameId = "84AC261A351B86932B775B76D1DD79F8",
                    ResourceType = ResourceType.Document,
                    NetworkId = "11ACE9783588040D644B905E8B55285B",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(requests, Has.Count.EqualTo(2));
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "1360.2",
                    LoaderId = "9E86B0282CC98B77FB0ABD49156DDFDD",
                    Request = new Request { Url = "http://this.is.a.test.com:1080/test.js", Method = HttpMethod.Get, },
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "1360.2",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData = JsonSerializer.SerializeToElement(new LoadingFinishedEventResponse { RequestId = "1360.2", }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(requests, Has.Count.EqualTo(1));
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse()
                {
                    RequestId = "LOADERID",
                    LoaderId = "LOADERID",
                    Request = new Request()
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData = JsonSerializer.SerializeToElement(new LoadingFinishedEventResponse() { RequestId = "LOADERID", }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(pendingRequests, Has.Count.EqualTo(1));
        Assert.That(finishedRequests, Is.Empty);
        Assert.That(pendingRequests[0].Response, Is.Null);

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "LOADERID",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(pendingRequests, Has.Count.EqualTo(1));
        Assert.That(finishedRequests, Has.Count.EqualTo(1));
        Assert.That(pendingRequests[0].Response, Is.Not.Null);
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "94051D839ACF29E53A3D1273FB20B4C4",
                    LoaderId = "94051D839ACF29E53A3D1273FB20B4C4",
                    Request = new Request
                    {
                        Url = "http://localhost:8080/iframe.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "1",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "94051D839ACF29E53A3D1273FB20B4C4",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(responses, Has.Count.EqualTo(1));
        Assert.That(requests, Has.Count.EqualTo(1));
        Assert.That(requests[0].Response, Is.Not.Null);
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse
                {
                    RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    LoaderId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    Request = new Request
                    {
                        Url = "http://localhost:8080/iframe.html",
                        Method = HttpMethod.Get,
                        Headers = new Dictionary<string, string>(),
                    },
                    Initiator = new Initiator { Type = InitiatorType.Other },
                    RedirectHasExtraInfo = false,
                    Type = ResourceType.Document,
                    FrameId = "F9C89A517341F1EFFE63310141630189",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JsonSerializer.SerializeToElement(new LoadingFinishedEventResponse()
                    {
                        RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "E18BEB94B486CA8771F9AFA2030FEA37",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(requests, Has.Count.EqualTo(1));
        Assert.That(responses, Has.Count.EqualTo(1));
        Assert.That(requests[0].Response, Is.Not.Null);
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
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse()
                {
                    RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    LoaderId = "6D76C8ACAECE880C722FA515AD380015",
                    Request = new Request()
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.OK,
                    HeadersText = "HTTP/1.1 200 OK\r\nconnection: keep-alive\r\ncontent-length: 85862\r\n\r\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JsonSerializer.SerializeToElement(new LoadingFinishedEventResponse()
                    {
                        RequestId = "6D76C8ACAECE880C722FA515AD380015",
                    }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse()
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    LoaderId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Request = new Request()
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.Redirect,
                    HeadersText = "HTTP/1.1 302 Found\\r\\nLocation: http://localhost:3000/#from-redirect\\r\\nDate: Wed, 05 Apr 2023 12:39:13 GMT\\r\\nConnection: keep-alive\\r\\nKeep-Alive: timeout=5\\r\\nTransfer-Encoding: chunked\\r\\n\\r\\n",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.requestWillBeSent",
                MessageData = JsonSerializer.SerializeToElement(new RequestWillBeSentResponse()
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    LoaderId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Request = new Request()
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceived",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedResponse
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
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.responseReceivedExtraInfo",
                MessageData = JsonSerializer.SerializeToElement(new ResponseReceivedExtraInfoResponse
                {
                    RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    Headers = new Dictionary<string, string>
                    {
                        { "connection", "keep-alive" }, { "content-length", "85862" },
                    },
                    StatusCode = HttpStatusCode.Redirect,
                    HeadersText = "HTTP/1.1 302 Found",
                }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        client.MessageReceived += Raise.EventWith(
            client,
            new MessageEventArgs()
            {
                MessageID = "Network.loadingFinished",
                MessageData =
                    JsonSerializer.SerializeToElement(new LoadingFinishedEventResponse()
                    {
                        RequestId = "4C2CC44FB6A6CAC5BE2780BCC9313105",
                    }, JsonHelper.DefaultJsonSerializerSettings.Value)
            });

        Assert.That(responses.Select(response => response.Status), Is.EqualTo(new[] { HttpStatusCode.OK, HttpStatusCode.Found, HttpStatusCode.OK }));
    }
}
