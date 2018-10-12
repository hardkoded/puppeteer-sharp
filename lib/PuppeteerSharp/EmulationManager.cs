using System;
using System.Threading.Tasks;
using PuppeteerSharp.Media;

namespace PuppeteerSharp
{
    internal class EmulationManager
    {
        private readonly CDPSession _client;
        private bool _hasTouch;
        private bool _emulatingMobile;

        public EmulationManager(CDPSession client)
        {
            _client = client;
        }

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

            await Task.WhenAll(new Task[]{
                _client.SendAsync("Emulation.setDeviceMetricsOverride", new
                {
                    mobile,
                    width,
                    height,
                    deviceScaleFactor,
                    screenOrientation
                }),
                _client.SendAsync("Emulation.setTouchEmulationEnabled", new
                {
                    enabled = hasTouch,
                    configuration = viewport.IsMobile ? "mobile" : "desktop"
                })
            }).ConfigureAwait(false);

            var reloadNeeded = _emulatingMobile != mobile || _hasTouch != hasTouch;
            _emulatingMobile = mobile;
            _hasTouch = hasTouch;
            return reloadNeeded;
        }
    }
}