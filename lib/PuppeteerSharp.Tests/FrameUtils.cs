using System.Text.RegularExpressions;

namespace PuppeteerSharp.Tests
{
    public class FrameUtils
    {
        public static string DumpFrames(Frame frame, string indentation = "")
        {
            var result = indentation + Regex.Replace(frame.Url, ":\\d{4}\\/", ":<PORT>/");
            foreach (var child in frame.ChildFrames)
            {
                result += '\n' + FrameUtils.DumpFrames(child, "    " + indentation);
            }
            return result;
        }
    }
}
