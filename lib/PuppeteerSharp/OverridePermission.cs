using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    /// <summary>
    /// Override permission.
    /// </summary>
    /// <seealso cref="IBrowserContext.OverridePermissionsAsync(string, System.Collections.Generic.IEnumerable{OverridePermission})"/>
    public enum OverridePermission
    {
        /// <summary>
        /// Geolocation.
        /// </summary>
        [EnumMember(Value = "geolocation")]
        Geolocation,

        /// <summary>
        /// MIDI.
        /// </summary>
        [EnumMember(Value = "midi")]
        Midi,

        /// <summary>
        /// Notifications.
        /// </summary>
        [EnumMember(Value = "notifications")]
        Notifications,

        /// <summary>
        /// Camera.
        /// </summary>
        [EnumMember(Value = "videoCapture")]
        Camera,

        /// <summary>
        /// Microphone.
        /// </summary>
        [EnumMember(Value = "audioCapture")]
        Microphone,

        /// <summary>
        /// Background sync.
        /// </summary>
        [EnumMember(Value = "backgroundSync")]
        BackgroundSync,

        /// <summary>
        /// Ambient light sensor, Accelerometer, Gyroscope, Magnetometer.
        /// </summary>
        [EnumMember(Value = "sensors")]
        Sensors,

        /// <summary>
        /// Accessibility events.
        /// </summary>
        [EnumMember(Value = "accessibilityEvents")]
        AccessibilityEvents,

        /// <summary>
        /// Clipboard read/write.
        /// </summary>
        [EnumMember(Value = "clipboardReadWrite")]
        ClipboardReadWrite,

        /// <summary>
        /// Payment handler.
        /// </summary>
        [EnumMember(Value = "paymentHandler")]
        PaymentHandler,

        /// <summary>
        /// MIDI sysex.
        /// </summary>
        [EnumMember(Value = "midiSysex")]
        MidiSysex,

        /// <summary>
        /// Idle detection.
        /// </summary>
        [EnumMember(Value = "idleDetection")]
        IdleDetection,

        /// <summary>
        /// Persistent Storage.
        /// </summary>
        [EnumMember(Value = "durableStorage")]
        PersistentStorage,
    }
}
