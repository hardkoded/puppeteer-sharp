using System;

namespace PuppeteerSharp.Input
{
    internal class MouseTransaction
    {
        public Action<TransactionData> Update { get; set; }

        public Action Commit { get; set; }

        public Action Rollback { get; set; }

        public class TransactionData
        {
            public bool IsCommitted { get; set; }

            public Point? Position { get; set; }

            // Buttons are tracked as changes, so a transaction never carries the buttons of another one.
            public MouseButton PressedButtons { get; set; }

            public MouseButton ReleasedButtons { get; set; }
        }
    }
}
