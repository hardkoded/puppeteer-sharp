using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class NavigationWatcher
    {
        private FrameManager _frameManager;
        private Frame _mainFrame;
        private Dictionary<string, string> _options;

        public NavigationWatcher(FrameManager frameManager, Frame mainFrame, Dictionary<string, string> options)
        {
            _frameManager = frameManager;
            _mainFrame = mainFrame;
            _options = options;
        }

        public Task NavigationTask { get; internal set; }
    }
}