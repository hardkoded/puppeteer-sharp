using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InputTests
{
    public class InputTests : PuppeteerPageBaseTest
    {
        public InputTests() : base()
        {
        }

        [Test, PuppeteerTest("input.spec", "Input", "should upload the file")]
        public async Task ShouldUploadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;
            var input = await Page.QuerySelectorAsync("input");
            await input.UploadFileAsync(filePath);
            Assert.That(await Page.EvaluateFunctionAsync<string>("e => e.files[0].name", input), Is.EqualTo("file-to-upload.txt"));
            Assert.That(await Page.EvaluateFunctionAsync<string>(@"e => {
                const reader = new FileReader();
                const promise = new Promise(fulfill => reader.onload = fulfill);
                reader.readAsText(e.files[0]);
                return promise.then(() => reader.result);
            }", input), Is.EqualTo("contents of the file"));
        }
    }
}
