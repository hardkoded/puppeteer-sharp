using System;

namespace PuppeteerSharp.Input
{
    internal class MouseTransaction
    {
        public Action<MouseState> Update { get; set; }

        public Action Commit { get; set; }

        public Action Rollback { get; set; }
    }
}
