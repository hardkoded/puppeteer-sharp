using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PuppeteerSharp.Tests
{
    public class ScreenshotHelper
    {
        public static bool PixelMatch(string screenShotFile, string fileName)
            => PixelMatch(screenShotFile, File.ReadAllBytes(fileName));

        public static bool PixelMatch(string screenShotFile, byte[] screenshot)
        {
            const int pixelThreshold = 10;
            const decimal totalTolerance = 0.05m;

            var baseImage = Image.Load<Rgb24>(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), TestConstants.IsChrome ? "golden-chromium" : "golden-firefox", screenShotFile));
            var compareImage = Image.Load<Rgb24>(screenshot);

            //Just  for debugging purpose
            compareImage.Save(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), TestConstants.IsChrome ? "golden-chromium" : "golden-firefox", "test.png"));

            if (baseImage.Width != compareImage.Width || baseImage.Height != compareImage.Height)
            {
                return false;
            }

            var invalidPixelsCount = 0;

            for (var y = 0; y < baseImage.Height; y++)
            {
                for (var x = 0; x < baseImage.Width; x++)
                {
                    var rgb1 = baseImage[x, y];
                    var rgb2 = compareImage[x, y];

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
