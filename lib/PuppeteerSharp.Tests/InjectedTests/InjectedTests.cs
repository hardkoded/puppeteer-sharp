using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InjectedTests
{
    public class InjectedTests : PuppeteerPageBaseTest
    {
        public InjectedTests() : base()
        {
        }

        [Test, PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should work")]
        public async Task ShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await world.EvaluateFunctionAsync<bool>(@"
                      PuppeteerUtil => {
                        return typeof PuppeteerUtil === 'object';
                      }",
                      new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)));
            Assert.That(result, Is.True);
        }

        [TestCase("mutation")]
        [TestCase("raf")]
        [TestCase("interval")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not start a stopped mutation poller")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not start a stopped raf poller")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not start a stopped interval poller")]
        public async Task ShouldNotStartAStoppedPoller(string kind)
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var calls = await world.EvaluateFunctionAsync<int>(@"
                async ({MutationPoller, RAFPoller, IntervalPoller}, kind) => {
                    let calls = 0;
                    const predicate = async () => {
                        ++calls;
                        return false;
                    };
                    const createPoller = () => {
                        switch (kind) {
                            case 'mutation':
                                return new MutationPoller(predicate, document);
                            case 'raf':
                                return new RAFPoller(predicate);
                            case 'interval':
                                return new IntervalPoller(predicate, 1);
                        }
                    };
                    const poller = createPoller();
                    await poller.stop();
                    await poller.start();
                    return calls;
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                kind);
            Assert.That(calls, Is.EqualTo(0));
        }

        [TestCase("mutation")]
        [TestCase("raf")]
        [TestCase("interval")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not restart a stopped mutation poller after its first predicate await")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not restart a stopped raf poller after its first predicate await")]
        [PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should not restart a stopped interval poller after its first predicate await")]
        public async Task ShouldNotRestartAStoppedPollerAfterItsFirstPredicateAwait(string kind)
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var calls = await world.EvaluateFunctionAsync<int>(@"
                async ({MutationPoller, RAFPoller, IntervalPoller}, kind) => {
                    let release;
                    const pending = new Promise(resolve => { release = resolve; });
                    let calls = 0;
                    const predicate = async () => {
                        ++calls;
                        return await pending;
                    };
                    const createPoller = () => {
                        switch (kind) {
                            case 'mutation':
                                return new MutationPoller(predicate, document);
                            case 'raf':
                                return new RAFPoller(predicate);
                            case 'interval':
                                return new IntervalPoller(predicate, 1);
                        }
                    };
                    const poller = createPoller();
                    const starting = poller.start();
                    const result = poller.result().catch(() => {});
                    await poller.stop();
                    release(false);
                    await starting;
                    await result;
                    document.body.append(document.createElement('div'));
                    await new Promise(resolve => setTimeout(resolve, 50));
                    await poller.stop();
                    return calls;
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                kind);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test, PuppeteerTest("injected.spec", "createFunction tests", "should work")]
        public async Task CreateFunctionShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await (Page.MainFrame as Frame)
                .IsolatedRealm.EvaluateFunctionAsync<int>(@"({createFunction}, fnString) => {
                    return createFunction(fnString)(4);
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                "() => 4");
            Assert.That(result, Is.EqualTo(4));
        }
    }
}
