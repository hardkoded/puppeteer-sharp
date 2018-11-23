using Newtonsoft.Json;

namespace PuppeteerSharp
{
    /// <summary>
    /// Cookie data.
    /// </summary>
    /// <seealso cref="Page.SetContentAsync(string)"/>
    /// <seealso cref="Page.DeleteCookieAsync(CookieParam[])"/>
    /// <seealso cref="Page.GetCookiesAsync(string[])"/>
    public class CookieParam
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }
        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>Expiration.</value>
        public double? Expires { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int? Size { get; set; }
        /// <summary>
        /// Gets or sets if it's HTTP only.
        /// </summary>
        /// <value>Whether it's http only or not.</value>
        public bool? HttpOnly { get; set; }
        /// <summary>
        /// Gets or sets if it's secure.
        /// </summary>
        /// <value>Whether it's secure or not.</value>
        public bool? Secure { get; set; }
        /// <summary>
        /// Gets or sets if it's session only.
        /// </summary>
        /// <value>Whether it's session only or not.</value>
        public bool? Session { get; set; }
    }
}