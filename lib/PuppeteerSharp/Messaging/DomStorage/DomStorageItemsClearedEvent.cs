using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging.DomStorage
{
    /// <summary>
    /// domStorageItemsCleared
    /// </summary>
	public class DomStorageItemsClearedEvent
	{
		/// <summary>
		/// Gets or sets StorageId
		/// </summary>
		[JsonProperty("storageId")]
		public StorageId StorageId { get; set; }
	}
}
