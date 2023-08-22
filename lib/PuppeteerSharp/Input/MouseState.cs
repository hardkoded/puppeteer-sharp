using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    internal class MouseState
    {
        public Point Position { get; set; }

        public MouseButton Button { get; set; } = MouseButton.None;
    }
}
