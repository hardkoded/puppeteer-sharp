using System;
namespace PuppeteerSharp
{
    public class TargetInfo
    {
        public TargetInfo()
        {
        }

        public TargetInfo(dynamic targetInfo)
        {
            Type = targetInfo.type;
            Url = targetInfo.url;
            TargetId = targetInfo.targetId;
            SourceObject = targetInfo;
        }

        public string Type { get; internal set; }
        public string Url { get; internal set; }
        public string TargetId { get; internal set; }
        public dynamic SourceObject { get; private set; }
    }
}
