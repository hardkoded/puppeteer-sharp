using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp.Input
{
    internal class MouseState
    {
        public Point Position { get; set; }

        public MouseButton Buttons { get; set; }
    }
}
