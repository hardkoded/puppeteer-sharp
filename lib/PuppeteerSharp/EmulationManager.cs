using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Media;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class EmulationManager
    {
        private CDPSession _client;
        private bool _emulatingMobile;
        private bool _hasTouch;

        public EmulationManager(CDPSession client)
        {
            _client = client;
        }

        public bool JavascriptEnabled { get; private set; } = true;

        internal void UpdateClient(CDPSession client) => _client = client;

        internal async Task EmulateTimezoneAsync(string timezoneId)
        {
            try
            {
                await _client.SendAsync(
                    "Emulation.setTimezoneOverride",
                    new EmulateTimezoneRequest { TimezoneId = timezoneId ?? string.Empty, }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid timezone"))
            {
                throw new PuppeteerException($"Invalid timezone ID: {timezoneId}");
            }
        }

        internal Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
            => _client.SendAsync(
                "Emulation.setEmulatedVisionDeficiency",
                new EmulationSetEmulatedVisionDeficiencyRequest { Type = type, });

        internal Task EmulateCPUThrottlingAsync(decimal? factor = null)
        {
            if (factor is < 1)
            {
                throw new ArgumentException("Throttling rate should be greater or equal to 1", nameof(factor));
            }

            return _client.SendAsync(
                "Emulation.setCPUThrottlingRate",
                new EmulationSetCPUThrottlingRateRequest { Rate = factor ?? 1, });
        }

        internal async Task EmulateIdleStateAsync(EmulateIdleOverrides overrides = null)
        {
            if (overrides != null)
            {
                await _client.SendAsync(
                    "Emulation.setIdleOverride",
                    new EmulationSetIdleOverrideRequest
                    {
                        IsUserActive = overrides.IsUserActive, IsScreenUnlocked = overrides.IsScreenUnlocked,
                    }).ConfigureAwait(false);
            }
            else
            {
                await _client.SendAsync("Emulation.clearIdleOverride").ConfigureAwait(false);
            }
        }

        internal async Task<bool> EmulateViewportAsync(ViewPortOptions viewport)
        {
            var mobile = viewport.IsMobile;
            var width = viewport.Width;
            var height = viewport.Height;
            var deviceScaleFactor = viewport.DeviceScaleFactor;
            var screenOrientation = viewport.IsLandscape
                ? new ScreenOrientation { Angle = 90, Type = ScreenOrientationType.LandscapePrimary, }
                : new ScreenOrientation { Angle = 0, Type = ScreenOrientationType.PortraitPrimary, };
            var hasTouch = viewport.HasTouch;

            await Task.WhenAll(
            [
                _client.SendAsync("Emulation.setDeviceMetricsOverride", new EmulationSetDeviceMetricsOverrideRequest
                {
                    Mobile = mobile,
                    Width = width,
                    Height = height,
                    DeviceScaleFactor = deviceScaleFactor,
                    ScreenOrientation = screenOrientation,
                }),
                _client.SendAsync(
                    "Emulation.setTouchEmulationEnabled",
                    new EmulationSetTouchEmulationEnabledRequest { Enabled = hasTouch, }),
            ]).ConfigureAwait(false);

            var reloadNeeded = _emulatingMobile != mobile || _hasTouch != hasTouch;
            _emulatingMobile = mobile;
            _hasTouch = hasTouch;
            return reloadNeeded;
        }

        internal Task EmulateMediaTypeAsync(MediaType type)
            => _client.SendAsync(
                "Emulation.setEmulatedMedia",
                new EmulationSetEmulatedMediaTypeRequest { Media = type });

        internal Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
            => _client.SendAsync(
                "Emulation.setEmulatedMedia",
                new EmulationSetEmulatedMediaFeatureRequest { Features = features });

        internal Task SetGeolocationAsync(GeolocationOption options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Longitude < -180 || options.Longitude > 180)
            {
                throw new ArgumentException($"Invalid longitude '{options.Longitude}': precondition - 180 <= LONGITUDE <= 180 failed.");
            }

            if (options.Latitude < -90 || options.Latitude > 90)
            {
                throw new ArgumentException($"Invalid latitude '{options.Latitude}': precondition - 90 <= LATITUDE <= 90 failed.");
            }

            if (options.Accuracy < 0)
            {
                throw new ArgumentException($"Invalid accuracy '{options.Accuracy}': precondition 0 <= ACCURACY failed.");
            }

            return _client.SendAsync("Emulation.setGeolocationOverride", options);
        }

        internal Task ResetDefaultBackgroundColorAsync()
            => _client.SendAsync("Emulation.setDefaultBackgroundColorOverride");

        internal Task SetTransparentBackgroundColorAsync()
            => _client.SendAsync("Emulation.setDefaultBackgroundColorOverride", new EmulationSetDefaultBackgroundColorOverrideRequest
            {
                Color = new EmulationSetDefaultBackgroundColorOverrideColor
                {
                    R = 0,
                    G = 0,
                    B = 0,
                    A = 0,
                },
            });

        internal Task SetJavaScriptEnabledAsync(bool enabled)
        {
            if (enabled == JavascriptEnabled)
            {
                return Task.CompletedTask;
            }

            JavascriptEnabled = enabled;
            return _client.SendAsync("Emulation.setScriptExecutionDisabled", new EmulationSetScriptExecutionDisabledRequest
            {
                Value = !enabled,
            });
        }
    }
}
