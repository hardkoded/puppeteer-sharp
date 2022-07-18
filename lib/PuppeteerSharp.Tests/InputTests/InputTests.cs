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
        public InputTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("input.spec.ts", "Input", "should upload the file")]
        [Fact]
        public async Task ShouldUploadTheFile()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;

            Assert.True(System.IO.File.Exists(filePath));

            var input = await Page.QuerySelectorAsync<HtmlInputElement>("input");
            await input.UploadFileAsync(filePath);

            var fileList = await input.GetFilesAsync();

            Assert.Equal(1, await fileList.GetLengthAsync());

            var files = await fileList.ToArrayAsync();

            var fileName = await files[0].GetNameAsync();

            Assert.Equal("file-to-upload.txt", fileName);

            var fileContents = await Page.EvaluateFunctionAsync<string>(@"e => {
                const reader = new FileReader();
                const promise = new Promise(fulfill => reader.onload = fulfill);
                reader.readAsText(e.files[0]);
                return promise.then(() => reader.result);
            }", (JSHandle)input);

            Assert.Equal("contents of the file", fileContents);
        }
    }
}
