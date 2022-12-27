namespace PuppeteerSharp
{
    /// <summary>
    /// Options for <see cref="IPage.EmulateIdleStateAsync(EmulateIdleOverrides)"/>.
    /// </summary>
    public class EmulateIdleOverrides
    {
        /// <summary>
        /// Whether the user is active or not.
        /// </summary>
        public bool IsUserActive { get; set; }

        /// <summary>
        /// Whether the screen is unlocked or not.
        /// </summary>
        public bool IsScreenUnlocked { get; set; }
    }
}
