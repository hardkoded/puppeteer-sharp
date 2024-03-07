using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Media;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ScreenshotTests
{
    public class PageScreenshotTests : PuppeteerBrowserContextBaseTest
    {
        public async Task ShouldWorkWithFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.png");
            var fileInfo = new FileInfo(outputFile);

            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2)]
        public async Task Usage()
        {
            var outputFile = Path.Combine(BaseDirectory, "Usage.png");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            #region ScreenshotAsync
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com");
            await page.ScreenshotAsync(outputFile);
            #endregion
            Assert.True(File.Exists(outputFile));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should work")]
        public async Task ShouldWork()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should clip rect")]
        public async Task ShouldClipRect()
        {
            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should use scale for clip")]
        public async Task ShouldUseScaleForClip()
        {
            await using var page = await Context.NewPageAsync();
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
                    Height = 100,
                    Scale = 2,
                }
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-clip-rect-scale2.png", screenshot));
        }

        public async Task ShouldClipScale()
        {
            await using var page = await Context.NewPageAsync();
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
                    Height = 100,
                    Scale = 2
                }
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-clip-rect-scale.png", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should get screenshot bigger than the viewport")]
        public async Task ShouldClipElementsToTheViewport()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should run in parallel")]
        public async Task ShouldRunInParallel()
        {
            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should take fullPage screenshots")]
        public async Task ShouldTakeFullPageScreenshots()
        {
            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should run in parallel in multiple pages")]
        public async Task ShouldRunInParallelInMultiplePages()
        {
            const int n = 2;
            var pageTasks = new List<Task<IPage>>();
            for (var i = 0; i < n; i++)
            {
                pageTasks.Add(Func());
                continue;

                async Task<IPage> Func()
                {
                    var page = await Context.NewPageAsync();
                    await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
                    return page;
                }
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Cdp", "should allow transparency")]
        public async Task ShouldAllowTransparency()
        {
            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Cdp", "should render white background on jpeg file")]
        public async Task ShouldRenderWhiteBackgroundOnJpegFile()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            await page.GoToAsync(TestConstants.EmptyPage);
            var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                OmitBackground = true,
                Type = ScreenshotType.Jpeg
            });
            Assert.True(ScreenshotHelper.PixelMatch("white.jpg", screenshot));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Cdp", "should work with webp")]
        public async Task ShouldWorkWithWebp()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            await page.GoToAsync(TestConstants.EmptyPage);
            var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                Type = ScreenshotType.Webp
            });
            Assert.Greater(screenshot.Length, 0);
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should work with odd clip size on Retina displays")]
        public async Task ShouldWorkWithOddClipSizeOnRetinaDisplays()
        {
            await using var page = await Context.NewPageAsync();
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

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should return base64")]
        public async Task ShouldReturnBase64()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotBase64Async();

            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", Convert.FromBase64String(screenshot)));
        }

        [Test, Retry(2), PuppeteerTest("screenshot.spec", "Screenshots Cdp", "should work in \"fromSurface: false\" mode")]
        public async Task ShouldWorkInFromSurfacedFalseMode()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotBase64Async(new ScreenshotOptions
            {
                FromSurface = false,
            });

            Assert.Greater(screenshot.Length, 0);
        }

        public void ShouldInferScreenshotTypeFromName()
        {
            Assert.AreEqual(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpg"));
            Assert.AreEqual(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpe"));
            Assert.AreEqual(ScreenshotType.Jpeg, ScreenshotOptions.GetScreenshotTypeFromFile("Test.jpeg"));
            Assert.AreEqual(ScreenshotType.Png, ScreenshotOptions.GetScreenshotTypeFromFile("Test.png"));
            Assert.Null(ScreenshotOptions.GetScreenshotTypeFromFile("Test.exe"));
        }

        public async Task ShouldWorkWithQuality()
        {
            await using var page = await Context.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var screenshot = await page.ScreenshotDataAsync(new ScreenshotOptions
            {
                Type = ScreenshotType.Jpeg,
                FullPage = true,
                Quality = 100
            });
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-grid-fullpage.png", screenshot));
        }
    }
}
