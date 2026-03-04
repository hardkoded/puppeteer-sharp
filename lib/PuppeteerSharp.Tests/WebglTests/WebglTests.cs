using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WebglTests
{
    public class WebglTests : PuppeteerPageBaseTest
    {
        public WebglTests() : base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.Args =
            [
                .. DefaultOptions.Args ?? [],
                // Current flags that enable software rendering.
                "--disable-gpu",
                "--enable-features=AllowSwiftShaderFallback,AllowSoftwareGLFallbackDueToCrashes",
                "--enable-unsafe-swiftshader",
            ];
        }

        [Test, PuppeteerTest("webgl.spec", "webgl Create webgl context", "should work")]
        public async Task ShouldWork()
        {
            await Page.EvaluateExpressionAsync(@"(() => {
                const canvas = document.createElement('canvas');
                const gl = canvas.getContext('webgl');
                if (!gl) {
                    throw new Error('WebGL context not created');
                }
            })()");
        }
    }
}
