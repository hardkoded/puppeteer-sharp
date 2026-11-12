using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsExceptionTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should fire")]
        public async Task ShouldFire()
        {
            var task = WaitForExceptionAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/error.html");
            var exception = await task;

            Assert.That(exception.Text, Does.Contain("Fancy error!"));
            Assert.That(exception.Name, Is.EqualTo("Error"));
            Assert.That(exception.StackTrace.Take(3).Select(f => f.FunctionName), Is.EqualTo(new[] { "c", "b", "a" }));
            Assert.That(exception.Location, Is.EqualTo(exception.StackTrace[0]));
            Assert.That(exception.Location.URL, Does.Contain("error.html"));
            Assert.That(exception.Location.LineNumber, Is.EqualTo(13));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should fire together with pageerror")]
        public async Task ShouldFireTogetherWithPageError()
        {
            var errorTcs = new TaskCompletionSource<PageErrorEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.PageError += (_, e) => errorTcs.TrySetResult(e);
            var task = WaitForExceptionAsync();

            await Page.GoToAsync(TestConstants.ServerUrl + "/error.html");

            Assert.That((await errorTcs.Task).Message, Does.Contain("Fancy error!"));
            Assert.That((await task).Text, Does.Contain("Fancy error!"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should fire for primitive values")]
        public async Task ShouldFireForPrimitiveValues()
        {
            var task = WaitForExceptionAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/error-primitive.html");
            var exception = await task;

            Assert.That(exception.Name, Is.EqualTo(string.Empty));
            Assert.That(exception.Handle, Is.Null);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should report the name and text as rendered by the browser")]
        public async Task ShouldReportTheNameAndTextAsRenderedByTheBrowser()
        {
            var cases = new (string Code, string Name, string Text)[]
            {
                ("const error = new Error('x'); error.name = 'AbortError'; throw error;", "AbortError", "AbortError: x"),
                ("throw new DOMException('bad', 'AbortError');", "AbortError", "AbortError: bad"),
                ("class ValidationError extends Error {} throw new ValidationError('v');", "ValidationError", "ValidationError: v"),
                ("throw new Error();", "Error", "Error"),
                ("throw new Error('line1\\nline2');", "Error", "Error: line1\nline2"),
                ("throw {code: 1};", string.Empty, "Object"),
                ("throw Symbol('s');", string.Empty, "Symbol(s)"),
            };

            foreach (var (code, name, text) in cases)
            {
                var task = WaitForExceptionAsync();
                await Page.SetContentAsync($"<script>{code};</script>");
                var exception = await task;
                Assert.That(exception.Name, Is.EqualTo(name), code);
                Assert.That(exception.Text, Is.EqualTo(text), code);
            }
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should fall back to the exception location for syntax errors")]
        public async Task ShouldFallBackToTheExceptionLocationForSyntaxErrors()
        {
            var task = WaitForExceptionAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/syntax-error.html");
            var exception = await task;

            Assert.That(exception.StackTrace, Is.Empty);
            Assert.That(exception.Name, Is.EqualTo("SyntaxError"));
            Assert.That(exception.Text, Is.EqualTo("SyntaxError: Unexpected token '='"));
            Assert.That(exception.Location.URL, Is.EqualTo(TestConstants.ServerUrl + "/syntax-error.js"));
            Assert.That(exception.Location.LineNumber, Is.EqualTo(2));
            Assert.That(exception.Location.ColumnNumber, Is.Not.Null);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should expose the remote exception via a moved handle")]
        public async Task ShouldExposeTheRemoteExceptionViaAMovedHandle()
        {
            var handleTcs = new TaskCompletionSource<IJSHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.ExceptionThrown += (_, e) => handleTcs.TrySetResult(e.Exception.Handle.Move());

            await Page.SetContentAsync(
                "<script>const error = new Error('custom'); error.code = 'E_CUSTOM'; throw error;</script>");

            await using var handle = await handleTcs.Task;
            Assert.That(handle.Disposed, Is.False);
            Assert.That(
                await handle.EvaluateFunctionAsync<string>("error => error.code"),
                Is.EqualTo("E_CUSTOM"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.Exception", "should dispose the handle after dispatch if not moved")]
        public async Task ShouldDisposeTheHandleAfterDispatchIfNotMoved()
        {
            var handleTcs = new TaskCompletionSource<IJSHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.ExceptionThrown += (_, e) => handleTcs.TrySetResult(e.Exception.Handle);

            await Page.SetContentAsync("<script>throw new Error('not moved');</script>");

            var handle = await handleTcs.Task;
            Assert.That(handle, Is.Not.Null);
            Assert.That(handle.Disposed, Is.True);
        }

        private Task<RemoteException> WaitForExceptionAsync(System.Func<RemoteException, bool> predicate = null)
        {
            var tcs = new TaskCompletionSource<RemoteException>(TaskCreationOptions.RunContinuationsAsynchronously);
            void Handler(object sender, RemoteExceptionEventArgs e)
            {
                if (predicate == null || predicate(e.Exception))
                {
                    Page.ExceptionThrown -= Handler;
                    tcs.TrySetResult(e.Exception);
                }
            }

            Page.ExceptionThrown += Handler;
            return tcs.Task;
        }
    }
}
