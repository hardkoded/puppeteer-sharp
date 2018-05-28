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

            var baseImage = Image.Load(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), screenShotFile));
            var compareImage = Image.Load(screenshot);

            //Just  for debugging purpose
            compareImage.Save(Path.Combine(TestUtils.FindParentDirectory("Screenshots"), "test.png"));

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
