using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp.Tests.FrameManagerTests;

public class FrameManagerTests
{
    private const string MainFrameId = "main";
    private const string OopifFrameId = "oopif";
    private const string SiblingFrameId = "sibling";

    private Scope _scope;

    private Connection _connection
    {
        get => _scope?.Connection;
        set => (_scope ??= new Scope()).Connection = value;
    }

    private FrameManager _frameManager
    {
        get => _scope?.FrameManager;
        set => (_scope ??= new Scope()).FrameManager = value;
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_scope != null)
        {
            await _scope.DisposeAsync();
            _scope = null;
        }
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager Page.frameAttached for a known frame",
            "should keep the main frame on the session of the page"
        )
    ]
    public async Task ShouldKeepTheMainFrameOnTheSessionOfThePage()
    {
        var setup = await SetupAsync();

        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = MainFrameId, parentFrameId = OopifFrameId }
        );
        EmitDefaultContext(setup.OopifSession, MainFrameId);
        await TickAsync();

        Assert.That(MainFrame.Client, Is.SameAs(setup.PageSession));
        Assert.That(HasContext(MainFrame), Is.False);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager Page.frameAttached for a known frame",
            "should keep a frame on the session of its parent frame"
        )
    ]
    public async Task ShouldKeepAFrameOnTheSessionOfItsParentFrame()
    {
        var setup = await SetupAsync();

        // The event has a different parent frame.
        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = SiblingFrameId, parentFrameId = OopifFrameId }
        );

        // The event has the same parent frame but a different session.
        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = SiblingFrameId, parentFrameId = MainFrameId }
        );
        EmitDefaultContext(setup.OopifSession, SiblingFrameId);
        await TickAsync();

        var frame = Frame(SiblingFrameId);
        Assert.That(frame.Client, Is.SameAs(setup.PageSession));
        Assert.That(HasContext(frame), Is.False);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager Page.frameAttached for a known frame",
            "should move an OOP frame to the session of its parent frame when its target is gone"
        )
    ]
    public async Task ShouldMoveAnOOPFrameToTheSessionOfItsParentFrameWhenItsTargetIsGone()
    {
        var setup = await SetupAsync();

        setup.Detach(setup.OopifTarget);
        setup.PageSession.Emit(
            "Page.frameAttached",
            new { frameId = OopifFrameId, parentFrameId = MainFrameId }
        );
        EmitDefaultContext(setup.PageSession, OopifFrameId);
        await TickAsync();

        var frame = Frame(OopifFrameId);
        Assert.That(frame.Client, Is.SameAs(setup.PageSession));
        Assert.That(HasContext(frame), Is.True);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager Page.frameAttached for a known frame",
            "should keep an OOP frame on its session while its target is attached"
        )
    ]
    public async Task ShouldKeepAnOOPFrameOnItsSessionWhileItsTargetIsAttached()
    {
        var setup = await SetupAsync();

        setup.PageSession.Emit(
            "Page.frameAttached",
            new { frameId = OopifFrameId, parentFrameId = MainFrameId }
        );
        EmitDefaultContext(setup.PageSession, OopifFrameId);
        await TickAsync();

        var frame = Frame(OopifFrameId);
        Assert.That(frame.Client, Is.SameAs(setup.OopifSession));
        Assert.That(HasContext(frame), Is.False);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager Page.frameAttached for a known frame",
            "should keep a nested OOP frame on its session while its target is attached"
        )
    ]
    public async Task ShouldKeepANestedOOPFrameOnItsSessionWhileItsTargetIsAttached()
    {
        var setup = await SetupAsync();
        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = "nested", parentFrameId = OopifFrameId }
        );
        await TickAsync();
        var nestedSession = CreateIframeSession("nested", OopifFrameId);
        var nestedTarget = CreateIframeTarget(nestedSession, "nested", OopifFrameId);
        setup.Attach(nestedTarget);
        await TickAsync();
        var frame = Frame("nested");
        Assert.That(frame.Client, Is.SameAs(nestedSession));

        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = "nested", parentFrameId = OopifFrameId }
        );
        EmitDefaultContext(setup.OopifSession, "nested");
        await TickAsync();

        Assert.That(frame.Client, Is.SameAs(nestedSession));
        Assert.That(HasContext(frame), Is.False);

        setup.Detach(nestedTarget);
        EmitDefaultContext(setup.OopifSession, "nested");
        await TickAsync();

        Assert.That(frame.Client, Is.SameAs(setup.OopifSession));
        Assert.That(HasContext(frame), Is.True);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager frame events",
            "should only handle the events of a frame on its session"
        )
    ]
    public async Task ShouldOnlyHandleTheEventsOfAFrameOnItsSession()
    {
        var setup = await SetupAsync();
        var frame = Frame(SiblingFrameId);
        var url = frame.Url;
        var lifecycleEvents = 0;
        _frameManager.LifecycleEvent += (_, _) => lifecycleEvents++;

        setup.OopifSession.Emit(
            "Page.frameNavigated",
            new
            {
                frame = new
                {
                    id = SiblingFrameId,
                    parentId = MainFrameId,
                    loaderId = "other-loader",
                    url = "https://other.test/",
                },
                type = "Navigation",
            }
        );
        setup.OopifSession.Emit(
            "Page.navigatedWithinDocument",
            new { frameId = SiblingFrameId, url = "https://other.test/#hash" }
        );
        setup.OopifSession.Emit(
            "Page.lifecycleEvent",
            new
            {
                frameId = SiblingFrameId,
                loaderId = "other-loader",
                name = "load",
                timestamp = 0,
            }
        );
        setup.OopifSession.Emit("Page.frameStoppedLoading", new { frameId = SiblingFrameId });
        setup.OopifSession.Emit(
            "Page.frameDetached",
            new { frameId = SiblingFrameId, reason = "remove" }
        );
        await TickAsync();

        Assert.That(Frame(SiblingFrameId), Is.SameAs(frame));
        Assert.That(frame.Url, Is.EqualTo(url));
        Assert.That(frame.Detached, Is.False);
        Assert.That(lifecycleEvents, Is.EqualTo(0));
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager frame events",
            "should handle the detachment of a frame on the session of an ancestor"
        )
    ]
    public async Task ShouldHandleTheDetachmentOfAFrameOnTheSessionOfAnAncestor()
    {
        var setup = await SetupAsync();

        setup.PageSession.Emit(
            "Page.frameDetached",
            new { frameId = OopifFrameId, reason = "remove" }
        );
        await TickAsync();

        Assert.That(Frame(OopifFrameId), Is.Null);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager frame events",
            "should only add child frames on the session of the parent frame"
        )
    ]
    public async Task ShouldOnlyAddChildFramesOnTheSessionOfTheParentFrame()
    {
        var setup = await SetupAsync();

        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = "child-of-main", parentFrameId = MainFrameId }
        );
        setup.OopifSession.Emit(
            "Page.frameAttached",
            new { frameId = "child-of-oopif", parentFrameId = OopifFrameId }
        );
        await TickAsync();

        Assert.That(Frame("child-of-main"), Is.Null);
        Assert.That(Frame("child-of-oopif").Client, Is.SameAs(setup.OopifSession));
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager frame events",
            "should only add a main frame on the session of the page"
        )
    ]
    public async Task ShouldOnlyAddAMainFrameOnTheSessionOfThePage()
    {
        var setup = await SetupAsync();

        setup.OopifSession.Emit(
            "Page.frameNavigated",
            new
            {
                frame = new
                {
                    id = "other-main",
                    loaderId = "other-loader",
                    url = "https://other.test/",
                },
                type = "Navigation",
            }
        );
        await TickAsync();

        Assert.That(Frame("other-main"), Is.Null);
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager iframe targets",
            "should ignore a closed target if the frame changes its session during a call"
        )
    ]
    public async Task ShouldIgnoreAClosedTargetIfTheFrameChangesItsSessionDuringACall()
    {
        var setup = await SetupAsync();
        const string method = "Page.addScriptToEvaluateOnNewDocument";
        setup.PageSession.Responses[method] = new { identifier = "1" };
        var closeTarget = new TaskCompletionSource<object>();
        setup.OopifSession.Responses[method] = closeTarget;

        var result = _frameManager.EvaluateOnNewDocumentAsync("1");
        await TickAsync();
        setup.Detach(setup.OopifTarget);
        Assert.That(Frame(OopifFrameId).Client, Is.SameAs(setup.PageSession));
        closeTarget.SetException(new TargetClosedException("Target closed"));

        Assert.That((await result).Identifier, Is.EqualTo("1"));
    }

    [
        Test,
        PuppeteerTest(
            "FrameManager.test.ts",
            "FrameManager iframe targets",
            "should use the parent frame from the target info"
        )
    ]
    public async Task ShouldUseTheParentFrameFromTheTargetInfo()
    {
        var setup = await SetupAsync();

        // The frame tree of the target has a different parent frame.
        var session = CreateIframeSession("late", OopifFrameId);

        setup.Attach(CreateIframeTarget(session, "late", MainFrameId));
        await TickAsync();

        var frame = Frame("late");
        Assert.That(frame.Client, Is.SameAs(session));
        Assert.That(frame.ParentFrame, Is.SameAs(MainFrame));
    }

    // Upstream waits for a setImmediate. Events are queued here, so wait until the queue is idle.
    private static Task TickAsync() => Task.Delay(200);

    private static void EmitDefaultContext(MockCdpSession session, string frameId) =>
        session.Emit(
            "Runtime.executionContextCreated",
            new
            {
                context = new
                {
                    id = 1,
                    origin = "https://example.test",
                    name = string.Empty,
                    uniqueId = "1",
                    auxData = new { frameId, isDefault = true },
                },
            }
        );

    private static bool HasContext(Frame frame) => ((IsolatedWorld)frame.MainRealm).HasContext;

    private CdpFrame MainFrame => (CdpFrame)_frameManager.MainFrame;

    private CdpFrame Frame(string frameId) => _frameManager.FrameTree.GetById(frameId);

    private MockCdpSession CreateSession(string id) => new(_connection, id);

    private MockCdpSession CreateIframeSession(string frameId, string parentFrameId)
    {
        var session = CreateSession(frameId);
        session.Responses["Page.getFrameTree"] = new
        {
            frameTree = new
            {
                frame = new
                {
                    id = frameId,
                    parentId = parentFrameId,
                    loaderId = $"{frameId}-loader",
                    url = $"https://{frameId}.test/",
                },
            },
        };
        return session;
    }

    private CdpTarget CreateIframeTarget(
        MockCdpSession session,
        string frameId,
        string parentFrameId
    ) =>
        new(
            new TargetInfo
            {
                Type = TargetType.IFrame,
                TargetId = frameId,
                ParentFrameId = parentFrameId,
            },
            session,
            null,
            null,
            null,
            null
        );

    /// <summary>
    /// Creates a frame manager with a main frame, a same-process child frame and
    /// an out-of-process child frame that has its own session.
    /// </summary>
    private async Task<FrameManagerSetup> SetupAsync()
    {
        _connection = Connection.CreateFromTransport(
            Substitute.For<IConnectionTransport>(),
            new ConnectOptions()
        );

        // The iframe targets that the target manager reports as attached.
        var targets = new ConcurrentDictionary<string, CdpTarget>();
        var targetManager = Substitute.For<ITargetManager>();
        targetManager
            .GetFrameTarget(Arg.Any<string>())
            .Returns(call => targets.GetValueOrDefault(call.Arg<string>()));
        var pageSession = CreateSession("page");
        _ = new CdpTarget(
            new TargetInfo { Type = TargetType.Page, TargetId = MainFrameId },
            pageSession,
            null,
            targetManager,
            null,
            null
        );
        var oopifSession = CreateIframeSession(OopifFrameId, MainFrameId);
        var page = Substitute.For<Page>((TaskQueue)null);
        _frameManager = new FrameManager(pageSession, page, new TimeoutSettings(), false, false);

        void Attach(CdpTarget target)
        {
            targets[target.TargetId] = target;
            _frameManager.OnAttachedToTarget(target);
        }

        void Detach(CdpTarget target)
        {
            targets.TryRemove(target.TargetId, out _);
            _frameManager.OnDetachedFromTarget(target);
        }

        pageSession.Emit(
            "Page.frameNavigated",
            new
            {
                frame = new
                {
                    id = MainFrameId,
                    loaderId = "main-loader",
                    url = "https://main.test/",
                },
                type = "Navigation",
            }
        );
        await TickAsync();
        foreach (var frameId in new[] { OopifFrameId, SiblingFrameId })
        {
            pageSession.Emit("Page.frameAttached", new { frameId, parentFrameId = MainFrameId });
        }

        await TickAsync();
        var oopifTarget = CreateIframeTarget(oopifSession, OopifFrameId, MainFrameId);
        Attach(oopifTarget);
        await TickAsync();

        Assert.That(MainFrame.Client, Is.SameAs(pageSession));
        Assert.That(Frame(SiblingFrameId).Client, Is.SameAs(pageSession));
        Assert.That(Frame(OopifFrameId).Client, Is.SameAs(oopifSession));

        return new FrameManagerSetup(pageSession, oopifSession, oopifTarget, Attach, Detach);
    }

    private sealed record FrameManagerSetup(
        MockCdpSession PageSession,
        MockCdpSession OopifSession,
        CdpTarget OopifTarget,
        Action<CdpTarget> Attach,
        Action<CdpTarget> Detach
    );

    private sealed class MockCdpSession(Connection connection, string sessionId)
        : CdpCDPSession(connection, TargetType.Page, sessionId, null)
    {
        public ConcurrentDictionary<string, object> Responses { get; } = new();

        public override bool Detached => false;

        public override async Task<JsonElement?> SendAsync(
            string method,
            object args = null,
            bool waitForCallback = true,
            CommandOptions options = null
        )
        {
            Responses.TryGetValue(method, out var response);
            if (response is TaskCompletionSource<object> pending)
            {
                response = await pending.Task;
            }

            return JsonSerializer.SerializeToElement(
                response ?? new { },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
        }

        public override Task DetachAsync() => Task.CompletedTask;

        public void Emit(string method, object data) =>
            OnMessageReceived(
                new MessageEventArgs
                {
                    MessageID = method,
                    MessageData = JsonSerializer.SerializeToElement(data),
                }
            );
    }

    // Holds the disposable pieces of a test, so they are disposed in TearDown.
    private sealed class Scope : IAsyncDisposable
    {
        public Connection Connection { get; set; }

        public FrameManager FrameManager { get; set; }

        public async ValueTask DisposeAsync()
        {
            if (FrameManager != null)
            {
                await FrameManager.DisposeAsync();
            }

            Connection?.Dispose();
        }
    }
}
