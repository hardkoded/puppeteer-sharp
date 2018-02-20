using System;

namespace PuppeteerSharp
{
    public class Clip
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Scale { get; internal set; }

        internal Clip Clone()
        {
            return new Clip
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height
            };
        }
    }
}