using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Media;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ScreenshotTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextScreenshotTests : DevToolsContextBaseTest
    {
        public DevToolsContextScreenshotTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.png");
            var fileInfo = new FileInfo(outputFile);

            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            await DevToolsContext.ScreenshotAsync(outputFile);

            fileInfo = new FileInfo(outputFile);
            Assert.True(new FileInfo(outputFile).Length > 0);
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", outputFile));

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));
        }

        [PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should clip rect")]
        [PuppeteerFact]
        public async Task ShouldClipRect()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 50,
                    Y = 100,
                    Width = 150,
                    Height = 100
                }
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-clip-rect.png", screenshot));
        }

        [PuppeteerFact]
        public async Task ShouldClipScale()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 50,
                    Y = 100,
                    Width = 150,
                    Height = 100,
                    Scale = 2
                }
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-clip-rect-scale.png", screenshot));
        }

        [PuppeteerFact]
        public async Task ShouldClipElementsToTheViewport()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 50,
                    Y = 600,
                    Width = 100,
                    Height = 100
                }
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-offscreen-clip.png", screenshot));
        }

        //[PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should run in parallel")]
        //[PuppeteerFact]
        //public async Task ShouldRunInParallel()
        //{
        //    await using (var page = await Context.NewPageAsync())
        //    {
        //        await DevToolsContext.SetViewportAsync(new ViewPortOptions
        //        {
        //            Width = 500,
        //            Height = 500
        //        });
        //        await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");

        //        var tasks = new List<Task<byte[]>>();
        //        for (var i = 0; i < 3; ++i)
        //        {
        //            tasks.Add(page.ScreenshotDataAsync(new ScreenshotOptions
        //            {
        //                Clip = new Clip
        //                {
        //                    X = 50 * i,
        //                    Y = 0,
        //                    Width = 50,
        //                    Height = 50
        //                }
        //            }));
        //        }

        //        await Task.WhenAll(tasks);
        //        Assert.True(ScreenshotHelper.PixelMatch("grid-cell-1.png", tasks[0].Result));
        //    }
        //}

        [PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should take fullPage screenshots")]
        [PuppeteerFact]
        public async Task ShouldTakeFullPageScreenshots()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                FullPage = true
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-grid-fullpage.png", screenshot));
        }

        //[PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should run in parallel in multiple pages")]
        //[PuppeteerFact]
        //public async Task ShouldRunInParallelInMultiplePages()
        //{
        //    var n = 2;
        //    var pageTasks = new List<Task<Page>>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        async Task<Page> func()
        //        {
        //            var page = await Context.NewPageAsync();
        //            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
        //            return page;
        //        }

        //        pageTasks.Add(func());
        //    }

        //    await Task.WhenAll(pageTasks);

        //    var screenshotTasks = new List<Task<byte[]>>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        screenshotTasks.Add(pageTasks[i].Result.ScreenshotDataAsync(new ScreenshotOptions
        //        {
        //            Clip = new Clip
        //            {
        //                X = 50 * i,
        //                Y = 0,
        //                Width = 50,
        //                Height = 50
        //            }
        //        }));
        //    }

        //    await Task.WhenAll(screenshotTasks);

        //    for (var i = 0; i < n; i++)
        //    {
        //        Assert.True(ScreenshotHelper.PixelMatch($"grid-cell-{i}.png", screenshotTasks[i].Result));
        //    }

        //    var closeTasks = new List<Task>();
        //    for (var i = 0; i < n; i++)
        //    {
        //        closeTasks.Add(pageTasks[i].Result.CloseAsync());
        //    }

        //    await Task.WhenAll(closeTasks);
        //}

        [PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should allow transparency")]
        [PuppeteerFact]
        public async Task ShouldAllowTransparency()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 100,
                Height = 100
            });
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                OmitBackground = true
            });

            Assert.True(ScreenshotHelper.PixelMatch("transparent.png", screenshot));
        }
        [PuppeteerTest("screenshot.spec.ts", "Page.screenshot", "should return base64")]
        [PuppeteerFact]
        public async Task ShouldReturnBase64()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotBase64Async();

            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", Convert.FromBase64String(screenshot)));
        }

        [PuppeteerFact]
        public void ShouldInferScreenshotTypeFromName()
        {
            Assert.Equal(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpg"));
            Assert.Equal(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpe"));
            Assert.Equal(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpeg"));
            Assert.Equal(ScreenshotType.Png, ScreenshotOptions.GetScreenshotTypeFromFile("Test.png"));
            Assert.Null(ScreenshotOptions.GetScreenshotTypeFromFile("Test.exe"));
        }

        [PuppeteerFact]
        public async Task ShouldWorkWithQuality()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await DevToolsContext.ScreenshotDataAsync(new ScreenshotOptions
            {
                Type = ScreenshotType.Jpeg,
                FullPage = true,
                Quality = 100
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-grid-fullpage.png", screenshot));
        }
    }
}
