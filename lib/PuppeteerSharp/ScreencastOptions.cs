namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.ScreencastAsync(ScreencastOptions)"/>.
    /// </summary>
    public class ScreencastOptions
    {
        /// <summary>
        /// File path to save the screencast to.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Specifies whether to overwrite the output file, or exit immediately if it already exists.
        /// </summary>
        public bool? Overwrite { get; set; }

        /// <summary>
        /// Specifies the output video format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Specifies the region of the viewport to crop.
        /// </summary>
        public BoundingBox Crop { get; set; }

        /// <summary>
        /// Scales the output video.
        /// For example, 0.5 will shrink the width and height of the output video by half.
        /// 2 will double the width and height of the output video.
        /// </summary>
        public decimal? Scale { get; set; }

        /// <summary>
        /// Specifies the speed to record at.
        /// For example, 0.5 will slowdown the output video by 50%.
        /// 2 will double the speed of the output video.
        /// </summary>
        public decimal? Speed { get; set; }

        /// <summary>
        /// Specifies the frame rate in frames per second.
        /// </summary>
        public int? Fps { get; set; }

        /// <summary>
        /// Specifies the number of times to loop playback.
        /// </summary>
        public double? Loop { get; set; }

        /// <summary>
        /// Specifies the delay between iterations of a loop, in ms.
        /// </summary>
        public int? Delay { get; set; }

        /// <summary>
        /// Specifies the recording quality (Constant Rate Factor between 0-63).
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// Specifies the maximum number of palette colors to quantize.
        /// </summary>
        public int? Colors { get; set; }

        /// <summary>
        /// Path to the ffmpeg executable.
        /// Required if ffmpeg is not in your PATH.
        /// </summary>
        public string FfmpegPath { get; set; }
    }
}
