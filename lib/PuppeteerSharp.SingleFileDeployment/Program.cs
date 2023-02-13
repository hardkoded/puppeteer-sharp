using PuppeteerSharp;

Console.WriteLine($"AppDomain.BaseDirectory: {AppContext.BaseDirectory}");
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync("google.jpg");
