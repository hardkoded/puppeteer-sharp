using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Media;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ScreenshotTests : PuppeteerBrowserBaseTest
    {
        public ScreenshotTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWorkWithFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.png");
            var fileInfo = new FileInfo(outputFile);

            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });

                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                await page.ScreenshotAsync(outputFile);

                fileInfo = new FileInfo(outputFile);
                Assert.True(new FileInfo(outputFile).Length > 0);
                Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", outputFile));

                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
        }

        [Fact]
        public async Task ShouldWork()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                var screenshot = await page.ScreenshotDataAsync();
                Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));
            }
        }

        [Fact]
        public async Task ShouldClipRect()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
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
        }

        [Fact]
        public async Task ShouldWorkForOffscreenClip()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
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
        }

        [Fact]
        public async Task ShouldRunInParallel()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

                var tasks = new List<Task<byte[]>>();
                for (var i = 0; i < 3; ++i)
                {
                    tasks.Add(page.ScreenshotDataAsync(new ScreenshotOptions
                    {
                        Clip = new Clip
                        {
                            X = 50 * i,
                            Y = 0,
                            Width = 50,
                            Height = 50
                        }
                    }));
                }

                await Task.WhenAll(tasks);
                Assert.True(ScreenshotHelper.PixelMatch("grid-cell-1.png", tasks[0].Result));
            }
        }

        [Fact]
        public async Task ShouldTakeFullPageScreenshots()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 500,
                    Height = 500
                });
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
                {
                    FullPage = true
                });
                Assert.True(ScreenshotHelper.PixelMatch("screenshot-grid-fullpage.png", screenshot));
            }
        }

        [Fact]
        public async Task ShouldRunInParallelInMultiplePages()
        {
            var n = 2;
            var pageTasks = new List<Task<Page>>();
            for (var i = 0; i < n; i++)
            {
                Func<Task<Page>> func = async () =>
                {
                    var page = await Browser.NewPageAsync();
                    await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                    return page;
                };

                pageTasks.Add(func());
            }

            await Task.WhenAll(pageTasks);

            var screenshotTasks = new List<Task<byte[]>>();
            for (var i = 0; i < n; i++)
            {
                screenshotTasks.Add(pageTasks[i].Result.ScreenshotDataAsync(new ScreenshotOptions
                {
                    Clip = new Clip
                    {
                        X = 50 * i,
                        Y = 0,
                        Width = 50,
                        Height = 50
                    }
                }));
            }

            await Task.WhenAll(screenshotTasks);

            for (var i = 0; i < n; i++)
            {
                Assert.True(ScreenshotHelper.PixelMatch($"grid-cell-{i}.png", screenshotTasks[i].Result));
            }

            var closeTasks = new List<Task>();
            for (var i = 0; i < n; i++)
            {
                closeTasks.Add(pageTasks[i].Result.CloseAsync());
            }

            await Task.WhenAll(closeTasks);
        }

        [Fact]
        public async Task ShouldAllowTransparency()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 100,
                    Height = 100
                });
                await page.GoToAsync(TestConstants.EmptyPage);
                var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
                {
                    OmitBackground = true
                });

                Assert.True(ScreenshotHelper.PixelMatch("transparent.png", screenshot));
            }
        }

        [Fact]
        public async Task ShouldWorkWithOddClipSizeOnRetinaDisplays()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
                {
                    Clip = new Clip
                    {
                        X = 0,
                        Y = 0,
                        Width = 11,
                        Height = 11
                    }
                });

                Assert.True(ScreenshotHelper.PixelMatch("screenshot-clip-odd-size.png", screenshot));
            }
        }
    }
}
