
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// The Accessibility class provides methods for inspecting Chromium's accessibility tree. 
    /// The accessibility tree is used by assistive technology such as screen readers.
    /// 
    /// Accessibility is a very platform-specific thing. On different platforms, there are different screen readers that might have wildly different output.
    /// Blink - Chrome's rendering engine - has a concept of "accessibility tree", which is than translated into different platform-specific APIs. 
    /// Accessibility namespace gives users access to the Blink Accessibility Tree.
    /// Most of the accessibility tree gets filtered out when converting from Blink AX Tree to Platform-specific AX-Tree or by screen readers themselves. 
    /// By default, Puppeteer tries to approximate this filtering, exposing only the "interesting" nodes of the tree.
    /// </summary>
    public class Accessibility
    {
        private readonly CDPSession _client;

        public Accessibility(CDPSession client)
        {
            _client = client;
        }

        public async Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
        {

        }
    }
}