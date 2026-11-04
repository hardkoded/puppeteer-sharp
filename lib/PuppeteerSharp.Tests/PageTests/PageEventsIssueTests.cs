using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests;

public class PageEventsIssueTests : PuppeteerPageBaseTest
{
    [Test, PuppeteerTest("page.spec", "Page Page.Events.Issue", "should emit issue event when CSP violation occurs")]
    public async Task ShouldEmitIssueEventWhenCspViolationOccurs()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");

        var issueTask = new TaskCompletionSource<Issue>();
        Page.Issue += (_, e) => issueTask.TrySetResult(e.Issue);

        await Page.AddScriptTagAsync(new AddTagOptions { Content = "console.log(\"CSP test\")" });

        var issue = await issueTask.Task;
        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.Code, Is.EqualTo("ContentSecurityPolicyIssue"));
    }

    [Test, PuppeteerTest("page.spec", "Page Page.Events.Issue", "should emit issue event from cross-origin iframe")]
    public async Task ShouldEmitIssueEventFromCrossOriginIframe()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);

        var cspIssueTask = new TaskCompletionSource<Issue>();
        Page.Issue += (_, e) =>
        {
            if (e.Issue.Code == "ContentSecurityPolicyIssue")
            {
                cspIssueTask.TrySetResult(e.Issue);
            }
        };

        var crossOriginUrl = TestConstants.CrossProcessUrl + "/csp.html";
        await Page.SetContentAsync($"<iframe src=\"{crossOriginUrl}\"></iframe>");

        var frame = await Page.WaitForFrameAsync(crossOriginUrl);
        Assert.That(frame, Is.Not.Null);

        await frame.AddScriptTagAsync(new AddTagOptions { Content = "console.log(\"CSP test in iframe\")" });

        var issue = await cspIssueTask.Task;
        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.Code, Is.EqualTo("ContentSecurityPolicyIssue"));
    }
}

public class PageEventsIssueDisabledTests : PuppeteerPageBaseTest
{
    public PageEventsIssueDisabledTests()
    {
        DefaultOptions = TestConstants.DefaultBrowserOptions();
        DefaultOptions.IssuesEnabled = false;
    }

    [Test, PuppeteerTest("page.spec", "Page Page.Events.Issue when issues are disabled", "should be able to connect and disable issues")]
    public async Task ShouldBeAbleToConnectAndDisableIssues()
    {
        var issueEmitted = false;
        Page.Issue += (_, _) => issueEmitted = true;

        await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");

        Assert.That(issueEmitted, Is.False);
    }
}
