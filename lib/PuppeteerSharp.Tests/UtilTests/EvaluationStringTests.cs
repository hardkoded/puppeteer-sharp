using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.UtilTests
{
    public class EvaluationStringTests : PuppeteerPageBaseTest
    {
        private const string Fn = "(arg) => arg";

        [
            Test,
            PuppeteerTest(
                "util.test",
                "util evaluationString",
                "serializes JSON compatible arguments"
            )
        ]
        public void SerializesJsonCompatibleArguments()
        {
            Assert.That(
                BindingUtils.EvaluationString(Fn, "a", 1, true, null, new[] { 1, 2 }),
                Is.EqualTo($"({Fn})(\"a\",1,true,undefined,[1,2])")
            );
            Assert.That(
                BindingUtils.EvaluationString(Fn, 0.1f, new[] { 0.1f }),
                Is.EqualTo($"({Fn})(0.1,[0.1])")
            );
        }

        [
            Test,
            PuppeteerTest(
                "util.test",
                "util evaluationString",
                "keeps undefined arguments in place"
            )
        ]
        public async Task KeepsUndefinedArgumentsInPlace()
        {
            Assert.That(await RoundTripAsync(null, "arg === undefined"), Is.True);
            Assert.That(
                BindingUtils.EvaluationString(Fn, null, 1),
                Is.EqualTo($"({Fn})(undefined,1)")
            );
        }

        [
            Test,
            PuppeteerTest(
                "util.test",
                "util evaluationString",
                "preserves NaN, Infinity and -Infinity"
            )
        ]
        public async Task PreservesNaNInfinityAndNegativeInfinity()
        {
            Assert.That(await RoundTripAsync(double.NaN, "Number.isNaN(arg)"), Is.True);
            Assert.That(await RoundTripAsync(double.PositiveInfinity, "arg === Infinity"), Is.True);
            Assert.That(
                await RoundTripAsync(double.NegativeInfinity, "arg === -Infinity"),
                Is.True
            );
        }

        [Test, PuppeteerTest("util.test", "util evaluationString", "preserves negative zero")]
        public async Task PreservesNegativeZero()
        {
            Assert.That(await RoundTripAsync(-0.0, "Object.is(arg, -0)"), Is.True);
            Assert.That(await RoundTripAsync(0.0, "Object.is(arg, 0)"), Is.True);
        }

        [Test, PuppeteerTest("util.test", "util evaluationString", "preserves bigint arguments")]
        public async Task PreservesBigintArguments()
        {
            Assert.That(await RoundTripAsync(new BigInteger(10), "arg === 10n"), Is.True);
            Assert.That(await RoundTripAsync(new BigInteger(-5), "arg === -5n"), Is.True);
            Assert.That(await RoundTripAsync(BigInteger.Pow(2, 70), "arg === 2n ** 70n"), Is.True);
        }

        [
            Test,
            PuppeteerTest(
                "util.test",
                "util evaluationString",
                "keeps the position of every argument"
            )
        ]
        public void KeepsThePositionOfEveryArgument()
        {
            Assert.That(
                BindingUtils.EvaluationString(Fn, double.NaN, 1, -0.0, new BigInteger(2)),
                Is.EqualTo($"({Fn})(NaN,1,-0,2n)")
            );
        }

        private Task<bool> RoundTripAsync(object value, string check) =>
            Page.EvaluateExpressionAsync<bool>(
                $"(() => {{ const arg = {BindingUtils.EvaluationString("(arg) => arg", value)}; return {check}; }})()"
            );
    }
}
