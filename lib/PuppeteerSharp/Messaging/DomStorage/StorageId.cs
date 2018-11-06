using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging.DomStorage
{
	/// <summary>
	/// DOM Storage identifier.
	/// </summary>
	public class StorageId
	{
		/// <summary>
		/// Gets or sets Security origin for the storage.
		/// </summary>
		[JsonProperty("securityOrigin")]
		public string SecurityOrigin { get; set; }
        /// <summary>
        /// Gets or sets Whether the storage is local storage (not session storage).
        /// </summary>
        [JsonProperty("isLocalStorage")]
        public bool IsLocalStorage { get; set; }
	}
}
