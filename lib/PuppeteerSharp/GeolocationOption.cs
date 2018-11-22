using System;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Geolocation option.
    /// </summary>
    /// <seealso cref="Page.SetGeolocationAsync(GeolocationOption)"/>
    public class GeolocationOption : IEquatable<GeolocationOption>
    {
        /// <summary>
        /// Latitude between -90 and 90.
        /// </summary>
        /// <value>The latitude.</value>
        public int Latitude { get; set; }
        /// <summary>
        /// Longitude between -180 and 180.
        /// </summary>
        /// <value>The longitude.</value>
        public int Longitude { get; set; }
        /// <summary>
        /// Optional non-negative accuracy value.
        /// </summary>
        /// <value>The accuracy.</value>
        public int Accuracy { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="PuppeteerSharp.GeolocationOption"/> is equal to the current <see cref="T:PuppeteerSharp.GeolocationOption"/>.
        /// </summary>
        /// <param name="other">The <see cref="PuppeteerSharp.GeolocationOption"/> to compare with the current <see cref="T:PuppeteerSharp.GeolocationOption"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="PuppeteerSharp.GeolocationOption"/> is equal to the current
        /// <see cref="T:PuppeteerSharp.GeolocationOption"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GeolocationOption other)
            => other != null &&
                Latitude == other.Latitude &&
                Longitude == other.Longitude &&
                Accuracy == other.Accuracy;
        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as GeolocationOption);
        /// <inheritdoc/>
        public override int GetHashCode()
            => (Latitude.GetHashCode() ^ 2014) +
                (Longitude.GetHashCode() ^ 2014) +
                (Accuracy.GetHashCode() ^ 2014);
    }
}