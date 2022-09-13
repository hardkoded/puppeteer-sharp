using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Dialog type.
    /// </summary>
    /// <seealso cref="Dialog"/>
    public enum DialogType
    {
        /// <summary>
        /// Alert dialog.
        /// </summary>
        Alert,

        /// <summary>
        /// Prompt dialog.
        /// </summary>
        Prompt,

        /// <summary>
        /// Confirm dialog.
        /// </summary>
        Confirm,

        /// <summary>
        /// Before unload dialog.
        /// </summary>
        [EnumMember(Value = "beforeunload")]
        BeforeUnload,
    }
}
