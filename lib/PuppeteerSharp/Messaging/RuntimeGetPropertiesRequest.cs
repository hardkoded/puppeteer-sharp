namespace CefSharp.DevTools.Dom.Messaging
{
    internal class RuntimeGetPropertiesRequest
    {
        public bool OwnProperties { get; set; }

        public string ObjectId { get; set; }
    }
}
