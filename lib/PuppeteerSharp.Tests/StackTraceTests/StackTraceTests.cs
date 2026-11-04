using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.StackTraceTests
{
    public class StackTraceTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("stacktrace.spec", "Stack trace", "should work")]
        public void ShouldWork()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () =>
            {
                await Page.EvaluateFunctionAsync(@"() => {
                    throw new Error('Test');
                }");
            });

            Assert.That(exception.Message, Does.Contain("Test"));
        }

        [Test, PuppeteerTest("stacktrace.spec", "Stack trace", "should work with handles")]
        public void ShouldWorkWithHandles()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () =>
            {
                await Page.EvaluateFunctionHandleAsync(@"() => {
                    throw new Error('Test');
                }");
            });

            Assert.That(exception.Message, Does.Contain("Test"));
        }

        [Test, PuppeteerTest("stacktrace.spec", "Stack trace", "should work with contiguous evaluation")]
        public async Task ShouldWorkWithContiguousEvaluation()
        {
            await using var thrower = await Page.EvaluateFunctionHandleAsync(@"() => {
                return () => {
                    throw new Error('Test');
                };
            }");

            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () =>
            {
                await thrower.EvaluateFunctionAsync(@"thrower => {
                    thrower();
                }");
            });

            Assert.That(exception.Message, Does.Contain("Test"));
        }

        [Test, PuppeteerTest("stacktrace.spec", "Stack trace", "should work with nested function calls")]
        public void ShouldWorkWithNestedFunctionCalls()
        {
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () =>
            {
                await Page.EvaluateFunctionAsync(@"() => {
                    function a() {
                        throw new Error('Test');
                    }
                    function b() {
                        a();
                    }
                    function c() {
                        b();
                    }
                    function d() {
                        c();
                    }
                    d();
                }");
            });

            Assert.That(exception.Message, Does.Contain("Test"));
        }

        [Test, PuppeteerTest("stacktrace.spec", "Stack trace", "should work for none error objects")]
        public async Task ShouldWorkForNonErrorObjects()
        {
            var errorTask = new TaskCompletionSource<PageErrorEventArgs>();

            void ErrorHandler(object sender, PageErrorEventArgs e)
            {
                errorTask.TrySetResult(e);
                Page.PageError -= ErrorHandler;
            }

            Page.PageError += ErrorHandler;

            await Task.WhenAll(
                errorTask.Task,
                Page.EvaluateFunctionAsync(@"() => {
                    // This can happen when a 404 with HTML is returned
                    void Promise.reject(new Response());
                }")
            );

            var error = await errorTask.Task;
            Assert.That(error, Is.Not.Null);
        }
    }
}
