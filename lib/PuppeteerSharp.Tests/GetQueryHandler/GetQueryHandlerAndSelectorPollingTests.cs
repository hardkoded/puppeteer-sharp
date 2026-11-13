using NUnit.Framework;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp.Tests.GetQueryHandler
{
    public class GetQueryHandlerAndSelectorPollingTests
    {
        [Test]
        public void UsesMutationPollingForACssSelector()
        {
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("div > span").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Mutation));
        }

        [Test]
        public void UsesRafPollingForACssSelectorWithAPseudoClass()
        {
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("input:focus").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Raf));
        }

        [Test]
        public void UsesMutationPollingForAPSelector()
        {
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("div >>> ::-p-text(a)").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Mutation));
        }

        [Test]
        public void UsesRafPollingForAPSelectorWithAPseudoClass()
        {
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("div:focus >>> span").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Raf));
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("input:focus ::-p-text(a)").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Raf));
        }

        [Test]
        public void UsesRafPollingForAPSelectorWithAnAriaQuery()
        {
            Assert.That(
                CustomQuerySelectorRegistry.Default.GetQueryHandlerAndSelector("div >>> ::-p-aria(a)").Polling,
                Is.EqualTo(WaitForFunctionPollingOption.Raf));
        }
    }
}
