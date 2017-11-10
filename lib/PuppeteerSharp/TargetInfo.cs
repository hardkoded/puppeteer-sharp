using System;
namespace PuppeteerSharp
{
    public class TargetInfo
    {
        public TargetInfo()
        {
        }

        public string Type { get; internal set; }
        public string Url { get; internal set; }
        public int targetId { get; internal set; }
    }
}
