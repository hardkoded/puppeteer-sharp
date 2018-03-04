using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ScreenshotTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWorkWithFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.png");
            var fileInfo = new FileInfo(outputFile);
            var page = await Browser.NewPageAsync();

            await page.SetViewport(new ViewPortOptions
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
            Assert.True(PixelMatch("screenshot-sanity.png", outputFile));

            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [Fact]
        public async Task ShouldWork()
        {
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotStreamAsync();
            Assert.True(PixelMatch("screenshot-sanity.png", screenshot));
        }

        [Fact]
        public async Task ShouldClipRect()
        {
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 50,
                    Y = 100,
                    Width = 150,
                    Height = 100
                }
            });
            Assert.True(PixelMatch("screenshot-clip-rect.png", screenshot));
        }

        [Fact]
        public async Task ShouldWorkForOffscreenClip()
        {
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 50,
                    Y = 600,
                    Width = 100,
                    Height = 100
                }
            });
            Assert.True(PixelMatch("screenshot-offscreen-clip.png", screenshot));
        }

        [Fact]
        public async Task ShouldRunInParallel()
        {
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var tasks = new List<Task<Stream>>();
            for (var i = 0; i < 3; ++i)
            {
                tasks.Add(page.ScreenshotStreamAsync(new ScreenshotOptions
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
            Assert.True(PixelMatch("grid-cell-1.png", tasks[0].Result));
        }

        [Fact]
        public async Task ShouldTakeFullPageScreenshots()
        {
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions
            {
                FullPage = true
            });
            Assert.True(PixelMatch("screenshot-grid-fullpage.png", screenshot));
        }

        [Fact]
        public async Task ShouldRunInParallelInMultiplePages()
        {
            var n = 2;
            var pageTasks = new List<Task<PuppeteerSharp.Page>>();
            for (var i = 0; i < n; i++)
            {
                Func<Task<PuppeteerSharp.Page>> func = async () =>
                {
                    var page = await Browser.NewPageAsync();
                    await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                    return page;
                };

                pageTasks.Add(func());
            }

            await Task.WhenAll(pageTasks);

            var screenshotTasks = new List<Task<Stream>>();
            for (var i = 0; i < n; i++)
            {
                screenshotTasks.Add(pageTasks[i].Result.ScreenshotStreamAsync(new ScreenshotOptions
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
                Assert.True(PixelMatch($"grid-cell-{i}.png", screenshotTasks[i].Result));
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
            var page = await Browser.NewPageAsync();
            await page.SetViewport(new ViewPortOptions
            {
                Width = 100,
                Height = 100
            });
            await page.GoToAsync(TestConstants.EmptyPage);
            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions
            {
                OmitBackground = true
            });

            Assert.True(PixelMatch("transparent.png", screenshot));
        }

        [Fact]
        public async Task ShouldWorkWithOddClipSizeOnRetinaDisplays()
        {
            var page = await Browser.NewPageAsync();
            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions
            {
                Clip = new Clip
                {
                    X = 0,
                    Y = 0,
                    Width = 11,
                    Height = 11
                }
            });

            Assert.True(PixelMatch("screenshot-clip-odd-size.png", screenshot));
        }

        private bool PixelMatch(string screenShotFile, string fileName)
        {
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                return PixelMatch(screenShotFile, stream);
            }
        }

        private bool PixelMatch(string screenShotFile, Stream screenshot)
        {
            const int pixelThreshold = 10;
            const decimal totalTolerance = 0.05m;

            var baseImage = Image.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Screenshots", screenShotFile));
            var compareImage = Image.Load(screenshot);

            //Just  for debugging purpose
            compareImage.Save(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Screenshots", "test.png"));

            if (baseImage.Width != compareImage.Width || baseImage.Height != compareImage.Height)
            {
                return false;
            }

            var rgb1 = default(Rgb24);
            var rgb2 = default(Rgb24);
            var invalidPixelsCount = 0;

            for (int y = 0; y < baseImage.Height; y++)
            {
                for (int x = 0; x < baseImage.Width; x++)
                {
                    var pixelA = baseImage[x, y];
                    var pixelB = compareImage[x, y];

                    pixelA.ToRgb24(ref rgb1);
                    pixelB.ToRgb24(ref rgb2);

                    if (Math.Abs(rgb1.R - rgb2.R) > pixelThreshold ||
                        Math.Abs(rgb1.G - rgb2.G) > pixelThreshold ||
                        Math.Abs(rgb1.B - rgb2.B) > pixelThreshold)
                    {
                        invalidPixelsCount++;
                    }
                }
            }

            return (invalidPixelsCount / (baseImage.Height * baseImage.Width)) < totalTolerance;
        }
    }
}
