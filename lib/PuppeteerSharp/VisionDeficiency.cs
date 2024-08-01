using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Types of vision deficiency to emulate using <see cref="IPage.EmulateVisionDeficiencyAsync(VisionDeficiency)"/>.
    /// </summary>
    public enum VisionDeficiency
    {
        /// <summary>
        /// None.
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Achromatopsia.
        /// </summary>
        [EnumMember(Value = "achromatopsia")]
        Achromatopsia,

        /// <summary>
        /// BlurredVision.
        /// </summary>
        [EnumMember(Value = "blurredVision")]
        BlurredVision,

        /// <summary>
        /// Deuteranopia.
        /// </summary>
        [EnumMember(Value = "deuteranopia")]
        Deuteranopia,

        /// <summary>
        /// Protanopia.
        /// </summary>
        [EnumMember(Value = "protanopia")]
        Protanopia,

        /// <summary>
        /// Tritanopia.
        /// </summary>
        [EnumMember(Value = "tritanopia")]
        Tritanopia,
    }
}
