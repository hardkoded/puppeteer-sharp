namespace PuppeteerSharp
{
    /// <summary>
    /// Override permission.
    /// </summary>
    /// <seealso cref="BrowserContext.OverridePermissionsAsync(string, System.Collections.Generic.IEnumerable{OverridePermission})"/>
    public enum OverridePermission
    {
        /// <summary>
        /// Geolocation.
        /// </summary>
        Geolocation,
        /// <summary>
        /// MIDI.
        /// </summary>
        Midi,
        /// <summary>
        /// Notifications.
        /// </summary>
        Notifications,
        /// <summary>
        /// Push.
        /// </summary>
        Push,
        /// <summary>
        /// Camera.
        /// </summary>
        Camera,
        /// <summary>
        /// Microphone.
        /// </summary>
        Microphone,
        /// <summary>
        /// Background sync.
        /// </summary>
        BackgroundSync,
        /// <summary>
        /// Ambient light sensor.
        /// </summary>
        AmbientLightSensor,
        /// <summary>
        /// Accelerometer.
        /// </summary>
        Accelerometer,
        /// <summary>
        /// Gyroscope.
        /// </summary>
        Gyroscope,
        /// <summary>
        /// Magnetometer.
        /// </summary>
        Magnetometer,
        /// <summary>
        /// Accessibility events.
        /// </summary>
        AccessibilityEvents,
        /// <summary>
        /// Clipboard read.
        /// </summary>
        ClipboardRead,
        /// <summary>
        /// Clipboard write.
        /// </summary>
        ClipboardWrite,
        /// <summary>
        /// Payment handler.
        /// </summary>
        PaymentHandler,
        /// <summary>
        /// MIDI sysex.
        /// </summary>
        MidiSysex
    }
}