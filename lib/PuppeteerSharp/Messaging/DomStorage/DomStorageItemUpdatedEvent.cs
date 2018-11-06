using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging.DomStorage
{
    /// <summary>
    /// domStorageItemUpdated
    /// </summary>
	public class DomStorageItemUpdatedEvent
	{
		/// <summary>
		/// Gets or sets StorageId
		/// </summary>
		[JsonProperty("storageId")]
		public StorageId StorageId { get; set; }
		/// <summary>
		/// Gets or sets Key
		/// </summary>
		[JsonProperty("key")]
		public string Key { get; set; }
		/// <summary>
		/// Gets or sets OldValue
		/// </summary>
		[JsonProperty("oldValue")]
		public string OldValue { get; set; }
		/// <summary>
		/// Gets or sets NewValue
		/// </summary>
		[JsonProperty("newValue")]
		public string NewValue { get; set; }
	}
}
