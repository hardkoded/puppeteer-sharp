using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// Clip data.
    /// </summary>
    /// <seealso cref="BoundingBox.ToClip"/>
    /// <seealso cref="ScreenshotOptions.Clip"/>
    public class Clip
    {
        /// <summary>
        /// x-coordinate of top-left corner of clip area.
        /// </summary>
        /// <value>The x.</value>
        [JsonProperty("x")]
        public decimal X { get; set; }
        /// <summary>
        /// y-coordinate of top-left corner of clip area.
        /// </summary>
        /// <value>The y.</value>
        [JsonProperty("y")]
        public decimal Y { get; set; }
        /// <summary>
        /// Width of clipping area.
        /// </summary>
        /// <value>The width.</value>
        [JsonProperty("width")]
        public decimal Width { get; set; }
        /// <summary>
        /// Height of clipping area.
        /// </summary>
        /// <value>The height.</value>
        [JsonProperty("height")]
        public decimal Height { get; set; }
        /// <summary>
        /// Scale of the webpage rendering. Defaults to 1.
        /// </summary>
        /// <value>The scale.</value>
        [JsonProperty("scale")]
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