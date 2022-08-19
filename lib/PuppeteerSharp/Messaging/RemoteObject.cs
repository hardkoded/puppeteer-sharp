using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    /// <summary>
    /// Remote object is a mirror object referencing original JavaScript object.
    /// </summary>
    public class RemoteObject
    {
        /// <summary>
        /// Gets or sets String representation of the object. (Optional)
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the Object class (constructor) name. Specified for object type values only. (Optional)
        /// </summary>
        public string ClassName { get; set; }
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
