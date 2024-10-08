using System.Text.Json.Serialization;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Media
{
    /// <summary>
    /// margin options used in <see cref="PdfOptions"/>.
    /// </summary>
    public record MarginOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PuppeteerSharp.Media.MarginOptions"/> class.
        /// </summary>
        public MarginOptions()
        {
        }

        /// <summary>
        /// Top margin, accepts values labeled with units.
        /// </summary>
        [JsonConverter(typeof(PrimitiveTypeConverter))]
        public object Top { get; set; }

        /// <summary>
        /// Left margin, accepts values labeled with units.
        /// </summary>
        [JsonConverter(typeof(PrimitiveTypeConverter))]
        public object Left { get; set; }

        /// <summary>
        /// Bottom margin, accepts values labeled with units.
        /// </summary>
        [JsonConverter(typeof(PrimitiveTypeConverter))]
        public object Bottom { get; set; }

        /// <summary>
        /// Right margin, accepts values labeled with units.
        /// </summary>
        [JsonConverter(typeof(PrimitiveTypeConverter))]
        public object Right { get; set; }
    }
}
