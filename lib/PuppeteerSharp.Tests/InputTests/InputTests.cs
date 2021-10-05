using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Input;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InputTests : PuppeteerPageBaseTest
    {
        private const string Dimensions = @"function dimensions() {
            const rect = document.querySelector('textarea').getBoundingClientRect();
            return {
                x: rect.left,
                y: rect.top,
                width: rect.width,
                height: rect.height
            };
        }";

        public InputTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("input.spec.ts", "Input", "should upload the file")]
        [Fact]
        public async Task ShouldUploadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;
            var input = await Page.QuerySelectorAsync("input");
            await input.UploadFileAsync(filePath);
            Assert.Equal("file-to-upload.txt", await Page.EvaluateFunctionAsync<string>("e => e.files[0].name", input));
            Assert.Equal("contents of the file", await Page.EvaluateFunctionAsync<string>(@"e => {
                const reader = new FileReader();
                const promise = new Promise(fulfill => reader.onload = fulfill);
                reader.readAsText(e.files[0]);
                return promise.then(() => reader.result);
            }", input));
        }
    }
}
