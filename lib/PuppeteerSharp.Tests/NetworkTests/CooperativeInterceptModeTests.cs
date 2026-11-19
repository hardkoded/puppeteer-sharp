using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests;

[TestFixture]
public class CooperativeInterceptModeTests
{
    [
        Test,
        PuppeteerTest(
            "HTTPRequest.test.ts",
            "HTTPRequest Cooperative Intercept Mode",
            "should continue with the overrides of the highest priority"
        )
    ]
    public async Task ShouldContinueWithTheOverridesOfTheHighestPriority()
    {
        var request = CreateRequest();
        await request.ContinueAsync(
            new Payload { Headers = new Dictionary<string, string> { ["foo"] = "high" } },
            1
        );
        await request.ContinueAsync(
            new Payload { Headers = new Dictionary<string, string> { ["foo"] = "low" } },
            0
        );

        await request.FinalizeInterceptionsAsync();

        Assert.That(request.Resolution.Action, Is.EqualTo(InterceptResolutionAction.Continue));
        Assert.That(request.Resolution.Overrides.Headers["foo"], Is.EqualTo("high"));
    }

    [
        Test,
        PuppeteerTest(
            "HTTPRequest.test.ts",
            "HTTPRequest Cooperative Intercept Mode",
            "should respond with the response of the highest priority"
        )
    ]
    public async Task ShouldRespondWithTheResponseOfTheHighestPriority()
    {
        var request = CreateRequest();
        await request.RespondAsync(new ResponseData { Body = "high" }, 1);
        await request.RespondAsync(new ResponseData { Body = "low" }, 0);

        await request.FinalizeInterceptionsAsync();

        Assert.That(request.Resolution.Action, Is.EqualTo(InterceptResolutionAction.Respond));
        Assert.That(request.Resolution.Response.Body, Is.EqualTo("high"));
    }

    [
        Test,
        PuppeteerTest(
            "HTTPRequest.test.ts",
            "HTTPRequest Cooperative Intercept Mode",
            "should abort with the error reason of the highest priority"
        )
    ]
    public async Task ShouldAbortWithTheErrorReasonOfTheHighestPriority()
    {
        var request = CreateRequest();
        await request.AbortAsync(RequestAbortErrorCode.BlockedByClient, 1);
        await request.AbortAsync(RequestAbortErrorCode.TimedOut, 0);

        await request.FinalizeInterceptionsAsync();

        Assert.That(request.Resolution.Action, Is.EqualTo(InterceptResolutionAction.Abort));
        Assert.That(
            request.Resolution.ErrorCode,
            Is.EqualTo(RequestAbortErrorCode.BlockedByClient)
        );
    }

    [
        Test,
        PuppeteerTest(
            "HTTPRequest.test.ts",
            "HTTPRequest Cooperative Intercept Mode",
            "should use the latest of the resolutions with the same priority"
        )
    ]
    public async Task ShouldUseTheLatestOfTheResolutionsWithTheSamePriority()
    {
        var request = CreateRequest();
        await request.RespondAsync(new ResponseData { Body = "first" }, 0);
        await request.RespondAsync(new ResponseData { Body = "second" }, 0);

        await request.FinalizeInterceptionsAsync();

        Assert.That(request.Resolution.Action, Is.EqualTo(InterceptResolutionAction.Respond));
        Assert.That(request.Resolution.Response.Body, Is.EqualTo("second"));
    }

    [
        Test,
        PuppeteerTest(
            "HTTPRequest.test.ts",
            "HTTPRequest Cooperative Intercept Mode",
            "should keep the response of a lower priority for a later respond"
        )
    ]
    public async Task ShouldKeepTheResponseOfALowerPriorityForALaterRespond()
    {
        var request = CreateRequest();
        await request.ContinueAsync(new Payload(), 1);
        await request.RespondAsync(new ResponseData { Body = "low" }, 0);
        Assert.That(request.ResponseForRequest.Body, Is.EqualTo("low"));

        await request.RespondAsync(request.ResponseForRequest, 2);

        await request.FinalizeInterceptionsAsync();

        Assert.That(request.Resolution.Action, Is.EqualTo(InterceptResolutionAction.Respond));
        Assert.That(request.Resolution.Response.Body, Is.EqualTo("low"));
    }

    private static TestRequest CreateRequest() => new();

    private sealed class TestRequest : Request<IResponse>
    {
        public override bool IsInterceptResolutionHandled { get; protected set; }

        public override InterceptResolutionState InterceptResolutionState =>
            IsInterceptResolutionHandled
                ? new InterceptResolutionState(InterceptResolutionAction.AlreadyHandled)
                : PendingResolutionState;

        public (
            InterceptResolutionAction Action,
            Payload Overrides,
            ResponseData Response,
            RequestAbortErrorCode ErrorCode
        ) Resolution { get; private set; }

        public override Task<string> FetchPostDataAsync() => Task.FromResult<string>(null);

        public override Task<byte[]> ContentAsync() => Task.FromResult<byte[]>(null);

        internal override Task ContinueInternalAsync(Payload overrides = null)
        {
            Resolution = (InterceptResolutionAction.Continue, overrides, null, default);
            return Task.CompletedTask;
        }

        internal override Task RespondInternalAsync(ResponseData response)
        {
            Resolution = (InterceptResolutionAction.Respond, null, response, default);
            return Task.CompletedTask;
        }

        internal override Task AbortInternalAsync(RequestAbortErrorCode errorCode)
        {
            Resolution = (InterceptResolutionAction.Abort, null, null, errorCode);
            return Task.CompletedTask;
        }

        protected override void VerifyInterception() { }

        protected override bool CanBeIntercepted() => true;
    }
}
