namespace PuppeteerSharp.Messaging
{
    internal class DomSetFileInputFilesRequest
    {
        public string ObjectId { get; set; }

        public string[] Files { get; set; }

        public int BackendNodeId { get; set; }
    }
}
