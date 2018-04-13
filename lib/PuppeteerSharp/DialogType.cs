using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    public enum DialogType
    {
        Alert,
        Prompt,
        Confirm,
        
        [EnumMember(Value = "beforeunload")]
        BeforeUnload,
    }
}