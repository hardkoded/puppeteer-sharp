using System.Diagnostics;

namespace PuppeteerSharp.Messaging.DomStorage
{
    /// <summary>
    /// DomStorageItem
    /// </summary>
    [DebuggerDisplay("{Key} : {Value}")]
	public class DomStorageItem
    {
        /// <summary>
        /// constrctor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
	    public DomStorageItem(string key, string value)
        {
	        Key = key;
	        Value = value;
        }
	    /// <summary>
        /// Key
        /// </summary>
	    public string Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
	    public string Value { get; set; }
    }
}
