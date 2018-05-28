using System;
using Newtonsoft.Json;
using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="Page.ScreenshotAsync(string, ScreenshotOptions)"/>, <see cref="Page.ScreenshotStreamAsync(ScreenshotOptions)"/> and <see cref="Page.ScreenshotDataAsync(ScreenshotOptions)"/>
    /// </summary>
    public class ScreenshotOptions
    {
        /// <summary>
        /// Specifies clipping region of the page.
        /// </summary>
        /// <value>The clip.</value>
        [JsonProperty("clip")]
        public Clip Clip { get; set; }
        /// <summary>
        /// When <c>true</c>, takes a screenshot of the full scrollable page. Defaults to <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if full page; otherwise, <c>false</c>.</value>
        [JsonProperty("fullPage")]
        public bool FullPage { get; set; }
        /// <summary>
        /// Hides default white background and allows capturing screenshots with transparency. Defaults to <c>false</c>
        /// </summary>
        /// <value><c>true</c> if omit background; otherwise, <c>false</c>.</value>
        [JsonProperty("omitBackground")]
        public bool OmitBackground { get; set; }
        /// <summary>
        /// Specify screenshot type, can be either jpeg or png. Defaults to 'png'.
        /// </summary>
        /// <value>The type.</value>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// The quality of the image, between 0-100. Not applicable to png images.
        /// </summary>
        /// <value>The quality.</value>
        [JsonProperty("quality")]
        public decimal? Quality { get; set; }
    }
}