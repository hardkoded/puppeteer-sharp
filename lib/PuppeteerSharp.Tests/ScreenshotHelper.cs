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

            var expectedImage = Image.Load<Rgb24>(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), "golden-chromium", screenShotFile));
            var actualImage = Image.Load<Rgb24>(screenshot);

#if DEBUG
            //Just  for debugging purpose
            actualImage.Save(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), "golden-chromium", "test.png"));
#endif

            if (expectedImage.Width != actualImage.Width || expectedImage.Height != actualImage.Height)
            {
                return false;
            }

            var invalidPixelsCount = 0;

            for (var y = 0; y < expectedImage.Height; y++)
            {
                for (var x = 0; x < expectedImage.Width; x++)
                {
                    var rgb1 = expectedImage[x, y];
                    var rgb2 = actualImage[x, y];

                    if (Math.Abs(rgb1.R - rgb2.R) > pixelThreshold ||
                        Math.Abs(rgb1.G - rgb2.G) > pixelThreshold ||
                        Math.Abs(rgb1.B - rgb2.B) > pixelThreshold)
                    {
                        invalidPixelsCount++;
                    }
                }
            }

            return (invalidPixelsCount / (expectedImage.Height * expectedImage.Width)) < totalTolerance;
        }
    }
}
