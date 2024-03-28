namespace PuppeteerSharp.Cdp.Messaging
{
    internal class EmulationSetIdleOverrideRequest
    {
        public bool IsUserActive { get; set; }

        public bool IsScreenUnlocked { get; set; }
    }
}
