using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    /// <summary>
    /// Remote object.
    /// </summary>
    public class RemoteObject
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RemoteObjectType Type { get; set; }
        /// <summary>
        /// Gets or sets the subtype.
        /// </summary>
        public RemoteObjectSubtype Subtype { get; set; }
        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// Gets or sets the unserializable value.
        /// </summary>
        public string UnserializableValue { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public JToken Value { get; set; }
    }
}
