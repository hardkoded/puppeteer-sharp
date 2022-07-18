using System;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom.Media;
using CefSharp.DevTools.Dom.Messaging;

namespace CefSharp.DevTools.Dom
{
    internal class EmulationManager
    {
        private readonly DevToolsConnection _connection;
        private bool _hasTouch;
        private bool _emulatingMobile;

        public EmulationManager(DevToolsConnection connection)
        {
            _connection = connection;
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

            await Task.WhenAll(new Task[] {
                _connection.SendAsync("Emulation.setDeviceMetricsOverride", new EmulationSetDeviceMetricsOverrideRequest
                {
                    Mobile = mobile,
                    Width = width,
                    Height = height,
                    DeviceScaleFactor = deviceScaleFactor,
                    ScreenOrientation = screenOrientation
                }),
                _connection.SendAsync("Emulation.setTouchEmulationEnabled", new EmulationSetTouchEmulationEnabledRequest
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
    }
}
