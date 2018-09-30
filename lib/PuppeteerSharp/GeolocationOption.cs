using Newtonsoft.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Geolocation option.
    /// </summary>
    /// <seealso cref="Page.SetGeolocationAsync(GeolocationOption)"/>
    public class GeolocationOption
    {
        /// <summary>
        /// Latitude between -90 and 90.
        /// </summary>
        /// <value>The latitude.</value>
        public decimal Latitude { get; set; }
        /// <summary>
        /// Longitude between -180 and 180.
        /// </summary>
        /// <value>The longitude.</value>
        public decimal Longitude { get; set; }
        /// <summary>
        /// Optional non-negative accuracy value.
        /// </summary>
        /// <value>The accuracy.</value>
        public decimal Accuracy { get; set; }
    }
}