using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging.DomStorage
{
    /// <summary>
    /// 
    /// </summary>
	public class DomStorageItemRemovedEvent
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
	}
}
