using System.Threading.Tasks;
using PuppeteerSharp.Media;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class EmulationManager
    {
        #region Members
        private readonly CDPSession _client;
        private bool _hasTouch;
        private bool _emulatingMobile;
        #endregion Members


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmulationManager"/> class.
        /// </summary>
        /// <param name="client">The cdp session object.</param>
        public EmulationManager(CDPSession client)
        {
            _client = client;
        }
        #endregion Constructors

        #region Methods
        internal async Task<bool> EmulateViewport(ViewPortOptions viewport)
        {
            var mobile = viewport.IsMobile;
            var width = viewport.Width;
            var height = viewport.Height;
            var deviceScaleFactor = viewport.DeviceScaleFactor;
            var screenOrientation = viewport.IsLandscape ?
                new ScreenOrientation
                {
                    Angle = 90,
                    Type = ScreenOrientationType.LandscapePrimary
                } :
                new ScreenOrientation
                {
                    Angle = 0,
                    Type = ScreenOrientationType.PortraitPrimary
                };
            var hasTouch = viewport.HasTouch;

            await Task.WhenAll(new Task[] {
                _client.SendAsync("Emulation.setDeviceMetricsOverride", new EmulationSetDeviceMetricsOverrideRequest
                {
                    Mobile = mobile,
                    Width = width,
                    Height = height,
                    DeviceScaleFactor = deviceScaleFactor,
                    ScreenOrientation = screenOrientation
                }),
                _client.SendAsync("Emulation.setTouchEmulationEnabled", new EmulationSetTouchEmulationEnabledRequest
                {
                    Enabled = hasTouch,
                    Configuration = viewport.IsMobile ? "mobile" : "desktop"
                })
            }).ConfigureAwait(false);

            var reloadNeeded = _emulatingMobile != mobile || _hasTouch != hasTouch;
            _emulatingMobile = mobile;
            _hasTouch = hasTouch;
            return reloadNeeded;
        }
        #endregion Methods
    }
}
