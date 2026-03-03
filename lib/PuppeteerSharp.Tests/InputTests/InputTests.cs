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

        [Test, PuppeteerTest("input.spec", "input tests ElementHandle.uploadFile", "should upload the file")]
        public async Task ShouldUploadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;
            var input = await Page.QuerySelectorAsync("input");
            await input.EvaluateFunctionAsync(@"e => {
                window._inputEvents = [];
                e.addEventListener('change', ev => window._inputEvents.push(ev.type));
                e.addEventListener('input', ev => window._inputEvents.push(ev.type));
            }");
            await input.UploadFileAsync(filePath);
            Assert.That(
                await input.EvaluateFunctionAsync<string>("e => e.files[0].name"),
                Is.EqualTo("file-to-upload.txt"));
            Assert.That(
                await input.EvaluateFunctionAsync<string>("e => e.files[0].type"),
                Is.EqualTo("text/plain"));
            Assert.That(
                await Page.EvaluateFunctionAsync<string[]>("() => window._inputEvents"),
                Is.EqualTo(new[] { "input", "change" }));
        }

        [Test, PuppeteerTest("input.spec", "input tests ElementHandle.uploadFile", "should read the file")]
        public async Task ShouldReadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;
            var input = await Page.QuerySelectorAsync("input");
            await input.EvaluateFunctionAsync(@"e => {
                window._inputEvents = [];
                e.addEventListener('change', ev => window._inputEvents.push(ev.type));
                e.addEventListener('input', ev => window._inputEvents.push(ev.type));
            }");
            await input.UploadFileAsync(filePath);
            Assert.That(
                await input.EvaluateFunctionAsync<string>(@"e => {
                    const file = e.files[0];
                    if (!file) {
                        throw new Error('No file found');
                    }
                    const reader = new FileReader();
                    const promise = new Promise(fulfill => {
                        reader.addEventListener('load', fulfill);
                    });
                    reader.readAsText(file);
                    return promise.then(() => reader.result);
                }"),
                Is.EqualTo("contents of the file"));
        }
    }
}
