using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkRestrictionTests;

public class WebRtcAndWebTransportTests : PuppeteerBaseTest
{
    /// <summary>
    /// The time to wait for traffic before the test concludes that the browser did not send any.
    /// </summary>
    private const int NoTrafficTimeout = 2000;

    private static readonly string[] TrafficNames =
    [
        "WebTransport packets",
        "WebRTC STUN requests",
        "WebRTC TURN requests over UDP",
        "WebRTC TURN requests over TCP",
    ];

    private enum Traffic
    {
        WebTransport,
        Stun,
        TurnUdp,
        TurnTcp,
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport blocklist",
            "should not send WebTransport packets to hosts in the blocklist"
        )
    ]
    public Task ShouldNotSendWebTransportPacketsToHostsInTheBlocklist() =>
        ShouldNotSendToBlockedHostAsync(Traffic.WebTransport);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport blocklist",
            "should not send WebRTC STUN requests to hosts in the blocklist"
        )
    ]
    public Task ShouldNotSendWebRtcStunRequestsToHostsInTheBlocklist() =>
        ShouldNotSendToBlockedHostAsync(Traffic.Stun);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport blocklist",
            "should not send WebRTC TURN requests over UDP to hosts in the blocklist"
        )
    ]
    public Task ShouldNotSendWebRtcTurnRequestsOverUdpToHostsInTheBlocklist() =>
        ShouldNotSendToBlockedHostAsync(Traffic.TurnUdp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport blocklist",
            "should not send WebRTC TURN requests over TCP to hosts in the blocklist"
        )
    ]
    public Task ShouldNotSendWebRtcTurnRequestsOverTcpToHostsInTheBlocklist() =>
        ShouldNotSendToBlockedHostAsync(Traffic.TurnTcp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport allowlist",
            "should not send WebTransport packets to hosts not in the allowlist"
        )
    ]
    public Task ShouldNotSendWebTransportPacketsToHostsNotInTheAllowlist() =>
        ShouldNotSendToNotAllowedHostAsync(Traffic.WebTransport);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport allowlist",
            "should not send WebRTC STUN requests to hosts not in the allowlist"
        )
    ]
    public Task ShouldNotSendWebRtcStunRequestsToHostsNotInTheAllowlist() =>
        ShouldNotSendToNotAllowedHostAsync(Traffic.Stun);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport allowlist",
            "should not send WebRTC TURN requests over UDP to hosts not in the allowlist"
        )
    ]
    public Task ShouldNotSendWebRtcTurnRequestsOverUdpToHostsNotInTheAllowlist() =>
        ShouldNotSendToNotAllowedHostAsync(Traffic.TurnUdp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport allowlist",
            "should not send WebRTC TURN requests over TCP to hosts not in the allowlist"
        )
    ]
    public Task ShouldNotSendWebRtcTurnRequestsOverTcpToHostsNotInTheAllowlist() =>
        ShouldNotSendToNotAllowedHostAsync(Traffic.TurnTcp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport allowlist",
            "should not connect WebRTC peers"
        )
    ]
    public async Task ShouldNotConnectWebRtcPeersWithAllowlist()
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://localhost:*/*"];

        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");

        Assert.That(await ConnectWebRtcPeersAsync(page, NoTrafficTimeout), Is.False);
    }

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport no restrictions",
            "should send WebTransport packets"
        )
    ]
    public Task ShouldSendWebTransportPackets() => ShouldSendTrafficAsync(Traffic.WebTransport);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport no restrictions",
            "should send WebRTC STUN requests"
        )
    ]
    public Task ShouldSendWebRtcStunRequests() => ShouldSendTrafficAsync(Traffic.Stun);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport no restrictions",
            "should send WebRTC TURN requests over UDP"
        )
    ]
    public Task ShouldSendWebRtcTurnRequestsOverUdp() => ShouldSendTrafficAsync(Traffic.TurnUdp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport no restrictions",
            "should send WebRTC TURN requests over TCP"
        )
    ]
    public Task ShouldSendWebRtcTurnRequestsOverTcp() => ShouldSendTrafficAsync(Traffic.TurnTcp);

    [
        Test,
        PuppeteerTest(
            "network_restrictions.spec",
            "Network Restrictions WebRTC and WebTransport no restrictions",
            "should connect WebRTC peers"
        )
    ]
    public async Task ShouldConnectWebRtcPeers()
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");

        Assert.That(await ConnectWebRtcPeersAsync(page, 30_000), Is.True);
    }

    private static async Task<bool> SendsTrafficAsync(
        IPage page,
        Traffic traffic,
        int? timeout = null
    )
    {
        var tcp = traffic == Traffic.TurnTcp;
        using var udpClient = tcp ? null : new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var tcpListener = tcp ? new TcpListener(IPAddress.Loopback, 0) : null;

        try
        {
            tcpListener?.Start();
            var port = tcp
                ? ((IPEndPoint)tcpListener.LocalEndpoint).Port
                : ((IPEndPoint)udpClient.Client.LocalEndPoint).Port;

            // The listener does not reply: it only shows if the browser started a connection.
            var received = tcp ? WaitForTcpAsync(tcpListener) : WaitForUdpAsync(udpClient);

            await StartTrafficAsync(page, traffic, port);

            var limit = timeout ?? 30_000;
            return await Task.WhenAny(received, Task.Delay(limit)) == received && await received;
        }
        finally
        {
            tcpListener?.Stop();
        }
    }

    private static async Task<bool> WaitForTcpAsync(TcpListener listener)
    {
        try
        {
            using var client = await listener.AcceptTcpClientAsync();
            return true;
        }
        catch (Exception ex)
            when (ex is ObjectDisposedException or SocketException or InvalidOperationException)
        {
            return false;
        }
    }

    private static async Task<bool> WaitForUdpAsync(UdpClient client)
    {
        try
        {
            await client.ReceiveAsync();
            return true;
        }
        catch (Exception ex) when (ex is ObjectDisposedException or SocketException)
        {
            return false;
        }
    }

    private static Task StartTrafficAsync(IPage page, Traffic traffic, int port) =>
        traffic switch
        {
            Traffic.WebTransport => page.EvaluateFunctionAsync(
                @"(url) => {
                    const transport = new WebTransport(url);
                    transport.ready.catch(() => {});
                    transport.closed.catch(() => {});
                    window.transport = transport;
                }",
                $"https://127.0.0.1:{port}/webtransport"
            ),
            Traffic.Stun => StartIceGatheringAsync(page, $"stun:127.0.0.1:{port}"),
            Traffic.TurnUdp => StartIceGatheringAsync(page, $"turn:127.0.0.1:{port}?transport=udp"),
            _ => StartIceGatheringAsync(page, $"turn:127.0.0.1:{port}?transport=tcp"),
        };

    private static Task StartIceGatheringAsync(IPage page, string urls) =>
        page.EvaluateFunctionAsync(
            @"async (urls) => {
                const connection = new RTCPeerConnection({
                    iceServers: [{urls, username: 'user', credential: 'password'}],
                });
                connection.createDataChannel('test');
                await connection.setLocalDescription(await connection.createOffer());
                window.connection = connection;
            }",
            urls
        );

    private static Task<bool> ConnectWebRtcPeersAsync(IPage page, int timeout) =>
        page.EvaluateFunctionAsync<bool>(
            @"async (timeout) => {
                const local = new RTCPeerConnection();
                const remote = new RTCPeerConnection();
                local.onicecandidate = event => {
                    if (event.candidate) {
                        void remote.addIceCandidate(event.candidate);
                    }
                };
                remote.onicecandidate = event => {
                    if (event.candidate) {
                        void local.addIceCandidate(event.candidate);
                    }
                };
                const channel = local.createDataChannel('test');
                const opened = new Promise(resolve => {
                    channel.onopen = () => {
                        resolve(true);
                    };
                    setTimeout(() => {
                        resolve(false);
                    }, timeout);
                });
                await local.setLocalDescription(await local.createOffer());
                await remote.setRemoteDescription(local.localDescription);
                await remote.setLocalDescription(await remote.createAnswer());
                await local.setRemoteDescription(remote.localDescription);
                try {
                    return await opened;
                } finally {
                    local.close();
                    remote.close();
                }
            }",
            timeout
        );

    private static async Task ShouldNotSendToBlockedHostAsync(Traffic traffic)
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.BlockList = ["*://127.0.0.1:*/*", "stun:*", "turn:*"];
        await AssertNoTrafficAsync(options, traffic);
    }

    private static async Task ShouldNotSendToNotAllowedHostAsync(Traffic traffic)
    {
        var options = TestConstants.DefaultBrowserOptions();
        options.Allowlist = ["*://localhost:*/*"];
        await AssertNoTrafficAsync(options, traffic);
    }

    private static async Task AssertNoTrafficAsync(LaunchOptions options, Traffic traffic)
    {
        await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");

        Assert.That(
            await SendsTrafficAsync(page, traffic, NoTrafficTimeout),
            Is.False,
            TrafficNames[(int)traffic]
        );
    }

    private static async Task ShouldSendTrafficAsync(Traffic traffic)
    {
        await using var browser = await Puppeteer.LaunchAsync(
            TestConstants.DefaultBrowserOptions(),
            TestConstants.LoggerFactory
        );
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");

        Assert.That(await SendsTrafficAsync(page, traffic), Is.True, TrafficNames[(int)traffic]);
    }
}
